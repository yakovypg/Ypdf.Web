using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Ypdf.Web.Domain.Models.Configuration;

namespace Ypdf.Web.PdfProcessingAPI.Infrastructure.Services;

public class RabbitMqProducerService : IRabbitMqProducerService
{
    private readonly string _hostName;
    private readonly string _queueName;

    public RabbitMqProducerService(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        _hostName = configuration.GetSection("RabbitMQ:HostName").Value
            ?? throw new ConfigurationException("Host name for RabbitMQ not specified");

        _queueName = configuration.GetSection("RabbitMQ:QueueName").Value
            ?? throw new ConfigurationException("Queue name for RabbitMQ not specified");
    }

    public async Task SendMessageAsync(object obj)
    {
        string? message = JsonSerializer.Serialize(obj);
        await SendMessageAsync(message).ConfigureAwait(false);
    }

    public async Task SendMessageAsync(string message)
    {
        var factory = new ConnectionFactory()
        {
            HostName = _hostName
        };

        using IConnection connection = await factory
            .CreateConnectionAsync()
            .ConfigureAwait(false);

        using IChannel channel = await connection
            .CreateChannelAsync()
            .ConfigureAwait(false);

        _ = await channel.QueueDeclareAsync(
            queue: _queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null)
            .ConfigureAwait(false);

        byte[] body = Encoding.UTF8.GetBytes(message);

        await channel
            .BasicPublishAsync(string.Empty, _queueName, body)
            .ConfigureAwait(false);
    }
}
