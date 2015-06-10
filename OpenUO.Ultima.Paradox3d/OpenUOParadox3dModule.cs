#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// OpenUOParadox3dModule.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

using OpenUO.Core.Patterns;
using OpenUO.Ultima.Adapters;
using OpenUO.Ultima.Paradox3d.Adapters;

using SiliconStudio.Paradox.Graphics;

#endregion

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
            container.Register<IAnimationStorageAdapter<Texture>, AnimationTextureStorageAdapter>();
            container.Register<IASCIIFontStorageAdapter<Texture>, ASCIIFontTextureAdapter>();
            container.Register<IGumpStorageAdapter<Texture>, GumpTextureAdapter>();
            container.Register<ITexmapStorageAdapter<Texture>, TexmapTextureAdapter>();
            container.Register<IUnicodeFontStorageAdapter<Texture>, UnicodeFontTextureAdapter>();
        }

        public void OnUnload(IContainer container)
        {
        }
    }
}