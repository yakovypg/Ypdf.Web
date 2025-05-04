using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ypdf.Web.Domain.Infrastructure.Connections;
using Ypdf.Web.Domain.Models.Api.Requests;
using Ypdf.Web.Domain.Models.Informing;
using Ypdf.Web.PdfProcessingAPI.Commands;

namespace Ypdf.Web.FilesAPI.Infrastructure.Connections;

public class SavedFileRabbitMqConsumer : RabbitMqConsumer
{
    private readonly Dictionary<PdfOperationType, BasePdfCommand> _commands;

    public SavedFileRabbitMqConsumer(
        MergeCommand mergeCommand,
        SplitCommand splitCommand,
        IConfiguration configuration,
        ILogger<RabbitMqConsumer> logger)
        : base(
            configuration ?? throw new ArgumentNullException(nameof(configuration)),
            logger ?? throw new ArgumentNullException(nameof(logger)))
    {
        _commands = new()
        {
            { PdfOperationType.Merge, mergeCommand },
            { PdfOperationType.Split, splitCommand }
        };
    }

    protected override string QueueNameConfigPath => "RabbitMQ:SavedFileQueueName";

    protected override async Task SaveRecievedDataAsync(string content)
    {
        ArgumentNullException.ThrowIfNull(content, nameof(content));

        try
        {
            PdfOperationData operationData = DeserializePdfOperationData(content);
            BasePdfCommand command = FindCommand(operationData.OperationType);

            var request = new ExecuteToolRequest()
            {
                PdfOperationData = operationData
            };

            await command
                .ExecuteAsync(request)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogWarning("Failed to process operation");
            Logger.LogError(ex, "{@Exception}", ex);
        }
    }

    private PdfOperationData DeserializePdfOperationData(string content)
    {
        ArgumentNullException.ThrowIfNull(content, nameof(content));

        Logger.LogInformation("Trying to deserialize recieved data: {Content}", content);

        PdfOperationData operationData = JsonSerializer.Deserialize<PdfOperationData>(content)
            ?? throw new InvalidOperationException();

        Logger.LogInformation("Recieved data successfully deserialized");

        return operationData;
    }

    private BasePdfCommand FindCommand(PdfOperationType operationType)
    {
        Logger.LogInformation(
            "Trying to find processor for operation {OperationType}",
            operationType);

        bool commandFound = _commands.TryGetValue(
            operationType,
            out BasePdfCommand? command);

        if (!commandFound || command is null)
        {
            Logger.LogWarning(
                "Processor for {OperationType} operation not found",
                operationType);

            throw new KeyNotFoundException();
        }

        Logger.LogInformation(
            "Processor for operation {OperationType} found",
            operationType);

        return command;
    }
}
