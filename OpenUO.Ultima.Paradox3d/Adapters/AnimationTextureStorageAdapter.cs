using System;
using System.IO;
using System.Threading.Tasks;

using OpenUO.Core.Patterns;
using OpenUO.Ultima.Adapters;

using SiliconStudio.Paradox.Graphics;

namespace OpenUO.Ultima.Paradox3d.Adapters
{
    internal class AnimationTextureStorageAdapter : Paradox3dStorageAdapterBase, IAnimationStorageAdapter<Texture>
    {
        private const int DoubleXor = (0x200 << 22) | (0x200 << 12);
        private BodyConverter _bodyConverter;
        private BodyTable _bodyTable;
        private FileIndexBase[] _fileIndices;
        private Hues _hues;
        private int[] _table;

        public AnimationTextureStorageAdapter(IContainer container) : base(container)
        {
        }

        public override int Length
        {
            get
            {
                if (!IsInitialized)
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

        public unsafe Frame<Texture>[] GetAnimation(int body, int action, int direction, int hue, bool preserveHue)
        {
            if (preserveHue)
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

            switch (fileType)
            {
                default:
                {
                    if (body < 200)
                    {
                        index = body*110;
                    }
                    else if (body < 400)
                    {
                        index = 22000 + ((body - 200)*65);
                    }
                    else
                    {
                        index = 35000 + ((body - 400)*175);
                    }

                    break;
                }
                case 2:
                {
                    if (body < 200)
                    {
                        index = body*110;
                    }
                    else
                    {
                        index = 22000 + ((body - 200)*65);
                    }

                    break;
                }
                case 3:
                {
                    if (body < 300)
                    {
                        index = body*65;
                    }
                    else if (body < 400)
                    {
                        index = 33000 + ((body - 300)*110);
                    }
                    else
                    {
                        index = 35000 + ((body - 400)*175);
                    }

                    break;
                }
                case 4:
                {
                    if (body < 200)
                    {
                        index = body*110;
                    }
                    else if (body < 400)
                    {
                        index = 22000 + ((body - 200)*65);
                    }
                    else
                    {
                        index = 35000 + ((body - 400)*175);
                    }

                    break;
                }
                case 5:
                {
                    if (body < 200 && body != 34) // looks strange, though it works.
                    {
                        index = body*110;
                    }
                    else
                    {
                        index = 35000 + ((body - 400)*65);
                    }

                    break;
                }
            }

            index += action*5;

            if (direction <= 4)
            {
                index += direction;
            }
            else
            {
                index += direction - (direction - 4)*2;
            }

            int length, extra;

            using (var stream = fileIndex.Seek(index, out length, out extra))
            {
                if (stream == null)
                {
                    return null;
                }

                var flip = (direction > 4);

                using (var bin = new BinaryReader(stream))
                {
                    var palette = new ushort[0x100];

                    for (var i = 0; i < 0x100; ++i)
                    {
                        palette[i] = (ushort) (bin.ReadUInt16() ^ 0x8000);
                    }

                    var startPosition = (int) bin.BaseStream.Position;
                    var frameCount = bin.ReadInt32();

                    var lookups = new int[frameCount];

                    for (var i = 0; i < frameCount; ++i)
                    {
                        lookups[i] = startPosition + bin.ReadInt32();
                    }

                    var frames = new Frame<Texture>[frameCount];

                    for (var i = 0; i < frameCount; ++i)
                    {
                        bin.BaseStream.Seek(lookups[i], SeekOrigin.Begin);

                        int xCenter = bin.ReadInt16();
                        int yCenter = bin.ReadInt16();

                        int width = bin.ReadUInt16();
                        int height = bin.ReadUInt16();

                        var texture = Texture.New2D(GraphicsDevice, width, height, PixelFormat.B5G5R5A1_UNorm);
                        var buffer = new ushort[width*height];

                        fixed (ushort* start = buffer)
                        {
                            var ptr = start;
                            var delta = texture.Width;

                            int header;

                            var xBase = xCenter - 0x200;
                            var yBase = (yCenter + height) - 0x200;

                            if (!flip)
                            {
                                ptr += xBase;
                                ptr += (yBase*delta);

                                while ((header = bin.ReadInt32()) != 0x7FFF7FFF)
                                {
                                    header ^= DoubleXor;

                                    var cur = ptr + ((((header >> 12) & 0x3FF)*delta) + ((header >> 22) & 0x3FF));
                                    var end = cur + (header & 0xFFF);

                                    while (cur < end)
                                    {
                                        *cur++ = palette[bin.ReadByte()];
                                    }
                                }
                            }
                            else
                            {
                                ptr -= xBase - width + 1;
                                ptr += (yBase*delta);

                                while ((header = bin.ReadInt32()) != 0x7FFF7FFF)
                                {
                                    header ^= DoubleXor;

                                    var cur = ptr + ((((header >> 12) & 0x3FF)*delta) - ((header >> 22) & 0x3FF));
                                    var end = cur - (header & 0xFFF);

                                    while (cur > end)
                                    {
                                        *cur-- = palette[bin.ReadByte()];
                                    }
                                }

                                xCenter = width - xCenter;
                            }
                        }

                        texture.SetData(buffer);

                        frames[i] = new Frame<Texture>(xCenter, yCenter, texture);
                    }

                    return frames;
                }
            }
        }

        public Task<Frame<Texture>[]> GetAnimationAsync(int body, int action, int direction, int hue, bool preserveHue)
        {
            return Task.FromResult(GetAnimation(body, action, direction, hue, preserveHue));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            foreach (var fileIndex in _fileIndices)
            {
                fileIndex.Close();
            }

            _fileIndices = null;
            _bodyConverter = null;
            _table = null;
            _bodyTable = null;
            _hues = null;
        }

        public void ApplyHue(Texture texture, Hue hue, bool onlyHueGrayPixels)
        {
            throw new NotSupportedException();
        }

        public void Translate(ref int body)
        {
            if (_table == null)
            {
                InitializeTable();
            }

            if (body <= 0 || body >= _table.Length)
            {
                body = 0;
                return;
            }

            body = (_table[body] & 0x7FFF);
        }

        public void Translate(ref int body, ref int hue)
        {
            if (_table == null)
            {
                InitializeTable();
            }

            if (body <= 0 || body >= _table.Length)
            {
                body = 0;
                return;
            }

            var table = _table[body];

            if ((table & (1 << 31)) != 0)
            {
                body = table & 0x7FFF;

                var vhue = (hue & 0x3FFF) - 1;

                //TODO: this... Hues.List.Length not available from here.
                //if (vhue < 0 || vhue >= Hues.List.Length)
                //    hue = (table >> 15) & 0xFFFF;
            }
        }

        private void InitializeTable()
        {
            var count = 400 + ((_fileIndices[0].Entries.Length - 35000)/175);

            _table = new int[count];

            for (var i = 0; i < count; ++i)
            {
                BodyTableEntry entry;
                _bodyTable.Entries.TryGetValue(i, out entry);

                if (entry == null || _bodyConverter.Contains(i))
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