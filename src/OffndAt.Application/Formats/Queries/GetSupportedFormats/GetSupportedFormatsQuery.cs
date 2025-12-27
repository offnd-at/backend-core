using Contracts.Formats;
using Core.Abstractions.Messaging;


namespace OffndAt.Application.Formats.Queries.GetSupportedFormats;/// <summary>
///     Query for retrieving available phrase formats.
/// </summary>
public sealed class GetSupportedFormatsQuery : IQuery<GetSupportedFormatsResponse>;
