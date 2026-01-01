using System.ComponentModel;
namespace OffndAt.Contracts.Links;


/// <summary>
///     Represents the link data transfer object.
/// </summary>
public sealed class LinkDto
{
    /// <summary>
    ///     Gets the target URL.
    /// </summary>
    [Description("The target URL to which the link will redirect.")]
    public required string TargetUrl { get; init; }

    /// <summary>
    ///     Gets the visits count.
    /// </summary>
    [Description("The number of times the link has been visited.")]
    public required int Visits { get; init; }
}
