using System.Text;
using Npgsql;
using OffndAt.Application.Abstractions.Data;
using OffndAt.Application.Abstractions.Links;
using OffndAt.Application.Links.Models;
using OffndAt.Domain.ValueObjects.Identifiers;

namespace OffndAt.Persistence.Repositories;

/// <summary>
///     Represents the repository for link visit summaries.
/// </summary>
/// <param name="dbContext">The database context.</param>
internal sealed class LinkVisitSummaryRepository(IDbContext dbContext) : ILinkVisitSummaryRepository
{
    /// <inheritdoc />
    public async Task UpsertTotalVisitsForManyAsync(IReadOnlyCollection<(LinkId, long)> updates, CancellationToken cancellationToken)
    {
        var query = new StringBuilder();
        var parameters = new List<NpgsqlParameter>();

        var values = new List<string>();
        var index = 0;

        foreach (var (linkId, visitCount) in updates)
        {
            var linkIdParam = $"@LinkId{index}";
            var visitCountParam = $"@VisitCount{index}";

            values.Add($"({linkIdParam}, {visitCountParam})");

            parameters.Add(new NpgsqlParameter(linkIdParam, linkId.Value));
            parameters.Add(new NpgsqlParameter(visitCountParam, visitCount));

            index++;
        }

        query.AppendLine($"INSERT INTO \"{nameof(LinkVisitSummary)}\" (");
        query.AppendLine($"    \"{nameof(LinkVisitSummary.LinkId)}\",");
        query.AppendLine($"    \"{nameof(LinkVisitSummary.TotalVisits)}\"");
        query.AppendLine(") VALUES");

        for (var i = 0; i < values.Count; i++)
        {
            var comma = i < values.Count - 1 ? "," : "";
            query.AppendLine($"    {values[i]}{comma}");
        }

        query.AppendLine($"ON CONFLICT (\"{nameof(LinkVisitSummary.LinkId)}\") DO UPDATE");
        query.AppendLine("SET");
        query.AppendLine(
            $"    \"{nameof(LinkVisitSummary.TotalVisits)}\" = \"{nameof(LinkVisitSummary)}\".\"{nameof(LinkVisitSummary.TotalVisits)}\" + EXCLUDED.\"{nameof(LinkVisitSummary.TotalVisits)}\";");

        await dbContext.ExecuteSqlAsync(query.ToString(), parameters, cancellationToken);
    }
}
