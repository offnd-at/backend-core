using Microsoft.EntityFrameworkCore;
using NSubstitute;
using OffndAt.Domain.Abstractions.Events;
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

        DbOptions = CreateNewContextOptions(PostgresContainer.GetConnectionString());

        await using var dbContext = CreateDbContext();
        await dbContext.Database.MigrateAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTeardown()
    {
        await PostgresContainer.StopAsync();
        await PostgresContainer.DisposeAsync();
    }

    private PostgreSqlContainer PostgresContainer { get; set; }

    protected DbContextOptions<OffndAtDbContext> DbOptions { get; private set; }

    private static DbContextOptions<OffndAtDbContext> CreateNewContextOptions(string connectionString) =>
        new DbContextOptionsBuilder<OffndAtDbContext>()
            .UseNpgsql(connectionString)
            .Options;

    protected OffndAtDbContext CreateDbContext() =>
        new(DbOptions, Substitute.For<IDomainEventPublisher>(), Substitute.For<IDomainEventCollector>());

    protected async Task ExecuteInTransactionAsync(Func<OffndAtDbContext, Task> test)
    {
        await using var context = CreateDbContext();
        await using var transaction =
            await context.Database.BeginTransactionAsync();

        await test(context);

        await transaction.RollbackAsync();
    }
}
