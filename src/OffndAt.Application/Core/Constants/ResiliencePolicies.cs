namespace OffndAt.Application.Core.Constants;

/// <summary>
///     Contains the resilience policies constants.
/// </summary>
public static class ResiliencePolicies
{
    /// <summary>
    ///     Returns the name of a policy used for phrase generation, that retries when the phrase is already in use.
    /// </summary>
    public const string PhraseAlreadyInUsePolicyName = "phrase-already-in-use";

    /// <summary>
    ///     Returns the name of a policy used for making requests to GitHub API.
    /// </summary>
    public const string GitHubApiPolicyName = "github-api";
}
