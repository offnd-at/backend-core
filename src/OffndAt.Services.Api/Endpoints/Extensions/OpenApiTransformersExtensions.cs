using System.Globalization;
using System.Net;
using System.Net.Mime;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using OffndAt.Infrastructure.Authentication.ApiKey;
using OffndAt.Infrastructure.Core.Constants;
using OffndAt.Services.Api.Endpoints.Examples;
using IOffndAtOpenApiExample = OffndAt.Services.Api.Endpoints.Examples.IOpenApiExample;
using IOpenApiExample = Microsoft.OpenApi.IOpenApiExample;

namespace OffndAt.Services.Api.Endpoints.Extensions;

/// <summary>
///     Contains extension methods for OpenAPI transformers configuration.
/// </summary>
internal static partial class OpenApiTransformersExtensions
{
    /// <summary>
    ///     Update the title and version for all OpenAPI documents.
    /// </summary>
    /// <param name="options">The OpenAPI options.</param>
    /// <returns>The configured options.</returns>
    public static OpenApiOptions UseDocumentTitleAndVersion(this OpenApiOptions options) =>
        options.AddDocumentTransformer((document, context, _) =>
        {
            document.Info.Title = $"offnd.at API | {context.DocumentName}";
            document.Info.Contact = new OpenApiContact
            {
                Name = "Grzegorz Hawliczek",
                Email = "ghawliczek@outlook.com",
                Url = new Uri("https://github.com/ghawliczek")
            };

            document.Tags = document.Tags is null
                ? null
                : new HashSet<OpenApiTag>(document.Tags.OrderBy(tag => tag.Name));

            return Task.CompletedTask;
        });

    /// <summary>
    ///     Registers the API key authentication scheme with the OpenAPI options.
    /// </summary>
    /// <param name="options">The OpenAPI options.</param>
    /// <returns>The configured options.</returns>
    public static OpenApiOptions UseApiKeyAuthentication(this OpenApiOptions options)
    {
        var scheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Name = HeaderNames.ApiKey,
            Scheme = ApiKeyDefaults.AuthenticationScheme
        };

        _ = options.AddDocumentTransformer((document, _, _) =>
        {
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
            document.Components.SecuritySchemes.Add(ApiKeyDefaults.AuthenticationScheme, scheme);

            return Task.CompletedTask;
        });

