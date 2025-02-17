namespace OffndAt.Services.Api.FunctionalTests.Abstractions;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Persistence.Data;
using Testcontainers.PostgreSql;

[TestFixture]
internal abstract class BaseFunctionalTest
{
    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        PostgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase($"functional_tests_{GetType().Name}_{Guid.NewGuid()}")
            .WithUsername("integration")
            .WithPassword("integration")
            .Build();

        await PostgresContainer.StartAsync();

        // TODO: need to replace broker settings here as well,
        // TODO: currently throws exception due to rabbit connection problem
        ApplicationFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(
                builder => builder.ConfigureTestServices(
                    services =>
                    {
                        services.RemoveAll<DbContextOptions<OffndAtDbContext>>();

                        services.AddDbContext<OffndAtDbContext>(options => options.UseNpgsql(PostgresContainer.GetConnectionString()));
                    }));

        HttpClient = ApplicationFactory.CreateClient();

        SetupVerifier();
    }

    [OneTimeTearDown]
    public async Task OneTimeTeardown()
    {
        HttpClient.Dispose();
        await ApplicationFactory.DisposeAsync();
        await PostgresContainer.StopAsync();
        await PostgresContainer.DisposeAsync();
    }

    protected HttpClient HttpClient { get; private set; }

    private PostgreSqlContainer PostgresContainer { get; set; }

    private WebApplicationFactory<Program> ApplicationFactory { get; set; }

    private static void SetupVerifier() => VerifierSettings.ScrubInlineGuids();
}
