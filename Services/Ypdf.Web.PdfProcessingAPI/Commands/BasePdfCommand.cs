using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Ypdf.Web.Domain.Commands;
using Ypdf.Web.Domain.Infrastructure.Extensions;
using Ypdf.Web.Domain.Models.Api.Exceptions;
using Ypdf.Web.Domain.Models.Informing;
using Ypdf.Web.FilesAPI.Infrastructure.Connections;
using Ypdf.Web.PdfProcessingAPI.Models.Requests;
using Ypdf.Web.PdfProcessingAPI.Models.Responses;

namespace Ypdf.Web.PdfProcessingAPI.Commands;

public abstract class BasePdfCommand : BaseCommand, ICommand<ExecuteToolRequest, ExecuteToolResponse>
{
    private readonly PdfOperationResultRabbitMqProducer _rabbitMqProducer;

    protected BasePdfCommand(
        PdfOperationType operationType,
        PdfOperationResultRabbitMqProducer rabbitMqProducer,
        IMapper mapper,
        ILogger<BaseCommand> logger)
        : base(
            mapper ?? throw new ArgumentNullException(nameof(mapper)),
            logger ?? throw new ArgumentNullException(nameof(logger)))
    {
        ArgumentNullException.ThrowIfNull(rabbitMqProducer, nameof(rabbitMqProducer));

        OperationType = operationType;
        _rabbitMqProducer = rabbitMqProducer;
    }

    protected PdfOperationType OperationType { get; }

    public virtual async Task<ExecuteToolResponse> ExecuteAsync(ExecuteToolRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        ValidateRequestParameters(request);

        string operationParameters = await request
            .ToJsonAsync()
            .ConfigureAwait(false);

        Logger.LogInformation(
            "Execute {OperationType} operation: {Parameters}",
            OperationType,
            operationParameters);

        (DateTimeOffset operationStart, DateTimeOffset operationEnd) = await
            GetCommandTask(request.PdfOperationData!)
            .ConfigureAwait(false);

        Logger.LogInformation("{OperationType} operation executed", OperationType);

        var operationResult = new PdfOperationResult()
        {
            UserId = request.PdfOperationData!.UserId,
            OperationType = OperationType,
            StartDate = operationStart,
            EndDate = operationEnd
        };

        await _rabbitMqProducer
            .SendMessageAsync(operationResult)
            .ConfigureAwait(false);

        return new ExecuteToolResponse(operationResult);
    }

    protected abstract Task<(DateTimeOffset OperationStart, DateTimeOffset OperationEnd)> GetCommandTask(
        PdfOperationData pdfOperationData);

    private static void ValidateRequestParameters(ExecuteToolRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        PdfOperationData? pdfOperationData = request.PdfOperationData;

        if (pdfOperationData is null
            || pdfOperationData.InputFilePaths is null
            || !pdfOperationData.InputFilePaths.Any()
            || string.IsNullOrWhiteSpace(pdfOperationData.OutputFilePath))
        {
            throw new BadRequestException("Files not specified");
        }
    }
}
