using OffndAt.Application.Abstractions.Messaging;
using OffndAt.Contracts.Formats.Responses;

namespace OffndAt.Application.Formats.Queries.GetSupportedFormats;

/// <summary>
///     Represents the query used for getting supported formats.
/// </summary>
public sealed class GetSupportedFormatsQuery : IQuery<GetSupportedFormatsResponse>;