        _ = options.AddOperationTransformer((operation, context, _) =>
        {
            if (context.Description.ActionDescriptor.EndpointMetadata.OfType<IAuthorizeData>().Any())
            {
                operation.Security ??= [];
                operation.Security.Add(
                    new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference(ApiKeyDefaults.AuthenticationScheme)] = []
                    });
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
    public static OpenApiOptions UseRequestAndResponseExamples(this OpenApiOptions options) =>
        options.AddOperationTransformer((operation, context, _) =>
        {
            AddRequestExample(context, operation);

            AddSuccessResponseExample(context, operation);

            AddFailedResponseExamples(operation);

            return Task.CompletedTask;
        });

    /// <summary>
    ///     Adds the request example for the specified operation.
    /// </summary>
    /// <param name="context">The OpenAPI operation transformer context.</param>
    /// <param name="operation">The OpenAPI operation.</param>
    private static void AddRequestExample(OpenApiOperationTransformerContext context, OpenApiOperation operation)
    {
        var requestType = context.Description.ParameterDescriptions
            .FirstOrDefault(description => description.Name.Contains("Request"))
            ?.Type;

        if (requestType is null)
        {
            return;
        }

        var type = typeof(IOpenApiExample<>).MakeGenericType(requestType);
        var service = context.ApplicationServices.GetService(type);

        if (service is not IOffndAtOpenApiExample example || operation.RequestBody?.Content is null)
        {
            return;
        }

        if (operation.RequestBody.Content.TryGetValue(MediaTypeNames.Application.Json, out var jsonContent))
        {
            jsonContent.Examples ??= new Dictionary<string, IOpenApiExample>();
            jsonContent.Examples.Add(
                requestType.Name,
                new OpenApiExample
                {
                    Value = example.GetExample()
                });
        }
        else if (operation.RequestBody.Content.TryGetValue(MediaTypeNames.Multipart.FormData, out var formDataContent))
        {
            formDataContent.Examples ??= new Dictionary<string, IOpenApiExample>();
            formDataContent.Examples.Add(
                requestType.Name,
                new OpenApiExample
                {
                    Value = example.GetExample()
                });
        }
        else if (operation.RequestBody.Content.TryGetValue(
            MediaTypeNames.Application.FormUrlEncoded,
            out var formUrlEncodedContent))
        {
            formUrlEncodedContent.Examples ??= new Dictionary<string, IOpenApiExample>();
            formUrlEncodedContent.Examples.Add(
                requestType.Name,
                new OpenApiExample
                {
                    Value = example.GetExample()
                });
        }
    }

    /// <summary>
    ///     Adds the success response example for the specified operation.
    /// </summary>
    /// <param name="context">The OpenAPI operation transformer context.</param>
    /// <param name="operation">The OpenAPI operation.</param>
    private static void AddSuccessResponseExample(
        OpenApiOperationTransformerContext context,
        OpenApiOperation operation)
    {
        var apiResponseType = context.Description.SupportedResponseTypes.FirstOrDefault(type => type.StatusCode < 400);

        if (apiResponseType?.Type is null || apiResponseType.Type == typeof(void))
        {
            return;
        }

        var type = typeof(IOpenApiExample<>).MakeGenericType(apiResponseType.Type);
        var service = context.ApplicationServices.GetService(type);

        if (service is not IOffndAtOpenApiExample example || operation.Responses is null)
        {
            return;
        }

        if (operation.Responses.TryGetValue(apiResponseType.StatusCode.ToString(CultureInfo.InvariantCulture), out var response) &&
            response.Content is not null &&
            response.Content.TryGetValue(MediaTypeNames.Application.Json, out var responseJsonContent))
        {
            responseJsonContent.Examples ??= new Dictionary<string, IOpenApiExample>();
            responseJsonContent.Examples.Add(
                StatusCodeToStringRegex().Replace(((HttpStatusCode)apiResponseType.StatusCode).ToString(), "$1 $2"),
                new OpenApiExample
                {
                    Value = example.GetExample()
                });
        }
    }

    /// <summary>
    ///     Adds the failed response examples for the specified operation.
    /// </summary>
    /// <param name="operation">The OpenAPI operation.</param>
    private static void AddFailedResponseExamples(OpenApiOperation operation)
    {
        if (operation.Responses is null)
        {
            return;
        }

        if (operation.Responses.TryGetValue(
                StatusCodes.Status400BadRequest.ToString(CultureInfo.InvariantCulture),
                out var badRequestResponse) &&
            badRequestResponse.Content is not null &&
            badRequestResponse.Content.TryGetValue(
                MediaTypeNames.Application.ProblemJson,
                out var badRequestResponseProblemJsonContent))
        {
            badRequestResponseProblemJsonContent.Examples ??= new Dictionary<string, IOpenApiExample>();
            badRequestResponseProblemJsonContent.Examples.Add(
                badRequestResponse.Description ?? "Bad Request",
                new OpenApiExample
                {
                    Value = ProblemResponseExamples.BadRequestExampleResponse
                });
        }

        if (operation.Responses.TryGetValue(
                StatusCodes.Status401Unauthorized.ToString(CultureInfo.InvariantCulture),
                out var unauthorizedResponse) &&
            unauthorizedResponse.Content is not null &&
            unauthorizedResponse.Content.TryGetValue(
                MediaTypeNames.Application.ProblemJson,
                out var unauthorizedResponseProblemJsonContent))
        {
            unauthorizedResponseProblemJsonContent.Examples ??= new Dictionary<string, IOpenApiExample>();
            unauthorizedResponseProblemJsonContent.Examples.Add(
                unauthorizedResponse.Description ?? "Unauthorized",
                new OpenApiExample
                {
                    Value = ProblemResponseExamples.UnauthorizedExampleResponse
                });
        }

        if (operation.Responses.TryGetValue(
                StatusCodes.Status404NotFound.ToString(CultureInfo.InvariantCulture),
                out var notFoundResponse) &&
            notFoundResponse.Content is not null &&
            notFoundResponse.Content.TryGetValue(
                MediaTypeNames.Application.ProblemJson,
                out var notFoundResponseProblemJsonContent))
        {
            notFoundResponseProblemJsonContent.Examples ??= new Dictionary<string, IOpenApiExample>();
            notFoundResponseProblemJsonContent.Examples.Add(
                notFoundResponse.Description ?? "Not Found",
                new OpenApiExample
                {
                    Value = ProblemResponseExamples.NotFoundExampleResponse
                });
        }

        if (operation.Responses.TryGetValue(
                StatusCodes.Status429TooManyRequests.ToString(CultureInfo.InvariantCulture),
                out var tooManyRequestsResponse) &&
            tooManyRequestsResponse.Content is not null &&
            tooManyRequestsResponse.Content.TryGetValue(
                MediaTypeNames.Application.ProblemJson,
                out var tooManyRequestsResponseProblemJsonContent))
        {
            tooManyRequestsResponseProblemJsonContent.Examples ??= new Dictionary<string, IOpenApiExample>();
            tooManyRequestsResponseProblemJsonContent.Examples.Add(
                tooManyRequestsResponse.Description ?? "Internal Server Error",
                new OpenApiExample
                {
                    Value = ProblemResponseExamples.TooManyRequestsExampleResponse
                });
        }

        if (operation.Responses.TryGetValue(
                StatusCodes.Status500InternalServerError.ToString(CultureInfo.InvariantCulture),
                out var internalServerErrorResponse) &&
            internalServerErrorResponse.Content is not null &&
            internalServerErrorResponse.Content.TryGetValue(
                MediaTypeNames.Application.ProblemJson,
                out var internalServerErrorResponseProblemJsonContent))
        {
            internalServerErrorResponseProblemJsonContent.Examples ??= new Dictionary<string, IOpenApiExample>();
            internalServerErrorResponseProblemJsonContent.Examples.Add(
                internalServerErrorResponse.Description ?? "Internal Server Error",
                new OpenApiExample
                {
                    Value = ProblemResponseExamples.InternalServerErrorExampleResponse
                });
        }
    }

    [GeneratedRegex("([a-z0-9])([A-Z])")]
    private static partial Regex StatusCodeToStringRegex();
}
