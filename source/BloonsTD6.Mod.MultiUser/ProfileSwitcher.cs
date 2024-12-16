using System.Runtime.InteropServices;

using BloonsTD6.Mod.MultiUser.Utilities;

using HarmonyLib;

using Il2CppInterop.Runtime;

using Il2CppNewtonsoft.Json;
using Il2CppNinjaKiwi.LiNK.Authentication;
using Il2CppNinjaKiwi.LiNK.Client;
using Il2CppNinjaKiwi.LiNK.Client.Files;

using Il2CppSystem.Runtime.Remoting;
using Il2CppInteropUtils = Il2CppInterop.Common.Il2CppInteropUtils;

namespace BloonsTD6.Mod.MultiUser;

internal static class ProfileSwitcher {
    /// <summary>
    /// Name of the profile to use for this application.
    /// </summary>
    public static string ProfileName { get; private set; } = "";

    /// <summary>
    /// Name of the save to use for this application.
    /// </summary>
    public static string SaveName { get; private set; } = "";

    public const string IdentityFileName = "identity";
    public const string SaveFileName = "Profile.Save";

    //private static NativeHook<Hooks.FileStorage_LoadFn> _filestorageLoadHook = Hooks.LoadFromFileStorage.Hook(LoadFromFileStorageImpl).Activate();
    /// <summary>
    /// Initializes this class instance.
    /// </summary>
    public static void Initialize() {
        var commandline = Environment.GetCommandLineArgs();
        for (int x = 0; x < commandline.Length; x++) {
            if (commandline[x] == "--profile")
                ProfileName = SanitizeFileName(commandline[x + 1]);

            if (commandline[x] == "--save")
                SaveName = SanitizeFileName(commandline[x + 1]);
        }


    }

    [HarmonyPatch(typeof(PlayerServiceComponent), nameof(PlayerServiceComponent.Awake))]
    public static class PSC_A {
        [HarmonyPostfix]
        public static void Postfix(ref PlayerServiceComponent __instance) {
            __instance.configuration.playerSaveFileName = SaveFileName;
            __instance.LiNK.LoginToIdentity(new KongToken(){displayName = "Hawk Rick Tuah Rick", gameAuthToken = "Real Auth", userID = "HI NK!"});
        }
    }

    public static IntPtr LoadFromFileStorageImpl(Il2CppNativeHookVariable<string> path,
        Il2CppNativeHookVariable<PasswordGenerator> passwordGenerator,
        Il2CppNativeHookVariable<JsonSerializerSettings> jsonSettings,
        FileSystemStorage.Strategy saveStrategy,
        bool ignoreIfNotReadable,
        IntPtr method) {
        var filePath = path.GetObject();

        if (filePath != null) {
            if (!string.IsNullOrEmpty(ProfileName) && filePath.EndsWith(IdentityFileName)) {
                var newFileName = Path.Combine(Path.GetDirectoryName(filePath), $"{IdentityFileName}-{ProfileName}");
                path.RawValue = IL2CPP.ManagedStringToIl2Cpp(newFileName);
                MelonLogger.Msg($"Redirecting Identity file: {newFileName}");
            } else if (!string.IsNullOrEmpty(SaveName) && filePath.EndsWith(SaveFileName)) {
                var newFileName = Path.Combine(Path.GetDirectoryName(filePath), $"{SaveFileName}.{SaveName}");
                path.RawValue = IL2CPP.ManagedStringToIl2Cpp(newFileName);
                MelonLogger.Msg($"Redirecting Save file: {newFileName}");
            }
        }

        return new();
        //return m_fileStorageNativeHook.Trampoline(path, passwordGenerator, jsonSettings, saveStrategy, ignoreIfNotReadable, method);
    }

    /// <summary>
    /// Sanitizes a file name such that it can be written to a file.
    /// </summary>
    private static string SanitizeFileName(this string fileName) {
        var invalidChars = Path.GetInvalidFileNameChars();
        return new string(fileName.Where(x => !invalidChars.Contains(x)).ToArray());
    }
}