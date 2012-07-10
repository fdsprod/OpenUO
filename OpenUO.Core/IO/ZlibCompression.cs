#region License Header
/***************************************************************************
 *   Copyright (c) 2011 OpenUO Software Team.
 *   All Right Reserved.
 *
 *   $Id: $:
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 ***************************************************************************/
 #endregion

using System;
using System.Runtime.InteropServices;

namespace OpenUO.Core.IO
{
    public static class ZlibCompression
    {
        public static readonly ICompressor Compressor;

        static ZlibCompression()
        {
            if (IntPtr.Size == 8)
                Compressor = new Compressor64();
            else
                Compressor = new Compressor32();
        }

        public static ZLibError Pack(byte[] dest, ref int destLength, byte[] source, int sourceLength)
        {
            return Compressor.Compress(dest, ref destLength, source, sourceLength);
        }

        public static ZLibError Pack(byte[] dest, ref int destLength, byte[] source, int sourceLength, ZLibQuality quality)
        {
            return Compressor.Compress(dest, ref destLength, source, sourceLength, quality);
        }

        public static ZLibError Unpack(byte[] dest, ref int destLength, byte[] source, int sourceLength)
        {
            return Compressor.Decompress(dest, ref destLength, source, sourceLength);
        }
    }

    public interface ICompressor
    {
        string Version { get; }

        ZLibError Compress(byte[] dest, ref int destLength, byte[] source, int sourceLength);
        ZLibError Compress(byte[] dest, ref int destLength, byte[] source, int sourceLength, ZLibQuality quality);

        ZLibError Decompress(byte[] dest, ref int destLength, byte[] source, int sourceLength);
    }

    public sealed class Compressor32 : ICompressor
    {
        [DllImport("zlib32", CallingConvention = CallingConvention.Cdecl)]
        private static extern string zlibVersion();

        [DllImport("zlib32", CallingConvention = CallingConvention.Cdecl)]
        private static extern ZLibError compress(byte[] dest, ref int destLength, byte[] source, int sourceLength);

        [DllImport("zlib32", CallingConvention = CallingConvention.Cdecl)]
        private static extern ZLibError compress2(byte[] dest, ref int destLength, byte[] source, int sourceLength, ZLibQuality quality);

        [DllImport("zlib32", CallingConvention = CallingConvention.Cdecl)]
        private static extern ZLibError uncompress(byte[] dest, ref int destLen, byte[] source, int sourceLen);
        
        public string Version
        {
            get { return zlibVersion(); }
        }

        public ZLibError Compress(byte[] dest, ref int destLength, byte[] source, int sourceLength)
        {
            return compress(dest, ref destLength, source, sourceLength);
        }

        public ZLibError Compress(byte[] dest, ref int destLength, byte[] source, int sourceLength, ZLibQuality quality)
        {
            return compress2(dest, ref destLength, source, sourceLength, quality);
        }

        public ZLibError Decompress(byte[] dest, ref int destLength, byte[] source, int sourceLength)
        {
            return uncompress(dest, ref destLength, source, sourceLength);
        }
    }

    public sealed class Compressor64 : ICompressor
    {
        [DllImport("zlib64", CallingConvention = CallingConvention.StdCall)]
        private static extern string zlibVersion();

        [DllImport("zlib64", CallingConvention = CallingConvention.StdCall)]
        private static extern ZLibError compress(byte[] dest, ref int destLength, byte[] source, int sourceLength);

        [DllImport("zlib64", CallingConvention = CallingConvention.StdCall)]
        private static extern ZLibError compress2(byte[] dest, ref int destLength, byte[] source, int sourceLength, ZLibQuality quality);

        [DllImport("zlib64", CallingConvention = CallingConvention.StdCall)]
        private static extern ZLibError uncompress(byte[] dest, ref int destLen, byte[] source, int sourceLen);
        
        public string Version
        {
            get { return zlibVersion(); }
        }

        public ZLibError Compress(byte[] dest, ref int destLength, byte[] source, int sourceLength)
        {
            return compress(dest, ref destLength, source, sourceLength);
        }

        public ZLibError Compress(byte[] dest, ref int destLength, byte[] source, int sourceLength, ZLibQuality quality)
        {
            return compress2(dest, ref destLength, source, sourceLength, quality);
        }

        public ZLibError Decompress(byte[] dest, ref int destLength, byte[] source, int sourceLength)
        {
            return uncompress(dest, ref destLength, source, sourceLength);
        }
    }

    public enum ZLibError : int
    {
        VersionError = -6,
        BufferError = -5,
        MemoryError = -4,
        DataError = -3,
        StreamError = -2,
        FileError = -1,
        Okay = 0,
        StreamEnd = 1,
        NeedDictionary = 2
    }

    public enum ZLibQuality : int
    {
        Default = -1,
        None = 0,
        Speed = 1,
        Size = 9
    }
}
