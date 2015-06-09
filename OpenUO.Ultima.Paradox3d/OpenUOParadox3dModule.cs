using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenUO.Core.Patterns;
using OpenUO.Ultima.Adapters;
using OpenUO.Ultima.Paradox3d.Adapters;

using SiliconStudio.Paradox.Graphics;

namespace OpenUO.Ultima.Paradox3d
{
    public class OpenUOParadox3dModule : IModule
    {
        public string Name
        {
            get { return "OpenUO Ultima SDK - Paradox3d Module"; }
        }

        public void OnLoad(IContainer container)
        {
            container.Register<IArtworkStorageAdapter<Texture>, ArtworkTextureAdapter>();
            //container.Register<IAnimationStorageAdapter<Texture>, AnimationTextureStorageAdapter>();
            //container.Register<IASCIIFontStorageAdapter<Texture>, ASCIIFontTextureeAdapter>();
            //container.Register<IGumpStorageAdapter<Texture>, GumpTextureAdapter>();
            container.Register<ITexmapStorageAdapter<Texture>, TexmapTextureAdapter>();
            //container.Register<IUnicodeFontStorageAdapter<Texture>, UnicodeFontTextureAdapter>();
        }

        public void OnUnload(IContainer container)
        {

        }
    }
}
