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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenUO.Core;
using OpenUO.Core.Patterns;
using OpenUO.Ultima.PresentationFramework;

namespace OpenUO.Ultima.UnitTests
{
    [TestClass]
    public class WriteableBitmapAdapterTestspAdapterTests : TestingBase
    {
        private static bool _configuredKernelForTest;
        private static IoCContainer _container;

        protected static IoCContainer Container
        {
            get
            {
                if (!_configuredKernelForTest)
                {
                    _container = new IoCContainer();
                    _container.RegisterModule<UltimaSDKImageSourceModule>();
                    _configuredKernelForTest = true;
                }

                return _container;
            }
        }

        [TestMethod]
        public void TestAnimationWriteableBitmapAdapter()
        {
            AnimationFactory factory = new AnimationFactory(Install, Container);
            Frame<ImageSource>[] frames = factory.GetAnimation<ImageSource>(1, 0, 0, 0, true);

            Guard.AssertIsNotNull(frames, "Animation 1 was not found.");
            Guard.AssertIsNotLessThan("Frames for animation 1, direction 0 were not found.", 1, frames.Length);
        }

        [TestMethod]
        public void TestTexmapsWriteableBitmapAdapter()
        {
            TexmapFactory factory = new TexmapFactory(Install, Container);
            ImageSource texmap = factory.GetTexmap<ImageSource>(0);

            Guard.AssertIsNotNull(texmap, "Texmap 0 was not found.");
        }

        [TestMethod]
        public void TestGumpWriteableBitmapAdapter()
        {
            GumpFactory factory = new GumpFactory(Install, Container);
            ImageSource gump = factory.GetGump<ImageSource>(0);

            Guard.AssertIsNotNull(gump, "Gump 0 was not found.");
        }

        [TestMethod]
        public void TestArtworkWriteableBitmapAdapter()
        {
            ArtworkFactory factory = new ArtworkFactory(Install, Container);

            ImageSource land = factory.GetLand<ImageSource>(0);
            ImageSource @static = factory.GetStatic<ImageSource>(0);

            Guard.AssertIsNotNull(land, "Land tile 0 was not found.");
            Guard.AssertIsNotNull(@static, "Static 0 was not found.");
        }

        [TestMethod]
        public void TestAsciiFontWriteableBitmapAdapter()
        {
            ASCIIFontFactory factory = new ASCIIFontFactory(Install, Container);
            ImageSource text = factory.GetText<ImageSource>(0, "This is a test", 0);
            Guard.AssertIsNotNull(text, "ASCII Font was not created.");
        }

        [TestMethod]
        public void TestUnicodeFontWriteableBitmapAdapter()
        {
            UnicodeFontFactory factory = new UnicodeFontFactory(Install, Container);
            ImageSource text = factory.GetText<ImageSource>(0, "This is a test", 0);
            Guard.AssertIsNotNull(text, "Unicode Font was not created.");
        }
    }
}
