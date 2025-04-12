using System;
using System.Net;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Ypdf.Web.Domain.Infrastructure.Extensions;
using Ypdf.Web.Domain.Models.Configuration;

namespace Ypdf.Web.PdfProcessingAPI.Infrastructure.Services;

public class RabbitMqProducerService : IRabbitMqProducerService, IDisposable
{
    private readonly ILogger<RabbitMqProducerService> _logger;

    private readonly string _hostName;
    private readonly string _queueName;
    private readonly SecureString _userName;
    private readonly SecureString _password;

    private bool _isDisposed;

    public RabbitMqProducerService(
        IConfiguration configuration,
        ILogger<RabbitMqProducerService> logger)
    {
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));

        _logger = logger;

        _hostName = configuration.GetSection("RabbitMQ:HostName").Value
            ?? throw new ConfigurationException("Host name for RabbitMQ not specified");

        _queueName = configuration.GetSection("RabbitMQ:QueueName").Value
            ?? throw new ConfigurationException("Queue name for RabbitMQ not specified");

        _userName = new SecureString();
        _password = new SecureString();

        _userName.Enrich(
            configuration["RabbitMQ:UserName"]
                ?? throw new ConfigurationException("User name for RabbitMQ not specified"));

        _password.Enrich(
            configuration["RabbitMQ:Password"]
                ?? throw new ConfigurationException("Password for RabbitMQ user not specified"));
    }

    ~RabbitMqProducerService()
    {
        Dispose(false);
    }

    public async Task SendMessageAsync(object obj)
    {
        ArgumentNullException.ThrowIfNull(obj, nameof(obj));

        string message = JsonSerializer.Serialize(obj);
        await SendMessageAsync(message).ConfigureAwait(false);
    }

    public async Task SendMessageAsync(string message)
    {
        ArgumentNullException.ThrowIfNull(message, nameof(message));

        var factory = new ConnectionFactory()
        {
            HostName = _hostName,
            UserName = new NetworkCredential(string.Empty, _userName).Password,
            Password = new NetworkCredential(string.Empty, _password).Password
        };

        using IConnection connection = await factory
            .CreateConnectionAsync()
            .ConfigureAwait(false);

        using IChannel channel = await connection
            .CreateChannelAsync()
            .ConfigureAwait(false);

        Task<QueueDeclareOk> queueDeclareTask = channel.QueueDeclareAsync(
            queue: _queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        _ = await queueDeclareTask.ConfigureAwait(false);

        byte[] body = Encoding.UTF8.GetBytes(message);

        _logger.LogInformation("Send message to RabbitMQ: {Message}", message);

        await channel
            .BasicPublishAsync(string.Empty, _queueName, body)
            .ConfigureAwait(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                _userName?.Dispose();
                _password?.Dispose();
            }

            _isDisposed = true;
        }
    }
}
