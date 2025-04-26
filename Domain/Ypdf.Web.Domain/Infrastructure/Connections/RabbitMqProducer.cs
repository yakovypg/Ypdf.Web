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

namespace Ypdf.Web.Domain.Infrastructure.Connections;

public class RabbitMqProducer : IRabbitMqProducer, IDisposable
{
    private const string DefaultHostNameConfigPath = "RabbitMQ:HostName";
    private const string DefaultQueueNameConfigPath = "RabbitMQ:QueueName";
    private const string DefaultUserNameConfigPath = "RabbitMQ:UserName";
    private const string DefaultPasswordConfigPath = "RabbitMQ:Password";

    private readonly SecureString _userName;
    private readonly SecureString _password;

    private bool _isDisposed;

    public RabbitMqProducer(IConfiguration configuration, ILogger<RabbitMqProducer> logger)
        : this(configuration, logger, DefaultUserNameConfigPath, DefaultPasswordConfigPath)
    {
    }

    public RabbitMqProducer(
        IConfiguration configuration,
        ILogger<RabbitMqProducer> logger,
        string userNameConfigPath,
        string passwordConfigPath)
    {
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));
        ArgumentNullException.ThrowIfNull(userNameConfigPath, nameof(userNameConfigPath));
        ArgumentNullException.ThrowIfNull(passwordConfigPath, nameof(passwordConfigPath));

        Configuration = configuration;
        Logger = logger;

        _userName = new SecureString();
        _password = new SecureString();

        _userName.Enrich(
            configuration[userNameConfigPath]
                ?? throw new ConfigurationException("User name for RabbitMQ not specified"));

        _password.Enrich(
            configuration[passwordConfigPath]
                ?? throw new ConfigurationException("Password for RabbitMQ user not specified"));
    }

    ~RabbitMqProducer()
    {
        Dispose(false);
    }

    protected IConfiguration Configuration { get; }
    protected ILogger<RabbitMqProducer> Logger { get; }

    protected virtual string HostNameConfigPath => DefaultHostNameConfigPath;
    protected virtual string QueueNameConfigPath => DefaultQueueNameConfigPath;

    public async Task SendMessageAsync(object obj)
    {
        ArgumentNullException.ThrowIfNull(obj, nameof(obj));

        string message = JsonSerializer.Serialize(obj);
        await SendMessageAsync(message).ConfigureAwait(false);
    }

    public async Task SendMessageAsync(string message)
    {
        ArgumentNullException.ThrowIfNull(message, nameof(message));

        Logger.LogInformation("Trying to connect to RabbitMQ");

        string hostName = GetHostName();
        string queueName = GetQueueName();

        var factory = new ConnectionFactory()
        {
            HostName = hostName,
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
            queue: queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        _ = await queueDeclareTask.ConfigureAwait(false);

        Logger.LogInformation("Connection to RabbitMQ established");

        byte[] body = Encoding.UTF8.GetBytes(message);

        Logger.LogInformation("Trying to send message to RabbitMQ: {Message}", message);

        await channel
            .BasicPublishAsync(string.Empty, queueName, body)
            .ConfigureAwait(false);

        Logger.LogInformation("Message to RabbitMQ was sent");
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

    private string GetHostName()
    {
        return Configuration.GetSection(HostNameConfigPath).Value
            ?? throw new ConfigurationException("Host name for RabbitMQ not specified");
    }

    private string GetQueueName()
    {
        return Configuration.GetSection(QueueNameConfigPath).Value
            ?? throw new ConfigurationException("Queue name for RabbitMQ not specified");
    }
}
