namespace OffndAt.Application.Formats.Queries.GetSupportedFormats;

using Contracts.Formats;
using Core.Abstractions.Messaging;

/// <summary>
///     Query for retrieving available phrase formats.
/// </summary>
public sealed class GetSupportedFormatsQuery : IQuery<GetSupportedFormatsResponse>;
