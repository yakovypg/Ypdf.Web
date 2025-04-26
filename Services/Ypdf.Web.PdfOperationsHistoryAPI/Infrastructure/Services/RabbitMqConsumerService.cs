using System;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ypdf.Web.Domain.Models.Informing;
using Ypdf.Web.PdfOperationsHistoryAPI.Infrastructure.Data.Repositories;

namespace Ypdf.Web.PdfOperationsHistoryAPI.Infrastructure.Services;

public class RabbitMqConsumerService : RabbitMqConsumer
{
    private readonly IPdfOperationResultRepository _pdfOperationResultRepository;

    public RabbitMqConsumerService(
        IPdfOperationResultRepository pdfOperationResultRepository,
        IConfiguration configuration,
        ILogger<RabbitMqConsumer> logger)
        : base(configuration, logger)
    {
        ArgumentNullException.ThrowIfNull(pdfOperationResultRepository, nameof(pdfOperationResultRepository));
        _pdfOperationResultRepository = pdfOperationResultRepository;
    }

    protected override void SaveRecievedData(string content)
    {
        ArgumentNullException.ThrowIfNull(content, nameof(content));

        try
        {
            Logger.LogInformation("Trying to deserialize recieved data: {Content}", content);

            PdfOperationResult result = JsonSerializer.Deserialize<PdfOperationResult>(content)
                ?? throw new InvalidOperationException();

            Logger.LogInformation("Recieved data successfully deserialized");
            Logger.LogInformation("Trying to save deserialized data");

            _pdfOperationResultRepository.Add(result);

            Logger.LogInformation("Deserialized data successfully saved");
        }
        catch (Exception ex)
        {
            Logger.LogError("Failed to save recieved data. Exception: {@Exception}", ex);
        }
    }
}
