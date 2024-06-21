namespace OffndAt.Infrastructure.Messaging.Json;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Application.Core.Abstractions.Messaging;
using Application.Links.Events.LinkCreated;
using Application.Links.Events.LinkVisited;

/// <summary>
///     Defines the reflection-based JSON contract resolver used by System.Text.Json to work with integration events.
/// </summary>
public sealed class IntegrationEventPolymorphicTypeResolver : DefaultJsonTypeInfoResolver
{
    /// <inheritdoc />
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        var jsonTypeInfo = base.GetTypeInfo(type, options);

        var integrationEventInterface = typeof(IIntegrationEvent);

        if (jsonTypeInfo.Type == integrationEventInterface)
        {
            jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
            {
                TypeDiscriminatorPropertyName = "$eventType",
                IgnoreUnrecognizedTypeDiscriminators = true,
                UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
                DerivedTypes =
                {
                    new JsonDerivedType(typeof(LinkCreatedIntegrationEvent), "LinkCreated"),
                    new JsonDerivedType(typeof(LinkVisitedIntegrationEvent), "LinkVisited")
                }
            };
        }

        return jsonTypeInfo;
    }
}
