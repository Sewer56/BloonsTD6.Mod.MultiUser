using System.Runtime.InteropServices;

namespace BloonsTD6.Mod.MultiUser.Utilities;

/// <summary>
/// Class for creating native hooks using MelonLoader.
/// It's really, really primitive hook implementation though :/.
/// </summary>
internal unsafe class NativeHook<TDelegate> where TDelegate : MulticastDelegate
{
    /// <summary>
    /// The original function to be hooked.
    /// </summary>
    public TDelegate OriginalFunction { get; private set; } = null!;

    /// <summary>
    /// Native pointer to the original function.
    /// </summary>
    public IntPtr OriginalFunctionPtr { get; }

    /// <summary>
    /// Native pointer to the ptr of the original function.
    /// </summary>
    public IntPtr MethodInfoPtr { get; }

    /// <summary>
    /// Allows for our function to be invoked.
    /// </summary>
    public TDelegate OurFunction { get; }

    /// <summary>
    /// Native pointer to our function.
    /// </summary>
    public IntPtr OurFunctionPtr { get; }

    /// <summary>
    /// True if the hook is active.
    /// </summary>
    public bool IsActive { get; private set; }

    public NativeHook(IntPtr methodInfoPtr, TDelegate function)
    {
        MethodInfoPtr       = methodInfoPtr;
        OriginalFunctionPtr = *(IntPtr*)MethodInfoPtr;
        
        OurFunction = function;
        OurFunctionPtr = Marshal.GetFunctionPointerForDelegate(OurFunction);
    }

    /// <summary>
    /// Activates this hook.
    /// </summary>
    public NativeHook<TDelegate> Activate()
    {
        if (IsActive) 
            return this;
        
        // originalPtr is modified in NativeHookAttach, god damn it MelonDevs, for having no docs.
        var originalPtr = OriginalFunctionPtr;
        MelonUtils.NativeHookAttach((IntPtr)(&originalPtr), OurFunctionPtr);
        OriginalFunction = Marshal.GetDelegateForFunctionPointer<TDelegate>(originalPtr);

        IsActive = true;
        return this;
    }
}