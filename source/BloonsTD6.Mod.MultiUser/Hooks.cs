using System.Runtime.InteropServices;
using BloonsTD6.Mod.MultiUser.Utilities;
using Il2CppNewtonsoft.Json;
using NinjaKiwi.LiNK;
using NinjaKiwi.Players;
using NinjaKiwi.Players.Files;
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
        AccessTools.Method(typeof(FileStorage), nameof(FileStorage.load), new Type[]
        {
            typeof(string),
            typeof(PasswordGenerator),
            typeof(JsonSerializerSettings),
            typeof(SaveStrategy),
            typeof(bool)
        }, new[] { typeof(Identity) }));

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr FileStorage_LoadFn(Il2CppNativeHookVariable<string> path,
        Il2CppNativeHookVariable<PasswordGenerator> passwordGenerator,
        Il2CppNativeHookVariable<JsonSerializerSettings> jsonSettings,
        SaveStrategy saveStrategy,
        bool ignoreIfNotReadable,
        IntPtr genericMethod);
}
#endregion