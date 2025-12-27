using OffndAt.Application.Core.Abstractions.Messaging;
using OffndAt.Contracts.Formats;

namespace OffndAt.Application.Formats.Queries.GetSupportedFormats;

/// <summary>
///     Query for retrieving available phrase formats.
/// </summary>
public sealed class GetSupportedFormatsQuery : IQuery<GetSupportedFormatsResponse>;
