using System.Net.Mime;
using Examples;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;


namespace OffndAt.Services.Api.Endpoints.Extensions;/// <summary>
///     Contains extension methods for OpenAPI transformers configuration.
/// </summary>
internal static class OpenApiTransformersExtensions
{
    /// <summary>
    ///     Update the title and version for all OpenAPI documents.
    /// </summary>
    /// <param name="options">The OpenAPI options.</param>
    /// <returns>The configured options.</returns>
    public static OpenApiOptions UseDocumentTitleAndVersion(this OpenApiOptions options) =>
        options.AddDocumentTransformer(
            (document, context, _) =>
            {
                document.Info.Title = $"offnd.at API | {context.DocumentName}";
                document.Info.Contact = new OpenApiContact { Url = new Uri("https://github.com/ghawliczek") };

                document.Tags = [.. document.Tags.OrderBy(tag => tag.Name)];

                return Task.CompletedTask;
            });

    /// <summary>
    ///     Registers problem details response examples with the OpenAPI options.
    /// </summary>
    /// <param name="options">The OpenAPI options.</param>
    /// <returns>The configured options.</returns>
    public static OpenApiOptions UseProblemDetailsExampleResponses(this OpenApiOptions options) =>
        options.AddOperationTransformer(
            (operation, _, _) =>
            {
                var badRequestResponse =
                    operation.Responses.FirstOrDefault(
                        response => int.TryParse(response.Key, out var statusCode) && statusCode == StatusCodes.Status400BadRequest);

                var notFoundResponse =
                    operation.Responses.FirstOrDefault(
                        response => int.TryParse(response.Key, out var statusCode) && statusCode == StatusCodes.Status404NotFound);

                var internalServerErrorResponse =
                    operation.Responses.FirstOrDefault(
                        response => int.TryParse(response.Key, out var statusCode) &&
                                    statusCode == StatusCodes.Status500InternalServerError);

                badRequestResponse.Value?.Content[MediaTypeNames.Application.ProblemJson].Example = ProblemResponseExamples.BadRequestExampleResponse;

                notFoundResponse.Value?.Content[MediaTypeNames.Application.ProblemJson].Example = ProblemResponseExamples.NotFoundExampleResponse;

                internalServerErrorResponse.Value?.Content[MediaTypeNames.Application.ProblemJson].Example = ProblemResponseExamples.InternalServerErrorExampleResponse;

                return Task.CompletedTask;
            });

    /// <summary>
    ///     Registers the JWT Bearer authentication scheme with the OpenAPI options.
    /// </summary>
    /// <param name="options">The OpenAPI options.</param>
    /// <returns>The configured options.</returns>
    public static OpenApiOptions UseJwtBearerAuthentication(this OpenApiOptions options)
    {
        var scheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Name = JwtBearerDefaults.AuthenticationScheme,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = JwtBearerDefaults.AuthenticationScheme
            }
        };

        _ = options.AddDocumentTransformer(
            (document, _, _) =>
            {
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes.Add(JwtBearerDefaults.AuthenticationScheme, scheme);

                return Task.CompletedTask;
            });

        _ = options.AddOperationTransformer(
            (operation, context, _) =>
            {
                if (context.Description.ActionDescriptor.EndpointMetadata.OfType<IAuthorizeData>().Any())
                {
                    operation.Security = [new OpenApiSecurityRequirement { [scheme] = [] }];
                }

                return Task.CompletedTask;
            });

        return options;
    }

    /// <summary>
    ///     Registers request and response examples with the OpenAPI options.
    /// </summary>
    /// <param name="options">The OpenAPI options.</param>
    /// <returns>The configured options.</returns>
    public static OpenApiOptions UseRequestAndResponseExamples(this OpenApiOptions options)
    {
        _ = options.AddSchemaTransformer(
            (schema, context, _) =>
            {
                var type = typeof(IOpenApiExample<>).MakeGenericType(context.JsonTypeInfo.Type);

                var service = context.ApplicationServices.GetService(type);
                if (service is IOpenApiExample example)
                {
                    schema.Example = example.GetExample();
                }

                return Task.CompletedTask;
            });

        return options;
    }
}
