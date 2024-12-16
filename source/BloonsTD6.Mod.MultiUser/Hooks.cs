using System.Runtime.InteropServices;
using BloonsTD6.Mod.MultiUser.Utilities;
using Il2CppNewtonsoft.Json;
using Il2CppNinjaKiwi.LiNK.Client.Files;
using Il2CppSystem.Runtime.Remoting;
using AccessTools = HarmonyLib.AccessTools;

// ReSharper disable InconsistentNaming

namespace BloonsTD6.Mod.MultiUser;

#region Native Hooks
internal class Hooks
{
    /// <summary>
    /// Uses IntPtr to ignore the generic type! Beware!!
    /// </summary>
    public static Il2CppNativeHookable<FileStorage_LoadFn> LoadFromFileStorage = new (
        AccessTools.Method(typeof(FileStorage), nameof(FileStorage.Load), [
            typeof(string),
            typeof(PasswordGenerator),
            typeof(JsonSerializerSettings),
            typeof(FileSystemStorage.Strategy),
            typeof(bool)
        ], [typeof(Identity)]));

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr FileStorage_LoadFn(Il2CppNativeHookVariable<string> path,
        Il2CppNativeHookVariable<PasswordGenerator> passwordGenerator,
        Il2CppNativeHookVariable<JsonSerializerSettings> jsonSettings,
        FileSystemStorage.Strategy saveStrategy,
        bool ignoreIfNotReadable,
        IntPtr genericMethod);
}
#endregion