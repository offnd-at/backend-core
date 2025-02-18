namespace OffndAt.Services.Api.FunctionalTests.Abstractions;

using Core;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Persistence.Data;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

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

        RabbitContainer = new RabbitMqBuilder()
            .WithImage("rabbitmq:4")
            .WithPortBinding(5672, 5672)
            .WithUsername("guest")
            .WithPassword("guest")
            .Build();

        await PostgresContainer.StartAsync();
        await RabbitContainer.StartAsync();

        ApplicationFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(
                builder => builder.ConfigureTestServices(
                    services =>
                    {
                        services.RemoveAll<DbContextOptions<OffndAtDbContext>>();

                        services.AddDbContext<OffndAtDbContext>(options => options.UseNpgsql(PostgresContainer.GetConnectionString()));
                    }));

        HttpClient = ApplicationFactory.CreateClient();
        HttpClientWithoutRedirects = ApplicationFactory.CreateDefaultClient(
            new NoRedirectHandler(new HttpClientHandler { AllowAutoRedirect = false }));
    }

    [OneTimeTearDown]
    public async Task OneTimeTeardown()
    {
        HttpClient.Dispose();
        HttpClientWithoutRedirects.Dispose();
        await ApplicationFactory.DisposeAsync();
        await RabbitContainer.StopAsync();
        await RabbitContainer.DisposeAsync();
        await PostgresContainer.StopAsync();
        await PostgresContainer.DisposeAsync();
    }

    protected HttpClient HttpClient { get; private set; }

    protected HttpClient HttpClientWithoutRedirects { get; private set; }

    private PostgreSqlContainer PostgresContainer { get; set; }

    private RabbitMqContainer RabbitContainer { get; set; }

    private WebApplicationFactory<Program> ApplicationFactory { get; set; }
}
