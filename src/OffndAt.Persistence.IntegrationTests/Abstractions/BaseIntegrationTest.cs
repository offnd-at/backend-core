using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OffndAt.Persistence.Data;
using Testcontainers.PostgreSql;

namespace OffndAt.Persistence.IntegrationTests.Abstractions;

[TestFixture]
internal abstract class BaseIntegrationTest
{
    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        PostgresContainer = new PostgreSqlBuilder("postgres:17-alpine")
            .WithDatabase($"integration_tests_{GetType().Name}_{Guid.NewGuid()}")
            .WithUsername("integration")
            .WithPassword("integration")
            .Build();

        await PostgresContainer.StartAsync();

        var dbOptions = CreateNewContextOptions(PostgresContainer.GetConnectionString());

        DbContext = new OffndAtDbContext(dbOptions, Substitute.For<IMediator>());

        // EnsureCreatedAsync totally bypasses migrations and just creates the schema.
        // If migrations need to be automatically applied on app start, then context.Database.MigrateAsync() should be used instead.
        await DbContext.Database.EnsureCreatedAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTeardown()
    {
        await DbContext.DisposeAsync();
        await PostgresContainer.StopAsync();
        await PostgresContainer.DisposeAsync();
    }

    protected OffndAtDbContext DbContext { get; private set; }
    private PostgreSqlContainer PostgresContainer { get; set; }

    private static DbContextOptions<OffndAtDbContext> CreateNewContextOptions(string connectionString)
    {
        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkNpgsql()
            .BuildServiceProvider();

        var builder = new DbContextOptionsBuilder<OffndAtDbContext>();

        builder.UseNpgsql(connectionString)
            .UseInternalServiceProvider(serviceProvider);

        return builder.Options;
    }
}
