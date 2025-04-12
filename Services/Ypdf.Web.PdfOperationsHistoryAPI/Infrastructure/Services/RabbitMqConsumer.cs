using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Ypdf.Web.Domain.Models.Configuration;

namespace Ypdf.Web.PdfOperationsHistoryAPI.Infrastructure.Services;

public class RabbitMqConsumer : BackgroundService, IAsyncDisposable
{
    private readonly ILogger<RabbitMqConsumer> _logger;

    private readonly string _hostName;
    private readonly string _queueName;

    private IConnection? _connection;
    private IChannel? _channel;

    private bool _isDisposed;

    public RabbitMqConsumer(IConfiguration configuration, ILogger<RabbitMqConsumer> logger)
    {
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));

        _logger = logger;

        _hostName = configuration.GetSection("RabbitMQ:HostName").Value
            ?? throw new ConfigurationException("Host name for RabbitMQ not specified");

        _queueName = configuration.GetSection("RabbitMQ:QueueName").Value
            ?? throw new ConfigurationException("Queue name for RabbitMQ not specified");
    }

    ~RabbitMqConsumer()
    {
        DisposeAsync(false)
            .AsTask()
            .GetAwaiter()
            .GetResult();
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(cancellationToken, nameof(cancellationToken));

        var factory = new ConnectionFactory()
        {
            HostName = _hostName
        };

        _connection = await factory
            .CreateConnectionAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        _channel = await _connection
            .CreateChannelAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        Task<QueueDeclareOk> queueDeclareTask = _channel.QueueDeclareAsync(
            queue: _queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        await queueDeclareTask.ConfigureAwait(false);

        await base
            .StartAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true)
            .ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                if (_channel is not null)
                {
                    await _channel
                        .CloseAsync()
                        .ConfigureAwait(false);
                }

                if (_connection is not null)
                {
                    await _connection
                        .CloseAsync()
                        .ConfigureAwait(false);
                }
            }

            _isDisposed = true;
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ArgumentNullException.ThrowIfNull(stoppingToken, nameof(stoppingToken));

        stoppingToken.ThrowIfCancellationRequested();

        if (_channel is null)
            throw new ConnectionNotEstablishedException("Connection with RabbitMQ not established.");

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, e) =>
        {
            byte[] body = e.Body.ToArray();
            string content = Encoding.UTF8.GetString(body);

            _logger.LogInformation("Recieved content from RabbitMQ: {Content}", content);

            await _channel.BasicAckAsync(e.DeliveryTag, false).ConfigureAwait(false);
        };

        Task<string> consumeTask = _channel.BasicConsumeAsync(
            queue: _queueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        _ = await consumeTask.ConfigureAwait(false);
    }
}
