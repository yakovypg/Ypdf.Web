using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ypdf.Web.Domain.Infrastructure.Connections;
using Ypdf.Web.Domain.Models.Informing;
using Ypdf.Web.PdfOperationsHistoryAPI.Infrastructure.Data.Repositories;

namespace Ypdf.Web.PdfOperationsHistoryAPI.Infrastructure.Connections;

public class PdfOperationResultRabbitMqConsumer : RabbitMqConsumer
{
    private readonly IPdfOperationResultRepository _pdfOperationResultRepository;

    public PdfOperationResultRabbitMqConsumer(
        IPdfOperationResultRepository pdfOperationResultRepository,
        IConfiguration configuration,
        ILogger<RabbitMqConsumer> logger)
        : base(
            configuration ?? throw new ArgumentNullException(nameof(configuration)),
            logger ?? throw new ArgumentNullException(nameof(logger)))
    {
        ArgumentNullException.ThrowIfNull(pdfOperationResultRepository, nameof(pdfOperationResultRepository));
        _pdfOperationResultRepository = pdfOperationResultRepository;
    }

    protected override string QueueNameConfigPath => "RabbitMQ:PdfOperationResultQueueName";

    protected override Task SaveRecievedDataAsync(string content)
    {
        ArgumentNullException.ThrowIfNull(content, nameof(content));

        try
        {
            PdfOperationResult result = DeserializePdfOperationResult(content);
            SavePdfOperationResult(result);
        }
        catch (Exception ex)
        {
            Logger.LogWarning("Failed to save recieved data");
            Logger.LogError(ex, "{@Exception}", ex);
        }

        return Task.CompletedTask;
    }

    private PdfOperationResult DeserializePdfOperationResult(string content)
    {
        ArgumentNullException.ThrowIfNull(content, nameof(content));

        Logger.LogInformation("Trying to deserialize recieved data: {Content}", content);

        PdfOperationResult result = JsonSerializer.Deserialize<PdfOperationResult>(content)
            ?? throw new InvalidOperationException();

        Logger.LogInformation("Recieved data successfully deserialized");

        return result;
    }

    private void SavePdfOperationResult(PdfOperationResult result)
    {
        ArgumentNullException.ThrowIfNull(result, nameof(result));

        Logger.LogInformation("Trying to save deserialized data");
        _pdfOperationResultRepository.Add(result);
        Logger.LogInformation("Deserialized data successfully saved");
    }
}
