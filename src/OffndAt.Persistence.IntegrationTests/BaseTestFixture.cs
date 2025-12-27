using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OffndAt.Persistence.Data;
using Testcontainers.PostgreSql;

namespace OffndAt.Persistence.IntegrationTests;

internal class BaseTestFixture : IDisposable
{
    protected OffndAtDbContext DbContext { get; private set; } = null!;
    private PostgreSqlContainer? PostgresContainer { get; set; }

    public void Dispose() => DbContext.Dispose();

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        PostgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
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
        if (PostgresContainer is not null)
        {
            await PostgresContainer.StopAsync();
        }
    }

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
