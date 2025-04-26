using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ypdf.Web.Domain.Infrastructure.Connections;

namespace Ypdf.Web.FilesAPI.Infrastructure.Connections;

public class SavedFileRabbitMqProducer : RabbitMqProducer
{
    public SavedFileRabbitMqProducer(
        IConfiguration configuration,
        ILogger<RabbitMqProducer> logger)
        : base(configuration, logger)
    {
    }

    protected override string QueueNameConfigPath => "RabbitMQ:SavedFileQueueName";
}
