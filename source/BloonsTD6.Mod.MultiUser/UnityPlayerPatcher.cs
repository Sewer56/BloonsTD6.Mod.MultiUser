    using Reloaded.Memory.Sigscan;

namespace BloonsTD6.Mod.MultiUser;

public class UnityPlayerPatcher(string lastVersionFilePath, string gameFolderPath) {   
    /// <summary>
    /// File path to the game folder.
    /// </summary>
    public string GameFolderPath { get; private set; } = gameFolderPath;

    /// <summary>
    /// File path to last game version
    /// </summary>
    public string LastVersionFilePath { get; private set; } = lastVersionFilePath;

    public static string ScanPattern = "48 8B C7 4C 8D 25 FE A4 18 01 66 0F 1F 44 00 00";
    public static byte[] PatchBytes = [
        0x48, 0x8B, 0xC7, // mov rax, rdi
        0x4C, 0x8D, 0x25, 0xCE, 0xA4, 0x18, 0x01, // lea r12,[rip+0x118A4CE] # Updated address to player setting always false
        0x66, 0x0F, 0x1F, 0x44, 0x00, 0x00 // nop WORD PTR [rax+rax]
    ];

    /// <summary>
    /// Patches UnityPlayer if necessary.
    /// </summary>
    /// <param name="versionPath"></param>
    /// <param name="gamePath">File path to last game version</param>
    public static void PatchIfNecessary(string versionPath, string gamePath)
    {
        if (!NeedsPatching(versionPath))
        {
            MelonLogger.Msg("Patching UnityPlayer is not necessary.");
            return;
        }

        // Set file paths
        var unityPlayerPath = Path.Combine(gamePath, "UnityPlayer.dll");
        var unityPlayerBackupPath = Path.Combine(gamePath, "UnityPlayer.dll.bak");

        // Read File & Move File to Make Backup
        // (This keeps file handle valid and allows us to make new UnityPlayer.dll)
        var unityPlayerBytes = File.ReadAllBytes(unityPlayerPath);
        File.Delete(unityPlayerBackupPath);
        File.Move(unityPlayerPath, unityPlayerBackupPath);

        try
        {
            var scanner = new Scanner(unityPlayerBytes);
            var scan = scanner.FindPattern(ScanPattern);

            if (!scan.Found)
            {
                MelonLogger.Error("Cannot patch UnityPlayer. Either is incompatible or already patched.");
                HandleError();
                return;
            }

            MelonLogger.Msg("Pattern Found. Patching!");
            Array.Copy(PatchBytes, 0, unityPlayerBytes, scan.Offset, PatchBytes.Length);
            File.WriteAllBytes(unityPlayerPath, unityPlayerBytes);
            MelonLogger.Msg("Successfully Patched UnityPlayer.");

            // Write version on success.
            File.WriteAllText(versionPath, MelonLoader.InternalUtils.UnityInformationHandler.GameVersion);
        }
        catch (Exception e)
        {
            // In case of error, copy our backup back to original player path.
            HandleError();
            throw e;
        }

        return;

        void HandleError() => File.Copy(unityPlayerBackupPath, unityPlayerPath, true);
    }

    /// <summary>
    /// Returns true if UnityPlayer needs patching.
    /// </summary>
    private static bool NeedsPatching(string versionPath)
    {
        //if (!File.Exists(versionPath))
            return true;
        
        var lastVersion    = Version.Parse(File.ReadAllText(versionPath));
        var currentVersion = Version.Parse(MelonLoader.InternalUtils.UnityInformationHandler.GameVersion);
        return currentVersion > lastVersion;
    }
}