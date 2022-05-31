namespace BloonsTD6.Mod.MultiUser.SigScan;

internal static class ArraySpanBackCompatExtensions
{
    public static T[] Slice<T>(this T[] array, int start, int length)
    {
        var newArray = new T[length];
        Array.Copy(array, start, newArray, 0, length);
        return newArray;
    }
    public static T[] Slice<T>(this T[] array, int start) => array.Slice(start, array.Length - start);
}