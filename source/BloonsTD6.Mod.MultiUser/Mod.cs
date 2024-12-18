using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using BloonsTD6.Mod.MultiUser;
using BTD_Mod_Helper.Extensions;
using Il2CppInterop.Common;
using MelonLoader.NativeUtils;
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
    public override void OnEarlyInitialize()
    {
        var bootConfigPath = Path.Combine(MelonEnvironment.UnityGameDataDirectory, "boot.config");

        var lines = File.ReadAllLines(bootConfigPath).ToList();
        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].StartsWith("single-instance"))
            {
                lines.RemoveAt(i);
                MelonLogger.Msg("Successfully enabled multi-instance.");
                break;
            }
        }
        File.WriteAllLines(bootConfigPath, lines);

        ProfileSwitcher.Initialize();
    }

    public override void OnLateInitializeMelon()
    {
        ProfileSwitcher.OnLateInitializeMelon();
    }
}