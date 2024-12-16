using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using BloonsTD6.Mod.MultiUser;
using BTD_Mod_Helper.Extensions;
using MelonLoader.Utils;

[assembly: MelonInfo(typeof(Mod), "Multi User", "1.0.0", "Sewer56")]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace BloonsTD6.Mod.MultiUser;

/// <summary>
/// This class contains all code specific to BTD6.
/// As such, it is not testable without going ingame.
/// </summary>
[ExcludeFromCodeCoverage] // game specific code
public class Mod : BloonsTD6Mod
{
    public override void OnApplicationStart()
    {
        ProfileSwitcher.Initialize();

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            MelonLogger.Msg("OS is not Windows. Skipping patching UnityPlayer. You'll only be able to run 1 game copy at once.");
            return;
        }

        var versionPath = Path.Combine(this.GetModSettingsDir(true), "multiuser-version.txt");
        UnityPlayerPatcher.PatchIfNecessary(versionPath, MelonEnvironment.GameRootDirectory);
    }
}