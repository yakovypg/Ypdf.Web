using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ypdf.Web.Domain.Infrastructure.Connections;

namespace Ypdf.Web.FilesAPI.Infrastructure.Connections;

public class PdfOperationResultRabbitMqProducer : RabbitMqProducer
{
    public PdfOperationResultRabbitMqProducer(
        IConfiguration configuration,
        ILogger<RabbitMqProducer> logger)
        : base(
            configuration ?? throw new ArgumentNullException(nameof(configuration)),
            logger ?? throw new ArgumentNullException(nameof(logger)))
    {
    }

    protected override string QueueNameConfigPath => "RabbitMQ:PdfOperationResultQueueName";
}
