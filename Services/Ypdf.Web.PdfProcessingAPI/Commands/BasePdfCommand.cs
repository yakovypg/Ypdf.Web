using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ypdf.Web.Domain.Commands;
using Ypdf.Web.Domain.Infrastructure.Extensions;
using Ypdf.Web.Domain.Models.Api.Exceptions;
using Ypdf.Web.Domain.Models.Informing;
using Ypdf.Web.PdfProcessingAPI.Infrastructure.Services;
using Ypdf.Web.PdfProcessingAPI.Models.Dto.Requests;
using Ypdf.Web.PdfProcessingAPI.Models.Dto.Responses;

namespace Ypdf.Web.PdfProcessingAPI.Commands;

public abstract class BasePdfCommand<TRequest> : BaseCommand, ICommand<TRequest, PdfOperationResponse>
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

    public virtual async Task<PdfOperationResponse> ExecuteAsync(TRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        string requestJson = await request
            .ToJsonAsync()
            .ConfigureAwait(false);

        Logger.LogInformation("Execute {CommandName} with {RequestJson}", CommandName, requestJson);

        (string outputFileName, string outputFilePath) = GetOutputFilePath();
        Logger.LogInformation("Output file path: {OutputPath}", outputFilePath);

        Task<(DateTime, DateTime)> commandTask = GetCommandTask(request, outputFilePath);

        (DateTime operationStart, DateTime operationEnd) = await commandTask.ConfigureAwait(false);
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
            throw new BadRequestException("File not specified.");
    }

    protected virtual void ValidateRequestParameters(IMultipleFilesPdfCommandRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        if (request.Files is null)
            throw new BadRequestException("Files not specified.");
    }

    protected abstract Task<(DateTime OperationStart, DateTime OperationEnd)> GetCommandTask(
        TRequest request,
        string outputFilePath);
}
