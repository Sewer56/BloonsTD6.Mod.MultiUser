﻿using System.Globalization;

namespace BloonsTD6.Mod.MultiUser.SigScan;

/// <summary>
/// [Internal and Test Use]
/// Represents the pattern to be searched by the scanner.
/// </summary>
public ref struct SimplePatternScanData
{
    private static char[] _maskIgnore = { '?', '?' };
    private static List<byte> _bytes = new List<byte>(1024);
    private static List<byte> _maskBuilder = new List<byte>(1024);
    private static object _buildLock = new object();

    /// <summary>
    /// The pattern of bytes to check for.
    /// </summary>
    public byte[] Bytes;

    /// <summary>
    /// The mask string to compare against. `x` represents check while `?` ignores.
    /// Each `x` and `?` represent 1 byte.
    /// </summary>
    public byte[] Mask;

    /// <summary>
    /// Creates a new pattern scan target given a string representation of a pattern.
    /// </summary>
    /// <param name="stringPattern">
    ///     The pattern to look for inside the given region.
    ///     Example: "11 22 33 ?? 55".
    ///     Key: ?? represents a byte that should be ignored, anything else if a hex byte. i.e. 11 represents 0x11, 1F represents 0x1F.
    /// </param>
    public SimplePatternScanData(string stringPattern)
    {
        var enumerator = new SpanSplitEnumerator<char>(stringPattern.ToCharArray(), ' ');
        var questionMarkFlag = _maskIgnore;

        lock (_buildLock)
        {
            _maskBuilder.Clear();
            _bytes.Clear();

            while (enumerator.MoveNext())
            {
                if (enumerator.Current.SequenceEqual(questionMarkFlag))
                {
                    _maskBuilder.Add(0x0);
                }
                else
                {
                    _bytes.Add(byte.Parse(new string(enumerator.Current), NumberStyles.HexNumber));
                    _maskBuilder.Add(0x1);
                }

            }

            Mask = _maskBuilder.ToArray();
            Bytes = _bytes.ToArray();
        }
    }
}