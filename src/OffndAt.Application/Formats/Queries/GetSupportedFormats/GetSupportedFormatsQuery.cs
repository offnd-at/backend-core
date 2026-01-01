using OffndAt.Application.Core.Abstractions.Messaging;
using OffndAt.Contracts.Formats;

namespace OffndAt.Application.Formats.Queries.GetSupportedFormats;

/// <summary>
///     Represents the query used for getting supported formats.
/// </summary>
public sealed class GetSupportedFormatsQuery : IQuery<GetSupportedFormatsResponse>;
