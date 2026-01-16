using System.Runtime.CompilerServices;

namespace OffndAt.Services.Api.FunctionalTests.Core;

internal sealed class VerifyGlobalSettings
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifierSettings.ScrubInlineGuids();
        VerifierSettings.DontIgnoreEmptyCollections();
        VerifyHttp.Initialize();
        Recording.Start();
    }
}
