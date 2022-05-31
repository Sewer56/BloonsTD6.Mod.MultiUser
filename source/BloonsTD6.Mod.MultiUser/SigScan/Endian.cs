using System.Runtime.InteropServices;

namespace BloonsTD6.Mod.MultiUser.SigScan;

internal static class Endian
{
    public static void Reverse<T>(this ref T value) where T : struct
    {
        int size  = Marshal.SizeOf(typeof(T));
        var bytes = new byte[size];
        var ptr   = Marshal.AllocHGlobal(size);

        try
        {
            Marshal.StructureToPtr(value, ptr, true);
            Marshal.Copy(ptr, bytes, 0, size); 
            Array.Reverse(bytes);
            Marshal.Copy(bytes, 0, ptr, bytes.Length);
            value = Marshal.PtrToStructure<T>(ptr);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }
}