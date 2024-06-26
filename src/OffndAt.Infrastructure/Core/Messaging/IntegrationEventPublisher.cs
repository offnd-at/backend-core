﻿namespace OffndAt.Infrastructure.Core.Messaging;

using System.Text;
using System.Text.Json;
using Application.Core.Abstractions.Messaging;
using Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Settings;

/// <summary>
///     Represents the integration event publisher.
/// </summary>
internal sealed class IntegrationEventPublisher : IIntegrationEventPublisher, IDisposable
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        TypeInfoResolver = new IntegrationEventPolymorphicTypeResolver()
    };

    private readonly IModel _channel;
    private readonly IConnection _connection;
    private readonly MessageBrokerSettings _messageBrokerSettings;

    /// <summary>
    ///     Initializes a new instance of the <see cref="IntegrationEventPublisher" /> class.
    /// </summary>
    /// <param name="messageBrokerSettingsOptions">The message broker settings options.</param>
    public IntegrationEventPublisher(IOptions<MessageBrokerSettings> messageBrokerSettingsOptions)
    {
        _messageBrokerSettings = messageBrokerSettingsOptions.Value;

        var connectionFactory = new ConnectionFactory
        {
            HostName = _messageBrokerSettings.Hostname,
            Port = _messageBrokerSettings.Port,
            UserName = _messageBrokerSettings.Username,
            Password = _messageBrokerSettings.Password
        };

        _connection = connectionFactory.CreateConnection();

        _channel = _connection.CreateModel();

        _channel.QueueDeclare(
            _messageBrokerSettings.QueueName,
            true,
            false,
            false);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _connection.Dispose();
        _channel.Dispose();
    }

    /// <inheritdoc />
    public Task PublishAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Serialize(integrationEvent, JsonSerializerOptions);
        var body = Encoding.UTF8.GetBytes(payload);

        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;

        _channel.BasicPublish(
            string.Empty,
            _messageBrokerSettings.QueueName,
            properties,
            body);

        return Task.CompletedTask;
    }
}
