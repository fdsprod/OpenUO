#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   AnimationImageSourceStorageAdapter.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

#region Usings

using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenUO.Core;
using OpenUO.Ultima.Adapters;

#endregion

namespace OpenUO.Ultima.PresentationOpenUO.Core.Adapters
{
    internal class AnimationImageSourceStorageAdapter : StorageAdapterBase, IAnimationStorageAdapter<ImageSource>
    {
        private const int DoubleXor = (0x200 << 22) | (0x200 << 12);
        private BodyConverter _bodyConverter;
        private BodyTable _bodyTable;
        private FileIndexBase[] _fileIndices;
        private Hues _hues;
        private int[] _table;

        public override int Length
        {
            get
            {
                if(!IsInitialized)
                {
                    Initialize();
                }

                return 0;
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            var install = Install;

            _fileIndices = new[]
                           {
                               install.CreateFileIndex("anim.idx", "anim.mul"),
                               install.CreateFileIndex("anim2.idx", "anim2.mul"),
                               install.CreateFileIndex("anim3.idx", "anim3.mul"),
                               install.CreateFileIndex("anim4.idx", "anim4.mul"),
                               install.CreateFileIndex("anim5.idx", "anim5.mul")
                           };

            _bodyTable = new BodyTable(install.GetPath("body.def"));
            _bodyConverter = new BodyConverter(install.GetPath("bodyconv.def"));
            _hues = new Hues(install);
        }

        public unsafe Frame<ImageSource>[] GetAnimation(int body, int action, int direction, int hue, bool preserveHue)
        {
            if(preserveHue)
            {
                Translate(ref body);
            }
            else
            {
                Translate(ref body, ref hue);
            }

            var fileType = _bodyConverter.Convert(ref body);
            var fileIndex = _fileIndices[fileType - 1];

            int index;

            switch(fileType)
            {
                default:
                {
                    if(body < 200)
                    {
                        index = body * 110;
                    }
                    else if(body < 400)
                    {
                        index = 22000 + ((body - 200) * 65);
                    }
                    else
                    {
                        index = 35000 + ((body - 400) * 175);
                    }

                    break;
                }
                case 2:
                {
                    if(body < 200)
                    {
                        index = body * 110;
                    }
                    else
                    {
                        index = 22000 + ((body - 200) * 65);
                    }

                    break;
                }
                case 3:
                {
                    if(body < 300)
                    {
                        index = body * 65;
                    }
                    else if(body < 400)
                    {
                        index = 33000 + ((body - 300) * 110);
                    }
                    else
                    {
                        index = 35000 + ((body - 400) * 175);
                    }

                    break;
                }
                case 4:
                {
                    if(body < 200)
                    {
                        index = body * 110;
                    }
                    else if(body < 400)
                    {
                        index = 22000 + ((body - 200) * 65);
                    }
                    else
                    {
                        index = 35000 + ((body - 400) * 175);
                    }

                    break;
                }
                case 5:
                {
                    if(body < 200 && body != 34) // looks strange, though it works.
                    {
                        index = body * 110;
                    }
                    else
                    {
                        index = 35000 + ((body - 400) * 65);
                    }

                    break;
                }
            }

            if((index + (action * 5)) > int.MaxValue)
            {
                throw new ArithmeticException();
            }

            index += action * 5;

            if(direction <= 4)
            {
                index += direction;
            }
            else
            {
                index += direction - (direction - 4) * 2;
            }

            int length, extra;

            using(var stream = fileIndex.Seek(index, out length, out extra))
            {
                if(stream == null)
                {
                    return null;
                }

                var flip = (direction > 4);

                using(var bin = new BinaryReader(stream))
                {
                    var palette = new ushort[0x100];

                    for(var i = 0; i < 0x100; ++i)
                    {
                        palette[i] = (ushort)(bin.ReadUInt16() ^ 0x8000);
                    }

                    var start = (int)bin.BaseStream.Position;
                    var frameCount = bin.ReadInt32();

                    var lookups = new int[frameCount];

                    for(var i = 0; i < frameCount; ++i)
                    {
                        lookups[i] = start + bin.ReadInt32();
                    }

                    var onlyHueGrayPixels = ((hue & 0x8000) == 0);

                    hue = (hue & 0x3FFF) - 1;

                    Hue hueObject = null;

                    if(hue >= 0 && hue < _hues.Table.Length)
                    {
                        hueObject = _hues.Table[hue];
                    }

                    var frames = new Frame<ImageSource>[frameCount];

                    for(var i = 0; i < frameCount; ++i)
                    {
                        bin.BaseStream.Seek(lookups[i], SeekOrigin.Begin);

                        int xCenter = bin.ReadInt16();
                        int yCenter = bin.ReadInt16();

                        int width = bin.ReadUInt16();
                        int height = bin.ReadUInt16();

                        var bmp = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr555, null);
                        bmp.Lock();

                        var line = (ushort*)bmp.BackBuffer;
                        var delta = bmp.BackBufferStride >> 1;

                        int header;

                        var xBase = xCenter - 0x200;
                        var yBase = (yCenter + height) - 0x200;

                        if(!flip)
                        {
                            line += xBase;
                            line += (yBase * delta);

                            while((header = bin.ReadInt32()) != 0x7FFF7FFF)
                            {
                                header ^= DoubleXor;

                                var cur = line + ((((header >> 12) & 0x3FF) * delta) + ((header >> 22) & 0x3FF));
                                var end = cur + (header & 0xFFF);

                                while(cur < end)
                                {
                                    *cur++ = palette[bin.ReadByte()];
                                }
                            }
                        }
                        else
                        {
                            line -= xBase - width + 1;
                            line += (yBase * delta);

                            while((header = bin.ReadInt32()) != 0x7FFF7FFF)
                            {
                                header ^= DoubleXor;

                                var cur = line + ((((header >> 12) & 0x3FF) * delta) - ((header >> 22) & 0x3FF));
                                var end = cur - (header & 0xFFF);

                                while(cur > end)
                                {
                                    *cur-- = palette[bin.ReadByte()];
                                }
                            }

                            xCenter = width - xCenter;
                        }

                        bmp.AddDirtyRect(new Int32Rect(0, 0, width, height));
                        bmp.Unlock();

                        if(hueObject != null)
                        {
                            ApplyHue(bmp, hueObject, onlyHueGrayPixels);
                        }

                        frames[i] = new Frame<ImageSource>(xCenter, yCenter, bmp);
                    }

                    return frames;
                }
            }
        }

