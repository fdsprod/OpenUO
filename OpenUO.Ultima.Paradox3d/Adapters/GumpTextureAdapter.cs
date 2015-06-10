using System.IO;
using System.Threading.Tasks;

using OpenUO.Core.Patterns;
using OpenUO.Ultima.Adapters;

using SiliconStudio.Paradox.Graphics;

namespace OpenUO.Ultima.Paradox3d.Adapters
{
    internal class GumpTextureAdapter : Paradox3dStorageAdapterBase, IGumpStorageAdapter<Texture>
    {
        private FileIndexBase _fileIndex;

        public GumpTextureAdapter(IContainer container) : base(container)
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

                return _fileIndex.Length;
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            var install = Install;

            _fileIndex =
                install.IsUopFormat
                    ? install.CreateFileIndex("gumpartLegacyMUL.uop", 0xFFFF, true, ".tga")
                    : install.CreateFileIndex("gumpidx.mul", "gumpart.mul");
        }

        public unsafe Texture GetGump(int index)
        {
            int length, extra;
            using (var stream = _fileIndex.Seek(index, out length, out extra))
            {
                if (stream == null)
                {
                    return null;
                }

                using (var bin = new BinaryReader(stream))
                {
                    var width = (extra >> 16) & 0xFFFF;
                    var height = extra & 0xFFFF;

                    var texture = Texture.New2D(GraphicsDevice, width, height, PixelFormat.B5G5R5A1_UNorm);
                    var buffer = new ushort[width * height];

                    var lookups = new int[height];
                    var startPosition = (int)bin.BaseStream.Position;

                    for (var i = 0; i < height; ++i)
                    {
                        lookups[i] = startPosition + (bin.ReadInt32() * 4);
                    }

                    fixed (ushort* start = buffer)
                    {
                        var ptr = start;
                        var delta = texture.Width;

                        for (var y = 0; y < height; ++y, ptr += delta)
                        {
                            bin.BaseStream.Seek(lookups[y], SeekOrigin.Begin);

                            var cur = ptr;
                            var end = ptr + delta;

                            while (cur < end)
                            {
                                var color = bin.ReadUInt16();
                                var next = cur + bin.ReadUInt16();

                                if (color == 0)
                                {
                                    cur = next;
                                }
                                else
                                {
                                    color ^= 0x8000;

                                    while (cur < next)
                                    {
                                        *cur++ = color;
                                    }
                                }
                            }
                        }
                    }

                    texture.SetData(buffer);

                    return texture;
                }
            }
        }

        public Task<Texture> GetGumpAsync(int index)
        {
            return Task.FromResult(GetGump(index));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (_fileIndex != null)
            {
                _fileIndex.Close();
                _fileIndex = null;
            }
        }
    }
}