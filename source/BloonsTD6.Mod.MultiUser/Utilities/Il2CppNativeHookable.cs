using System.Reflection;

namespace BloonsTD6.Mod.MultiUser.Utilities;

/// <summary>
/// Class that helps natively hooking IL2CPP methods.
/// </summary>
/// <typeparam name="TDelegate">The delegate type for which to create the native hook for.</typeparam>
internal class Il2CppNativeHookable<TDelegate> where TDelegate : MulticastDelegate
{
    /// <summary>
    /// The pointer to the original native method with which this was instanced.
    /// This is the pointer to the underlying IL2CPP native function.
    /// </summary>
    public IntPtr OriginalMethodInfoPtr { get; private set; }

    /// <summary>
    /// The original method with which this class was instanced.
    /// </summary>
    public MethodBase OriginalMethod { get; private set; }

    /// <summary/>
    /// <param name="method">The method to be natively hooked.</param>

    public unsafe Il2CppNativeHookable(MethodBase method)
    {
        OriginalMethod = method;
        OriginalMethodInfoPtr = UnhollowerSupport.MethodBaseToIl2CppMethodInfoPointer(method);
    }

    /// <summary>
    /// Creates a hook for this hookable method.
    /// </summary>
    /// <param name="function">The function to execute instead of the original.</param>
    /// <returns></returns>
    public NativeHook<TDelegate> Hook(TDelegate function)
    {
        return new NativeHook<TDelegate>(OriginalMethodInfoPtr, function);
    }
}