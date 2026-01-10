namespace OffndAt.Services.Api.FunctionalTests.Core;

internal sealed class NoRedirectHandler : DelegatingHandler
{
    public NoRedirectHandler() =>
        InnerHandler = new HttpClientHandler
        {
            AllowAutoRedirect = false
        };
}
