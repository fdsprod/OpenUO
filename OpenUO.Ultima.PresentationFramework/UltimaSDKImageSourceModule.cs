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

using System.Windows.Media;
using OpenUO.Core.Patterns;
using OpenUO.Ultima.Adapters;
using OpenUO.Ultima.PresentationFramework.Adapters;

namespace OpenUO.Ultima.PresentationFramework
{
    public class UltimaSDKImageSourceModule : IModule
    {
        public string Name
        {
            get { return "OpenUO Ultima SDK - ImageSource Module"; }
        }

        public void OnLoad(IoCContainer container)
        {
            container.Register<IArtworkStorageAdapter<ImageSource>, ArtworkImageSourceAdapter>();
            container.Register<IAnimationStorageAdapter<ImageSource>, AnimationImageSourceStorageAdapter>();
            container.Register<IASCIIFontStorageAdapter<ImageSource>, ASCIIFontImageSourceAdapter>();
            container.Register<IGumpStorageAdapter<ImageSource>, GumpImageSourceAdapter>();
            container.Register<ITexmapStorageAdapter<ImageSource>, TexmapImageSourceAdapter>();
            container.Register<IUnicodeFontStorageAdapter<ImageSource>, UnicodeFontImageSourceAdapter>();
        }

        public void OnUnload(IoCContainer container)
        {

        }

        private T Get<T>() { return default(T); }
    }
}
