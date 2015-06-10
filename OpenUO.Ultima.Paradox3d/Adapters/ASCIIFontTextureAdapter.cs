using System.IO;
using System.Threading.Tasks;

using OpenUO.Core.Patterns;
using OpenUO.Ultima.Adapters;

using SiliconStudio.Paradox.Graphics;

namespace OpenUO.Ultima.Paradox3d.Adapters
{
    internal class ASCIIFontTextureAdapter : Paradox3dStorageAdapterBase, IASCIIFontStorageAdapter<Texture>
    {
        private ASCIIFont[] _fonts;

        public ASCIIFontTextureAdapter(IContainer container) : base(container)
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

                return _fonts.Length;
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            var install = Install;

            _fonts = new ASCIIFont[10];

            using (var reader = new BinaryReader(File.Open(install.GetPath("fonts.mul"), FileMode.Open)))
            {
                for (var i = 0; i < 10; ++i)
                {
                    reader.ReadByte(); //header

                    var chars = new ASCIIChar[224];
                    var fontHeight = 0;

                    for (var k = 0; k < 224; ++k)
                    {
                        var width = reader.ReadByte();
                        var height = reader.ReadByte();

                        reader.ReadByte(); // delimeter?

                        if (width > 0 && height > 0)
                        {
                            if (height > fontHeight && k < 96)
                            {
                                fontHeight = height;
                            }

                            var pixels = new ushort[width * height];

                            for (var y = 0; y < height; ++y)
                            {
                                for (var x = 0; x < width; ++x)
                                {
                                    pixels[y * width + x] = (ushort)(reader.ReadByte() | (reader.ReadByte() << 8));
                                }
                            }

                            chars[k] = new ASCIIChar
                            {
                                Pixels = pixels,
                                Width = width,
                                Height = height
                            };
                        }
                    }

                    _fonts[i] = new ASCIIFont(fontHeight, chars);
                }
            }
        }

        public unsafe Texture GetText(int fontId, string text, short hueId)
        {
            var font = _fonts[fontId];

            var width = font.GetWidth(text);
            var height = font.Height;

            var texture = Texture.New2D(GraphicsDevice, width, height, PixelFormat.B5G5R5A1_UNorm);
            var buffer = new ushort[width * height];

            fixed (ushort* start = buffer)
            {
                var ptr = start;
                var delta = texture.Width;
                
                var dx = 0;

                for (var i = 0; i < text.Length; ++i)
                {
                    var c = ((((text[i]) - 0x20) & 0x7FFFFFFF)%224);
                    var ch = font.Chars[c];

                    var charWidth = ch.Width;
                    var charHeight = ch.Height;

                    var pixels = ch.Pixels;

                    for (var dy = 0; dy < charHeight; ++dy)
                    {
                        var dest = (ptr + (delta*(dy + (font.Height - charHeight)))) + (dx);

                        for (var k = 0; k < charWidth; ++k)
                        {
                            *dest++ = pixels[charWidth*dy + k];
                        }
                    }

                    dx += charWidth;
                }
            }

            texture.SetData(buffer);

            return texture;
        }

        public Task<Texture> GetTextAsync(int fontId, string text, short hueId)
        {
            return Task.FromResult(GetText(fontId, text, hueId));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _fonts = null;
        }
    }
}