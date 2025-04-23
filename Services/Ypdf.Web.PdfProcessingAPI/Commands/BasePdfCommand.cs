using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ypdf.Web.Domain.Commands;
using Ypdf.Web.Domain.Infrastructure.Extensions;
using Ypdf.Web.Domain.Models.Api.Exceptions;
using Ypdf.Web.Domain.Models.Informing;
using Ypdf.Web.PdfProcessingAPI.Infrastructure.Services;
using Ypdf.Web.PdfProcessingAPI.Models.Requests;
using Ypdf.Web.PdfProcessingAPI.Models.Responses;

namespace Ypdf.Web.PdfProcessingAPI.Commands;

public abstract class BasePdfCommand<TRequest> : BaseCommand, IProtectedCommand<TRequest, PdfOperationResponse>
    where TRequest : IPdfCommandRequest
{
    protected BasePdfCommand(
        string commandName,
        PdfOperationType operationType,
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
        ArgumentNullException.ThrowIfNull(outputFilePathService, nameof(outputFilePathService));
        ArgumentNullException.ThrowIfNull(rabbitMqSenderService, nameof(rabbitMqSenderService));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        CommandName = commandName;
        OperationType = operationType;
        OutputFilePathService = outputFilePathService;
        RabbitMqSenderService = rabbitMqSenderService;
        Configuration = configuration;
    }

    protected string CommandName { get; }
    protected PdfOperationType OperationType { get; }

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

        VerifyAccess(userClaims);

        (string outputFileName, string outputFilePath) = GetOutputFilePath();
        Logger.LogInformation("Output file path: {OutputPath}", outputFilePath);

        Task<(DateTimeOffset, DateTimeOffset)> commandTask = GetCommandTask(request, outputFilePath);

        (DateTimeOffset operationStart, DateTimeOffset operationEnd) = await commandTask
            .ConfigureAwait(false);

        Logger.LogInformation("{CommandName} successfully executed", CommandName);

        var operationResult = new PdfOperationResult()
        {
            UserId = request.UserId,
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

    protected virtual void VerifyAccess(ClaimsPrincipal userClaims)
    {
        ArgumentNullException.ThrowIfNull(userClaims, nameof(userClaims));

        if (!HasAccess(userClaims))
        {
            Logger.LogWarning(
                "User {@User} doesn't have access to the {OperationType} operation",
                userClaims,
                OperationType);

            throw new ForbiddenException("User doesn't have access to the resource");
        }
    }

    protected abstract bool HasAccess(ClaimsPrincipal userClaims);

    protected abstract Task<(DateTimeOffset OperationStart, DateTimeOffset OperationEnd)> GetCommandTask(
        TRequest request,
        string outputFilePath);
}
