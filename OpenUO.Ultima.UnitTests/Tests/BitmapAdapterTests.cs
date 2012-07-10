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

using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenUO.Core;
using OpenUO.Core.Patterns;
using OpenUO.Ultima.Windows.Forms;

namespace OpenUO.Ultima.UnitTests
{
    [TestClass]
    public class BitmapAdapterTests : TestingBase
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
                    _container.RegisterModule<UltimaSDKBitmapModule>();
                    _configuredKernelForTest = true;
                }

                return _container;
            }
        }

        [TestMethod]
        public void TestAnimationBitmapAdapter()
        {
            AnimationFactory factory = new AnimationFactory(Install, Container);
            Frame<Bitmap>[] frames = factory.GetAnimation<Bitmap>(1, 0, 0, 0, true);

            Guard.AssertIsNotNull(frames, "Animation 1 was not found.");
            Guard.AssertIsNotLessThan("Frames for animation 1, direction 0 were not found.", 1, frames.Length);
        }

        [TestMethod]
        public void TestTexmapsBitmapAdapter()
        {
            TexmapFactory factory = new TexmapFactory(Install, Container);
            Bitmap texmap = factory.GetTexmap<Bitmap>(0);

            Guard.AssertIsNotNull(texmap, "Texmap 0 was not found.");
        }

        [TestMethod]
        public void TestGumpBitmapAdapter()
        {
            GumpFactory factory = new GumpFactory(Install, Container);
            Bitmap gump = factory.GetGump<Bitmap>(0);

            Guard.AssertIsNotNull(gump, "Gump 0 was not found.");
        }

        [TestMethod]
        public void TestArtworkBitmapAdapter()
        {
            ArtworkFactory factory = new ArtworkFactory(Install, Container);

            Bitmap land = factory.GetLand<Bitmap>(0);
            Bitmap @static = factory.GetStatic<Bitmap>(0);

            Guard.AssertIsNotNull(land, "Land tile 0 was not found.");
            Guard.AssertIsNotNull(@static, "Static 0 was not found.");
        }

        [TestMethod]
        public void TestAsciiFontBitmapAdapter()
        {
            ASCIIFontFactory factory = new ASCIIFontFactory(Install, Container);
            Bitmap text = factory.GetText<Bitmap>(0, "This is a test", 0);
            Guard.AssertIsNotNull(text, "ASCII Font was not created.");
        }

        [TestMethod]
        public void TestUnicodeFontBitmapAdapter()
        {
            UnicodeFontFactory factory = new UnicodeFontFactory(Install, Container);
            Bitmap text = factory.GetText<Bitmap>(0, "This is a test", 0);
            Guard.AssertIsNotNull(text, "Unicode Font was not created.");
        }
    }
}
