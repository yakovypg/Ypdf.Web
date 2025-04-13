using System;
using System.Net;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Ypdf.Web.Domain.Infrastructure.Extensions;
using Ypdf.Web.Domain.Models.Configuration;
using Ypdf.Web.Domain.Models.Informing;
using Ypdf.Web.PdfOperationsHistoryAPI.Infrastructure.Data.Repositories;

namespace Ypdf.Web.PdfOperationsHistoryAPI.Infrastructure.Services;

public class RabbitMqConsumer : BackgroundService
{
    private readonly IPdfOperationResultRepository _pdfOperationResultRepository;
    private readonly ILogger<RabbitMqConsumer> _logger;

    private readonly string _hostName;
    private readonly string _queueName;

    private readonly SecureString? _userName;
    private readonly SecureString? _password;

    private IConnection? _connection;
    private IChannel? _channel;

    private bool _isDisposed;

    public RabbitMqConsumer(
        IPdfOperationResultRepository pdfOperationResultRepository,
        IConfiguration configuration,
        ILogger<RabbitMqConsumer> logger)
    {
        ArgumentNullException.ThrowIfNull(pdfOperationResultRepository, nameof(pdfOperationResultRepository));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));

        _pdfOperationResultRepository = pdfOperationResultRepository;
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

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(cancellationToken, nameof(cancellationToken));

        _logger.LogInformation("Trying to connect to RabbitMQ");

        var factory = new ConnectionFactory()
        {
            HostName = _hostName,
            UserName = new NetworkCredential(string.Empty, _userName).Password,
            Password = new NetworkCredential(string.Empty, _password).Password
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

        _logger.LogInformation("Connection to RabbitMQ established");

        await base
            .StartAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(cancellationToken, nameof(cancellationToken));

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
            throw new ConnectionNotEstablishedException("Connection with RabbitMQ not established.");

        _logger.LogInformation("Trying to start listening RabbitMQ");

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, e) =>
        {
            byte[] body = e.Body.ToArray();
            string content = Encoding.UTF8.GetString(body);

            _logger.LogInformation("Recieved content from RabbitMQ: {Content}", content);

            SaveRecievedData(content);

            await _channel.BasicAckAsync(e.DeliveryTag, false).ConfigureAwait(false);
        };

        Task<string> consumeTask = _channel.BasicConsumeAsync(
            queue: _queueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        _logger.LogInformation("RabbitMQ consume task created");

        _ = await consumeTask.ConfigureAwait(false);
    }

    private void SaveRecievedData(string content)
    {
        ArgumentNullException.ThrowIfNull(content, nameof(content));

        try
        {
            _logger.LogInformation("Trying to deserialize recieved data: {Content}", content);

            PdfOperationResult result = JsonSerializer.Deserialize<PdfOperationResult>(content)
                ?? throw new InvalidOperationException();

            _logger.LogInformation("Recieved data successfully deserialized");
            _logger.LogInformation("Trying to save deserialized data");

            _pdfOperationResultRepository.Add(result);

            _logger.LogInformation("Deserialized data successfully saved");
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to save recieved data. Exception: {@Exception}", ex);
        }
    }
}
