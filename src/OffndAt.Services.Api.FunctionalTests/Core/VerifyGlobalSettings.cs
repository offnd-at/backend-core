namespace OffndAt.Services.Api.FunctionalTests.Core;

using System.Runtime.CompilerServices;

internal sealed class VerifyGlobalSettings
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifyHttp.Initialize();
        Recording.Start();
    }
}
