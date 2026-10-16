using System.Collections.Generic;

namespace Blazor.Toolkit.Samples.Client.Services;

/// <summary>
/// Bundled contributor list used as a last-resort fallback when the
/// GitHub Contributors API is unreachable from the browser (CORS,
/// offline, rate-limit, etc). The shape matches the GitHub
/// <c>GET /repos/{owner}/{repo}/contributors?anon=true</c> response
/// and is sorted by <c>contributions</c> descending, matching the API
/// order.
///
/// <para>
/// How to refresh: run
/// <code>curl -H "Accept: application/json" \
///   "https://api.github.com/repos/syncfusion/blazor-toolkit/contributors?per_page=100&anon=true"</code>
/// and replace the entries in <see cref="All"/> with the new JSON
/// array. The <see cref="Contributors"/> page re-sorts and re-projects
/// the data, so the file only needs to be a verbatim copy of the
/// GitHub response (or any subset of it).
/// </para>
/// </summary>
internal static class StaticFallbackContributors
{
    public static IReadOnlyList<Dictionary<string, object?>> GetAll() => All;

    private static readonly IReadOnlyList<Dictionary<string, object?>> All = new List<Dictionary<string, object?>>
    {
        new()
        {
            ["login"] = "Indhumathy-Loganathan",
            ["id"] = 58062424,
            ["avatar_url"] = "https://avatars.githubusercontent.com/u/58062424?v=4",
            ["html_url"] = "https://github.com/Indhumathy-Loganathan",
            ["type"] = "User",
            ["contributions"] = 29,
        },
        new()
        {
            ["login"] = "dependabot[bot]",
            ["id"] = 49699333,
            ["avatar_url"] = "https://avatars.githubusercontent.com/in/29110?v=4",
            ["html_url"] = "https://github.com/apps/dependabot",
            ["type"] = "Bot",
            ["contributions"] = 27,
        },
        new()
        {
            ["login"] = "PrinceOliver",
            ["id"] = 31838171,
            ["avatar_url"] = "https://avatars.githubusercontent.com/u/31838171?v=4",
            ["html_url"] = "https://github.com/PrinceOliver",
            ["type"] = "User",
            ["contributions"] = 11,
        },
        new()
        {
            ["login"] = "YokeshSF4393",
            ["id"] = 154953366,
            ["avatar_url"] = "https://avatars.githubusercontent.com/u/154953366?v=4",
            ["html_url"] = "https://github.com/YokeshSF4393",
            ["type"] = "User",
            ["contributions"] = 10,
        },
        new()
        {
            ["login"] = "VigneshNatarajan27",
            ["id"] = 44961462,
            ["avatar_url"] = "https://avatars.githubusercontent.com/u/44961462?v=4",
            ["html_url"] = "https://github.com/VigneshNatarajan27",
            ["type"] = "User",
            ["contributions"] = 10,
        },
        // Anonymous contributor returned by the GitHub Contributors API
        // when ?anon=true is passed. No login / id / avatar / profile
        // URL — the page renders this row as a non-link card with an
        // initial-based placeholder avatar.
        new()
        {
            ["name"] = "vignesh.natarajan@syncfusion.com",
            ["email"] = "nssdv@1994",
            ["type"] = "Anonymous",
            ["contributions"] = 7,
        },
        new()
        {
            ["login"] = "rajendranr-5483",
            ["id"] = 17924565,
            ["avatar_url"] = "https://avatars.githubusercontent.com/u/17924565?v=4",
            ["html_url"] = "https://github.com/rajendranr-5483",
            ["type"] = "User",
            ["contributions"] = 6,
        },
        new()
        {
            ["login"] = "Keerthivasan-Ramamoorthy",
            ["id"] = 56718830,
            ["avatar_url"] = "https://avatars.githubusercontent.com/u/56718830?v=4",
            ["html_url"] = "https://github.com/Keerthivasan-Ramamoorthy",
            ["type"] = "User",
            ["contributions"] = 5,
        },
        new()
        {
            ["login"] = "Yokesh-SF4393",
            ["id"] = 155958849,
            ["avatar_url"] = "https://avatars.githubusercontent.com/u/155958849?v=4",
            ["html_url"] = "https://github.com/Yokesh-SF4393",
            ["type"] = "User",
            ["contributions"] = 5,
        },
        new()
        {
            ["login"] = "SyncfusionBuild",
            ["id"] = 89958145,
            ["avatar_url"] = "https://avatars.githubusercontent.com/u/89958145?v=4",
            ["html_url"] = "https://github.com/SyncfusionBuild",
            ["type"] = "User",
            ["contributions"] = 1,
        },
    };
}