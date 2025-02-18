namespace OffndAt.Services.Api.FunctionalTests.Core;

internal sealed class NoRedirectHandler : DelegatingHandler
{
    public NoRedirectHandler(HttpMessageHandler innerHandler) =>
        InnerHandler = innerHandler ?? throw new ArgumentNullException(nameof(innerHandler));
}
