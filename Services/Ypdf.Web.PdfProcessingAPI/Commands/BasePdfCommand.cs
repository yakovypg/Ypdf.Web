using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ypdf.Web.Domain.Commands;
using Ypdf.Web.Domain.Infrastructure.Extensions;
using Ypdf.Web.Domain.Models.Api.Exceptions;
using Ypdf.Web.Domain.Models.Configuration;
using Ypdf.Web.Domain.Models.Informing;
using Ypdf.Web.PdfProcessingAPI.Infrastructure.Services;
using Ypdf.Web.PdfProcessingAPI.Models.Requests;
using Ypdf.Web.PdfProcessingAPI.Models.Responses;

namespace Ypdf.Web.PdfProcessingAPI.Commands;

public abstract class BasePdfCommand<TRequest> : BaseCommand, IProtectedCommand<TRequest, PdfOperationResponse>
{
    protected BasePdfCommand(
        string commandName,
        PdfOperationType operationType,
        ISubscriptionInfoService subscriptionInfoService,
        IOutputFilePathService outputFilePathService,
        IRabbitMqProducerService rabbitMqSenderService,
        IConfiguration configuration,
        IMapper mapper,
        ILogger<BaseCommand> logger)
        : base(
            mapper ?? throw new ArgumentNullException(nameof(mapper)),
            logger ?? throw new ArgumentNullException(nameof(logger)))
    {
        ArgumentNullException.ThrowIfNull(commandName, nameof(commandName));
        ArgumentNullException.ThrowIfNull(subscriptionInfoService, nameof(subscriptionInfoService));
        ArgumentNullException.ThrowIfNull(outputFilePathService, nameof(outputFilePathService));
        ArgumentNullException.ThrowIfNull(rabbitMqSenderService, nameof(rabbitMqSenderService));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        CommandName = commandName;
        OperationType = operationType;
        SubscriptionInfoService = subscriptionInfoService;
        OutputFilePathService = outputFilePathService;
        RabbitMqSenderService = rabbitMqSenderService;
        Configuration = configuration;
    }

    protected string CommandName { get; }
    protected PdfOperationType OperationType { get; }

    protected ISubscriptionInfoService SubscriptionInfoService { get; }
    protected IOutputFilePathService OutputFilePathService { get; }
    protected IRabbitMqProducerService RabbitMqSenderService { get; }
    protected IConfiguration Configuration { get; }

    public virtual async Task<PdfOperationResponse> ExecuteAsync(
        TRequest request,
        ClaimsPrincipal userClaims)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        ArgumentNullException.ThrowIfNull(userClaims, nameof(userClaims));

        string requestJson = await request
            .ToJsonAsync()
            .ConfigureAwait(false);

        Logger.LogInformation("Execute {CommandName} with {RequestJson}", CommandName, requestJson);

        await VerifyAccessToOperationAsync(userClaims)
            .ConfigureAwait(false);

        int userId = GetUserId(userClaims);

        (string outputFileName, string outputFilePath) = GetOutputFilePath();
        Logger.LogInformation("Output file path: {OutputPath}", outputFilePath);

        Task<(DateTimeOffset, DateTimeOffset)> commandTask = GetCommandTask(request, outputFilePath);

        (DateTimeOffset operationStart, DateTimeOffset operationEnd) = await commandTask
            .ConfigureAwait(false);

        Logger.LogInformation("{CommandName} successfully executed", CommandName);

        var operationResult = new PdfOperationResult()
        {
            UserId = userId,
            OperationType = OperationType,
            StartDate = operationStart,
            EndDate = operationEnd
        };

        await RabbitMqSenderService
            .SendMessageAsync(operationResult)
            .ConfigureAwait(false);

        return new PdfOperationResponse(outputFileName, operationResult);
    }

    protected virtual (string FileName, string FilePath) GetOutputFilePath()
    {
        string fileName = OutputFilePathService.GetNextOutputFileName("pdf");
        string filePath = OutputFilePathService.GetOutputFilePath(fileName);

        return (fileName, filePath);
    }

    protected virtual void ValidateRequestParameters(ISingleFilePdfCommandRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        if (request.File is null)
            throw new BadRequestException("File not specified");
    }

    protected virtual void ValidateRequestParameters(IMultipleFilesPdfCommandRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        if (request.Files is null)
            throw new BadRequestException("Files not specified");
    }

    protected virtual async Task VerifyAccessToOperationAsync(ClaimsPrincipal userClaims)
    {
        ArgumentNullException.ThrowIfNull(userClaims, nameof(userClaims));

        bool hasAccess = await HasUserAccessToOperationAsync(userClaims)
            .ConfigureAwait(false);

        if (!hasAccess)
        {
            Logger.LogWarning(
                "User {@User} doesn't have access to the {OperationType} operation",
                userClaims.ToTypeValuePairs(),
                OperationType);

            throw new ForbiddenException("User doesn't have access to the resource");
        }
    }

    protected virtual async Task<bool> HasUserAccessToOperationAsync(ClaimsPrincipal userClaims)
    {
        ArgumentNullException.ThrowIfNull(userClaims, nameof(userClaims));

        string? subscriptionName = userClaims.Get(JwtCustomClaimNames.Subscription);

        if (string.IsNullOrEmpty(subscriptionName))
            return false;

        string operationName = OperationType.ToString();

        return await SubscriptionInfoService
            .IsOperationAllowedAsync(subscriptionName, operationName)
            .ConfigureAwait(false);
    }

    protected abstract Task<(DateTimeOffset OperationStart, DateTimeOffset OperationEnd)> GetCommandTask(
        TRequest request,
        string outputFilePath);

    private int GetUserId(ClaimsPrincipal userClaims)
    {
        ArgumentNullException.ThrowIfNull(userClaims, nameof(userClaims));

        if (!userClaims.Get(NetCoreIdentityClaimNames.Sub, out int userId))
        {
            Logger.LogWarning(
                "Cannot get user id from claims: {UserClaims}",
                userClaims.ToTypeValuePairs());

            throw new ForbiddenException();
        }

        return userId;
    }
}
