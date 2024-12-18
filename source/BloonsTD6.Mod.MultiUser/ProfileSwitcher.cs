using System.Runtime.InteropServices;

using HarmonyLib;

using Il2CppInterop.Runtime;

using Il2CppNewtonsoft.Json;

using Il2CppNinjaKiwi.LiNK.Client;
using Il2CppNinjaKiwi.LiNK.Client.Files;

using Il2CppSystem.Runtime.Remoting;

using MelonLoader.NativeUtils;
using MelonLoader.Utils;
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
    public static class PlayerServiceComponent_Awake {
        [HarmonyPrefix]
        public static bool Prefix(ref PlayerServiceComponent __instance) {
            if (string.IsNullOrEmpty(SaveName))
                return true;
            var root = Path.Combine(MelonEnvironment.GameRootDirectory, "MultiUser");
            __instance.configuration.playerSaveFileName = $"{SaveName}-{SaveFileName}";
            __instance.configuration.playerDataRootPath = root;
            return true;
        }
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr LoadStorageDelegate(
        IntPtr storage, //IStorage
        IntPtr passwordGenerator, //PasswordGenerator
        IntPtr jsonSettings, //JsonSerializerSettings
        IntPtr ignoreIfNotReadable, //bool
        IntPtr methodInfo
    );

    private static NativeHook<LoadStorageDelegate> _hook;
    private static LoadStorageDelegate _patchDelegate;

    public static IntPtr LoadStorage(IntPtr storage, //IStorage
        IntPtr passwordGenerator, //PasswordGenerator
        IntPtr jsonSettings, //JsonSerializerSettings
        IntPtr ignoreIfNotReadable, //bool
        IntPtr methodInfo) {
        var filePath = IL2CPP.PointerToValueGeneric<IStorage>(storage, false, false);

        if (filePath == null || string.IsNullOrEmpty(ProfileName) || !filePath.Name.EndsWith(IdentityFileName))
            goto ending;
        
        var newFileName = Path.Combine(MelonEnvironment.GameRootDirectory, "MultiUser", $"{IdentityFileName}-{ProfileName}");

        storage = FileSystemStorage.CreateStorage(newFileName, FileSystemStorage.Strategy.Basic).Pointer;
        MelonLogger.Msg($"Redirecting Identity file: {newFileName}");

        ending:

        return _hook.Trampoline.Invoke(storage, passwordGenerator, jsonSettings, ignoreIfNotReadable, methodInfo);
    }

    public static unsafe void OnLateInitializeMelon() {
        // Getting the IntPtr for our target method with GetIl2CppMethodInfoPointerFieldForGeneratedMethod
        var method = AccessTools.Method(typeof(FileStorage), nameof(FileStorage.LoadStorage), [
            typeof(IStorage),
            typeof(PasswordGenerator),
            typeof(JsonSerializerSettings),
            typeof(bool)
        ], [typeof(Identity)]);

        if (method == null) {
            MelonLogger.Error("Failed to find method");
            foreach (var declaredMethod in AccessTools.GetDeclaredMethods(typeof(FileStorage))) {
                MelonLogger.Msg(declaredMethod.Name);
                foreach (var VARIABLE in declaredMethod.GetParameters()) {
                    MelonLogger.Msg(VARIABLE.Name + " : " + VARIABLE.ParameterType);
                }
            }

            return;
        }

        IntPtr originalMethod = *(IntPtr*)(IntPtr)Il2CppInteropUtils
            .GetIl2CppMethodInfoPointerFieldForGeneratedMethod(method).GetValue(null);

        _patchDelegate = LoadStorage;

        IntPtr delegatePointer = Marshal.GetFunctionPointerForDelegate(_patchDelegate);

        NativeHook<LoadStorageDelegate> hook = new NativeHook<LoadStorageDelegate>(originalMethod, delegatePointer);
        hook.Attach();
        _hook = hook;
    }


    /// <summary>
    /// Sanitizes a file name such that it can be written to a file.
    /// </summary>
    private static string SanitizeFileName(this string fileName) {
        var invalidChars = Path.GetInvalidFileNameChars();
        return new string(fileName.Where(x => !invalidChars.Contains(x)).ToArray());
    }
}