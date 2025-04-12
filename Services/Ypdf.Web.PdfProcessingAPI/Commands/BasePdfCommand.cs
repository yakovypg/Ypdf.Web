using System;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ypdf.Web.Domain.Commands;
using Ypdf.Web.Domain.Infrastructure.Extensions;
using Ypdf.Web.Domain.Models.Configuration;
using Ypdf.Web.Domain.Models.Informing;
using Ypdf.Web.PdfProcessingAPI.Models.Dto.Requests;
using Ypdf.Web.PdfProcessingAPI.Models.Dto.Responses;
using Ypdf.Web.PdfProcessingAPI.Services;

namespace Ypdf.Web.PdfProcessingAPI.Commands;

public abstract class BasePdfCommand<TRequest> : BaseCommand, ICommand<TRequest, PdfOperationResponse>
    where TRequest : IPdfCommandRequest
{
    protected BasePdfCommand(
        string commandName,
        PdfOperationType operationType,
        IRabbitMqSenderService rabbitMqSenderService,
        IConfiguration configuration,
        IMapper mapper,
        ILogger<BaseCommand> logger)
        : base(
            mapper ?? throw new ArgumentNullException(nameof(mapper)),
            logger ?? throw new ArgumentNullException(nameof(logger)))
    {
        ArgumentNullException.ThrowIfNull(commandName, nameof(commandName));
        ArgumentNullException.ThrowIfNull(rabbitMqSenderService, nameof(rabbitMqSenderService));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        CommandName = commandName;
        OperationType = operationType;
        RabbitMqSenderService = rabbitMqSenderService;
        Configuration = configuration;
    }

    protected string CommandName { get; }
    protected PdfOperationType OperationType { get; }

    protected IRabbitMqSenderService RabbitMqSenderService { get; }
    protected IConfiguration Configuration { get; }

    public virtual async Task<PdfOperationResponse> ExecuteAsync(TRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        string requestJson = await request
            .ToJsonAsync()
            .ConfigureAwait(false);

        Logger.LogInformation("Execute {CommandName} with {RequestJson}", CommandName, requestJson);

        string outputPath = GetOutputPath();
        Logger.LogInformation("Output path: {OutputPath}", outputPath);

        Task<(DateTime, DateTime)> commandTask = GetCommandTask(request, outputPath);

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

        return new PdfOperationResponse(operationResult);
    }

    protected virtual string GetRootOutputFilesDirectoryPath()
    {
        return Configuration.GetSection("Storages:OutputFiles").Value
            ?? throw new ConfigurationException("Output files directory not specified");
    }

    protected virtual string GetDefaultOutputDirectoryPath()
    {
        string outputFilesDirectoryPath = GetRootOutputFilesDirectoryPath();
        string outputFileName = Guid.NewGuid().ToString();

        return Path.Combine(outputFilesDirectoryPath, outputFileName);
    }

    protected virtual string GetDefaultOutputFilePath()
    {
        string outputFilesDirectoryPath = GetRootOutputFilesDirectoryPath();
        string outputFileName = $"{Guid.NewGuid()}.pdf";

        return Path.Combine(outputFilesDirectoryPath, outputFileName);
    }

    protected abstract string GetOutputPath();

    protected abstract Task<(DateTime OperationStart, DateTime OperationEnd)> GetCommandTask(
        TRequest request,
        string outputPath);
}
