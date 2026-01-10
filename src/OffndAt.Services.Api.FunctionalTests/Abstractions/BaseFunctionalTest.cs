using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OffndAt.Infrastructure.Core.Constants;
using OffndAt.Persistence.Data;
using OffndAt.Services.Api.FunctionalTests.Core;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace OffndAt.Services.Api.FunctionalTests.Abstractions;

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
            .WithPortBinding(5673, 5672)
            .WithUsername("guest")
            .WithPassword("guest")
            .Build();

        await PostgresContainer.StartAsync();
        await RabbitContainer.StartAsync();

        ApplicationFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder
                .UseEnvironment(EnvironmentNames.Testing)
                .ConfigureTestServices(services =>
                {
                    services.RemoveAll<DbContextOptions<OffndAtDbContext>>();

                    services.AddDbContext<OffndAtDbContext>(options => options.UseNpgsql(PostgresContainer.GetConnectionString()));
                }));

        HttpClient = ApplicationFactory.CreateClient();
        HttpClient.DefaultRequestHeaders.Add(HeaderNames.ApiKey, "test-api-key");

        HttpClientWithoutRedirects = ApplicationFactory.CreateDefaultClient(new NoRedirectHandler());
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
