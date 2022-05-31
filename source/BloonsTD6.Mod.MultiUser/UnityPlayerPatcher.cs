using BloonsTD6.Mod.MultiUser.SigScan;

namespace BloonsTD6.Mod.MultiUser;

public class UnityPlayerPatcher
{   
    /// <summary>
    /// File path to the game folder.
    /// </summary>
    public string GameFolderPath { get; private set; }

    /// <summary>
    /// File path to last game version
    /// </summary>
    public string LastVersionFilePath { get; private set; }

    public static string ScanPattern = "0F 84 B5 00 00 00 40 84 F6 40 0F 94 C6";
    public static byte[] PatchBytes  = new byte[]
    {
        0x90, 0x90, 0x90, 0x90, 0x90, 0x90, // nop x 6
        0xBE, 0x01, 0x00, 0x00, 0x00,       // mov esi, 1
        0x90, 0x90                          // nop x 2
    };

    public UnityPlayerPatcher(string lastVersionFilePath, string gameFolderPath)
    {
        LastVersionFilePath = lastVersionFilePath;
        GameFolderPath = gameFolderPath;
    }

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
                MelonLogger.Msg("Cannot patch UnityPlayer. Either is incompatible or already patched.");
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

        void HandleError() => File.Copy(unityPlayerBackupPath, unityPlayerPath, true);
    }

    /// <summary>
    /// Returns true if UnityPlayer needs patching.
    /// </summary>
    private static bool NeedsPatching(string versionPath)
    {
        if (!File.Exists(versionPath))
            return true;
        
        var lastVersion    = Version.Parse(File.ReadAllText(versionPath));
        var currentVersion = Version.Parse(MelonLoader.InternalUtils.UnityInformationHandler.GameVersion);
        return currentVersion > lastVersion;
    }
}