        public Task<Frame<ImageSource>[]> GetAnimationAsync(int body, int action, int direction, int hue, bool preserveHue)
        {
            return Task.FromResult(GetAnimation(body, action, direction, hue, preserveHue));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            foreach(var fileIndex in _fileIndices)
            {
                fileIndex.Close();
            }

            _fileIndices = null;
            _bodyConverter = null;
            _table = null;
            _bodyTable = null;
            _hues = null;
        }

        public unsafe void ApplyHue(ImageSource bmp, Hue hue, bool onlyHueGrayPixels)
        {
            var writeable = bmp as WriteableBitmap;

            Guard.RequireIsNotNull(writeable, "writeable");

            writeable.Lock();

            var stride = writeable.BackBufferStride >> 1;
            var width = (int)bmp.Width;
            var height = (int)bmp.Height;
            var delta = stride - width;

            var pBuffer = (ushort*)writeable.BackBuffer;
            var pLineEnd = pBuffer + width;
            var pImageEnd = pBuffer + (stride * height);

            ushort* pColors = stackalloc ushort[0x40];
            var hueColors = hue.Colors;

            fixed(ushort* pOriginal = hueColors)
            {
                var pSource = pOriginal;
                var pDest = pColors;
                var pEnd = pDest + 32;

                while(pDest < pEnd)
                {
                    *pDest++ = 0;
                }

                pEnd += 32;

                while(pDest < pEnd)
                {
                    *pDest++ = *pSource++;
                }
            }

            if(onlyHueGrayPixels)
            {
                while(pBuffer < pImageEnd)
                {
                    while(pBuffer < pLineEnd)
                    {
                        int c = *pBuffer;
                        var r = (c >> 10) & 0x1F;
                        var g = (c >> 5) & 0x1F;
                        var b = c & 0x1F;

                        if(r == g && r == b)
                        {
                            *pBuffer++ = pColors[c >> 10];
                        }
                        else
                        {
                            ++pBuffer;
                        }
                    }

                    pBuffer += delta;
                    pLineEnd += stride;
                }
            }
            else
            {
                while(pBuffer < pImageEnd)
                {
                    while(pBuffer < pLineEnd)
                    {
                        *pBuffer = pColors[(*pBuffer) >> 10];
                        ++pBuffer;
                    }

                    pBuffer += delta;
                    pLineEnd += stride;
                }
            }

            writeable.AddDirtyRect(new Int32Rect(0, 0, width, height));
            writeable.Unlock();
        }

        public void Translate(ref int body)
        {
            if(_table == null)
            {
                InitializeTable();
            }

            if(body <= 0 || body >= _table.Length)
            {
                body = 0;
                return;
            }

            body = (_table[body] & 0x7FFF);
        }

        public void Translate(ref int body, ref int hue)
        {
            if(_table == null)
            {
                InitializeTable();
            }

            if(body <= 0 || body >= _table.Length)
            {
                body = 0;
                return;
            }

            var table = _table[body];

            if((table & (1 << 31)) != 0)
            {
                body = table & 0x7FFF;

                var vhue = (hue & 0x3FFF) - 1;

                //if (vhue < 0 || vhue >= Hues.List.Length)
                //    hue = (table >> 15) & 0xFFFF;
            }
        }

        private void InitializeTable()
        {
            var count = 400 + ((_fileIndices[0].Entries.Length - 35000) / 175);

            _table = new int[count];

            for(var i = 0; i < count; ++i)
            {
                BodyTableEntry entry;
                _bodyTable.Entries.TryGetValue(i, out entry);

                if(entry == null || _bodyConverter.Contains(i))
                {
                    _table[i] = i;
                }
                else
                {
                    _table[i] = entry.m_OldID | (1 << 31) | (((entry.m_NewHue ^ 0x8000) & 0xFFFF) << 15);
                }
            }
        }
    }
}