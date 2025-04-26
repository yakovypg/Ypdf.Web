using System;
using System.Net;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Ypdf.Web.Domain.Infrastructure.Extensions;
using Ypdf.Web.Domain.Models.Configuration;

namespace Ypdf.Web.Domain.Infrastructure.Connections;

public abstract class RabbitMqConsumer : BackgroundService
{
    private const string DefaultHostNameConfigPath = "RabbitMQ:HostName";
    private const string DefaultQueueNameConfigPath = "RabbitMQ:QueueName";
    private const string DefaultUserNameConfigPath = "RabbitMQ:UserName";
    private const string DefaultPasswordConfigPath = "RabbitMQ:Password";

    private string? _hostName;
    private string? _queueName;

    private SecureString? _userName;
    private SecureString? _password;

    private IConnection? _connection;
    private IChannel? _channel;

    private bool _isDisposed;

    protected RabbitMqConsumer(
        IConfiguration configuration,
        ILogger<RabbitMqConsumer> logger)
    {
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));

        Configuration = configuration;
        Logger = logger;
    }

    protected IConfiguration Configuration { get; }
    protected ILogger<RabbitMqConsumer> Logger { get; }

    protected virtual string HostNameConfigPath => DefaultHostNameConfigPath;
    protected virtual string QueueNameConfigPath => DefaultQueueNameConfigPath;
    protected virtual string UserNameConfigPath => DefaultUserNameConfigPath;
    protected virtual string PasswordConfigPath => DefaultPasswordConfigPath;

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("Trying to connect to RabbitMQ");

        InitNames();
        InitCredentials();

        await InitConnectionAsync(cancellationToken)
            .ConfigureAwait(false);

        await InitQueueAsync(cancellationToken)
            .ConfigureAwait(false);

        Logger.LogInformation("Connection to RabbitMQ established");

        await base
            .StartAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null)
        {
            await _channel
                .CloseAsync(cancellationToken)
                .ConfigureAwait(false);

            _channel = null;
        }

        if (_connection is not null)
        {
            await _connection
                .CloseAsync(cancellationToken)
                .ConfigureAwait(false);

            _connection = null;
        }

        await base
            .StopAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public override void Dispose()
    {
        Dispose(true);
        base.Dispose();

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

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ArgumentNullException.ThrowIfNull(stoppingToken, nameof(stoppingToken));

        stoppingToken.ThrowIfCancellationRequested();

        if (_channel is null)
            throw new ConnectionNotEstablishedException("Connection with RabbitMQ not established");

        Logger.LogInformation("Trying to start listening RabbitMQ");

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, e) =>
        {
            byte[] body = e.Body.ToArray();
            string content = Encoding.UTF8.GetString(body);

            Logger.LogInformation("Recieved content from RabbitMQ: {Content}", content);

            await SaveRecievedDataAsync(content)
                .ConfigureAwait(false);

            await _channel.BasicAckAsync(e.DeliveryTag, false).ConfigureAwait(false);
        };

        Task<string> consumeTask = _channel.BasicConsumeAsync(
            queue: _queueName!,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        Logger.LogInformation("RabbitMQ consume task created");

        _ = await consumeTask.ConfigureAwait(false);
    }

    protected abstract Task SaveRecievedDataAsync(string content);

    private void InitNames()
    {
        _hostName = Configuration.GetSection(HostNameConfigPath).Value
            ?? throw new ConfigurationException("Host name for RabbitMQ not specified");

        _queueName = Configuration.GetSection(QueueNameConfigPath).Value
            ?? throw new ConfigurationException("Queue name for RabbitMQ not specified");
    }

    private void InitCredentials()
    {
        _userName = new SecureString();
        _password = new SecureString();

        _userName.Enrich(Configuration[UserNameConfigPath]
            ?? throw new ConfigurationException("User name for RabbitMQ not specified"));

        _password.Enrich(Configuration[PasswordConfigPath]
            ?? throw new ConfigurationException("Password for RabbitMQ user not specified"));
    }

    private async Task InitConnectionAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory()
        {
            HostName = _hostName!,
            UserName = new NetworkCredential(string.Empty, _userName).Password,
            Password = new NetworkCredential(string.Empty, _password).Password
        };

        _connection = await factory
            .CreateConnectionAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        _channel = await _connection
            .CreateChannelAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    private async Task InitQueueAsync(CancellationToken cancellationToken)
    {
        Task<QueueDeclareOk> queueDeclareTask = _channel!.QueueDeclareAsync(
            queue: _queueName!,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        await queueDeclareTask.ConfigureAwait(false);
    }
}
