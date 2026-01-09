using System.Text.Json.Nodes;

namespace OffndAt.Services.Api.Endpoints.Examples;

/// <summary>
///     Contains examples of application/problem+json responses.
/// </summary>
internal static class ProblemResponseExamples
{
    /// <summary>
    ///     Gets the bad request example response.
    /// </summary>
    public static JsonObject BadRequestExampleResponse =>
        new()
        {
            ["type"] = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            ["title"] = "Bad Request",
            ["status"] = 400,
            ["detail"] = "The target URL is required.",
            ["instance"] = "POST /v1/links",
            ["errors"] = new JsonArray
            {
                new JsonObject
                {
                    ["code"] = "GenerateLink.TargetUrlIsRequired",
                    ["message"] = "The target URL is required."
                }
            },
            ["traceId"] = "00-f618608b1d382a3f158e8d8a470415cb-97d612370d05fa48-00",
            ["requestId"] = "0HNA90IP0U3OO:00000009",
            ["activityId"] = "00-146776dabba4b99694fd536a57d0b32d-b39f7cb8d29b92a8-00"
        };

    /// <summary>
    ///     Gets the unauthorized example response.
    /// </summary>
    public static JsonObject UnauthorizedExampleResponse =>
        new()
        {
            ["type"] = "https://tools.ietf.org/html/rfc9110#section-15.5.2",
            ["title"] = "Unauthorized",
            ["status"] = 401,
            ["instance"] = "POST /v1/links",
            ["errors"] = new JsonArray
            {
                new JsonObject
                {
                    ["code"] = "General.Unauthorized",
                    ["message"] = "You are not authorized to access the requested resource or perform the requested action."
                }
            },
            ["traceId"] = "00-8841cb0553bbecacd1b732e7794462fd-335133498c4dab90-01",
            ["requestId"] = "0HNCUPU0UAM7N:00000004",
            ["activityId"] = "00-8841cb0553bbecacd1b732e7794462fd-335133498c4dab90-01"
        };

    /// <summary>
    ///     Gets the not found example response.
    /// </summary>
    public static JsonObject NotFoundExampleResponse =>
        new()
        {
            ["type"] = "https://tools.ietf.org/html/rfc9110#section-15.5.5",
            ["title"] = "Not Found",
            ["status"] = 404,
            ["detail"] = "The requested resource was not found.",
            ["instance"] = "GET /v1/redirects/test-url-phrase",
            ["errors"] = new JsonArray
            {
                new JsonObject
                {
                    ["code"] = "General.NotFound",
                    ["message"] = "The requested resource was not found."
                }
            },
            ["traceId"] = "00-f618608b1d382a3f158e8d8a470415cb-97d612370d05fa48-00",
            ["requestId"] = "0HNA91JDFUJ9C:00000002",
            ["activityId"] = "00-effef528b3db3c7cb91c135ec0fffe0e-d0e9ab8a83207dcf-00"
        };

    /// <summary>
    ///     Gets the too many requests example response.
    /// </summary>
    public static JsonObject TooManyRequestsExampleResponse =>
        new()
        {
            ["title"] = "An application error occurred.",
            ["status"] = 429,
            ["instance"] = "GET /v1/redirects/sample-url",
            ["errors"] = new JsonArray
            {
                new JsonObject
                {
                    ["code"] = "General.TooManyRequests",
                    ["message"] = "Too many requests. Please try again later."
                }
            },
            ["traceId"] = "00-91d9c6dd47f04e2c6bb3d116b9d18a8c-ecdd4ff1eefef635-01",
            ["requestId"] = "0HNIFO753RU9P:00000063",
            ["activityId"] = "00-91d9c6dd47f04e2c6bb3d116b9d18a8c-ecdd4ff1eefef635-01"
        };

    /// <summary>
    ///     Gets the internal server error example response.
    /// </summary>
    public static JsonObject InternalServerErrorExampleResponse =>
        new()
        {
            ["type"] = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
            ["title"] = "An unexpected error occurred.",
            ["status"] = 500,
            ["instance"] = "POST /v1/links",
            ["errors"] = new JsonArray
            {
                new JsonObject
                {
                    ["code"] = "General.ServerError",
                    ["message"] = "The server encountered an unrecoverable error."
                }
            },
            ["traceId"] = "00-8a9e88318b1dd74bc15d638c2dfdbc0a-4d196748e9294430-00",
            ["requestId"] = "0HNA91PNHN1ND:00000003",
            ["activityId"] = "00-8a9e88318b1dd74bc15d638c2dfdbc0a-4d196748e9294430-00"
        };
}
