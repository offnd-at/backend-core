using Microsoft.OpenApi.Any;


namespace OffndAt.Services.Api.Endpoints.Examples;/// <summary>
///     Contains examples of application/problem+json responses.
/// </summary>
internal static class ProblemResponseExamples
{
    /// <summary>
    ///     Gets the bad request example response.
    /// </summary>
    public static OpenApiObject BadRequestExampleResponse =>
        new()
        {
            ["type"] = new OpenApiString("https://tools.ietf.org/html/rfc9110#section-15.5.1"),
            ["title"] = new OpenApiString("One or more validation errors occurred."),
            ["status"] = new OpenApiInteger(400),
            ["instance"] = new OpenApiString("POST /v1/links"),
            ["errors"] = new OpenApiArray
            {
                new OpenApiObject
                {
                    ["code"] = new OpenApiString("GenerateLink.TargetUrlIsRequired"),
                    ["message"] = new OpenApiString("The target URL is required.")
                }
            },
            ["traceId"] = new OpenApiString("00-f618608b1d382a3f158e8d8a470415cb-97d612370d05fa48-00"),
            ["requestId"] = new OpenApiString("0HNA90IP0U3OO:00000009"),
            ["activityId"] = new OpenApiString("00-146776dabba4b99694fd536a57d0b32d-b39f7cb8d29b92a8-00")
        };

    /// <summary>
    ///     Gets the not found example response.
    /// </summary>
    public static OpenApiObject NotFoundExampleResponse =>
        new()
        {
            ["type"] = new OpenApiString("https://tools.ietf.org/html/rfc9110#section-15.5.5"),
            ["title"] = new OpenApiString("Not Found"),
            ["status"] = new OpenApiInteger(404),
            ["detail"] = new OpenApiString("The requested resource was not found."),
            ["instance"] = new OpenApiString("GET /v1/redirects/test-url-phrase"),
            ["errors"] = new OpenApiArray
            {
                new OpenApiObject
                {
                    ["code"] = new OpenApiString("General.NotFound"),
                    ["message"] = new OpenApiString("The requested resource was not found.")
                }
            },
            ["traceId"] = new OpenApiString("00-f618608b1d382a3f158e8d8a470415cb-97d612370d05fa48-00"),
            ["requestId"] = new OpenApiString("0HNA91JDFUJ9C:00000002"),
            ["activityId"] = new OpenApiString("00-effef528b3db3c7cb91c135ec0fffe0e-d0e9ab8a83207dcf-00")
        };

    /// <summary>
    ///     Gets the internal server error example response.
    /// </summary>
    public static OpenApiObject InternalServerErrorExampleResponse =>
        new()
        {
            ["type"] = new OpenApiString("https://tools.ietf.org/html/rfc9110#section-15.6.1"),
            ["title"] = new OpenApiString("An unexpected error occurred."),
            ["status"] = new OpenApiInteger(500),
            ["instance"] = new OpenApiString("POST /v1/links"),
            ["errors"] = new OpenApiArray
            {
                new OpenApiObject
                {
                    ["code"] = new OpenApiString("General.ServerError"),
                    ["message"] = new OpenApiString("The server encountered an unrecoverable error.")
                }
            },
            ["traceId"] = new OpenApiString("00-8a9e88318b1dd74bc15d638c2dfdbc0a-4d196748e9294430-00"),
            ["requestId"] = new OpenApiString("0HNA91PNHN1ND:00000003"),
            ["activityId"] = new OpenApiString("00-8a9e88318b1dd74bc15d638c2dfdbc0a-4d196748e9294430-00")
        };
}
