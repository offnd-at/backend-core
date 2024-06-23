namespace OffndAt.Services.Background.Tasks;

using System.Text.Json;
using Application.Core.Abstractions.Messaging;
using Infrastructure.Core.Messaging.Json;
using Infrastructure.Core.Messaging.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

/// <summary>
///     Represents the background service that consumes integration events received from the message queue.
/// </summary>
internal sealed class IntegrationEventConsumerBackgroundService : IHostedService, IDisposable
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        TypeInfoResolver = new IntegrationEventPolymorphicTypeResolver()
    };

    private readonly IModel _channel;
    private readonly IConnection _connection;
    private readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    ///     Initializes a new instance of the <see cref="IntegrationEventConsumerBackgroundService" />
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="messageBrokerOptions">The message broker options.</param>
    /// <param name="logger">The logger.</param>
    public IntegrationEventConsumerBackgroundService(
        IServiceProvider serviceProvider,
        IOptions<MessageBrokerSettings> messageBrokerOptions,
        ILogger<IntegrationEventConsumerBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        var settings = messageBrokerOptions.Value;

        var connectionFactory = new ConnectionFactory
        {
            HostName = settings.Hostname,
            Port = settings.Port,
            UserName = settings.Username,
            Password = settings.Password,
            DispatchConsumersAsync = true
        };

        _connection = connectionFactory.CreateConnection();

        _channel = _connection.CreateModel();

        _channel.CallbackException += OnCallbackException;

        _channel.QueueDeclare(
            settings.QueueName,
            true,
            false,
            false);

        try
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += OnIntegrationEventReceived;

            _channel.BasicConsume(settings.QueueName, false, consumer);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Unable to process integration events");
            throw;
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _channel.Close();
        _connection.Close();
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        Dispose();

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Processes the integration event received from the message queue.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="eventArgs">The event arguments.</param>
    /// <returns>The completed task.</returns>
    private async Task OnIntegrationEventReceived(object? sender, BasicDeliverEventArgs eventArgs)
    {
        var integrationEvent = JsonSerializer.Deserialize<IIntegrationEvent>(eventArgs.Body.Span, JsonSerializerOptions);

        if (integrationEvent is null)
        {
            _logger.LogWarning(
                "Skipping empty integration event, message identifier := {MessageId}",
                eventArgs.BasicProperties.MessageId);

            _channel.BasicAck(eventArgs.DeliveryTag, false);
            return;
        }

        using var scope = _serviceProvider.CreateScope();

        var integrationEventConsumer = scope.ServiceProvider.GetRequiredService<IIntegrationEventConsumer>();

        await integrationEventConsumer.ConsumeAsync(integrationEvent);

        _channel.BasicAck(eventArgs.DeliveryTag, false);
    }

    /// <summary>
    ///     Processes the exception received from the model callback.
    /// </summary>
    /// <param name="channel">The channel.</param>
    /// <param name="eventArgs">The event arguments.</param>
    private void OnCallbackException(object? channel, BaseExceptionEventArgs? eventArgs) =>
        _logger.LogWarning(eventArgs?.Exception, "Encountered exception during messaging");
}
