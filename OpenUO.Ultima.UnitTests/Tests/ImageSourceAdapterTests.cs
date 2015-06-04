#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   ImageSourceAdapterTests.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

#region Usings

using System.Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenUO.Core;
using OpenUO.Core.Patterns;
using OpenUO.Ultima.PresentationFramework;

#endregion

namespace OpenUO.Ultima.UnitTests
{
    [TestClass]
    public class WriteableBitmapAdapterTestspAdapterTests : TestingBase
    {
        private static bool _configuredKernelForTest;
        private static Container _container;

        protected static Container Container
        {
            get
            {
                if(!_configuredKernelForTest)
                {
                    _container = new Container();
                    _container.RegisterModule<OpenUOImageSourceModule>();
                    _configuredKernelForTest = true;
                }

                return _container;
            }
        }

        [TestMethod]
        public void TestAnimationWriteableBitmapAdapter()
        {
            var factory = new AnimationFactory(Install, Container);
            var frames = factory.GetAnimation<ImageSource>(1, 0, 0, 0, true);

            Guard.AssertIsNotNull(frames, "Animation 1 was not found.");
            Guard.AssertIsNotLessThan("Frames for animation 1, direction 0 were not found.", 1, frames.Length);
        }

        [TestMethod]
        public void TestTexmapsWriteableBitmapAdapter()
        {
            var factory = new TexmapFactory(Install, Container);
            var texmap = factory.GetTexmap<ImageSource>(1);

            Guard.AssertIsNotNull(texmap, "Texmap 0 was not found.");
        }

        [TestMethod]
        public void TestGumpWriteableBitmapAdapter()
        {
            var factory = new GumpFactory(Install, Container);
            var gump = factory.GetGump<ImageSource>(0);

            Guard.AssertIsNotNull(gump, "Gump 0 was not found.");
        }

        [TestMethod]
        public void TestArtworkWriteableBitmapAdapter()
        {
            var factory = new ArtworkFactory(Install, Container);

            var land = factory.GetLand<ImageSource>(0);
            var @static = factory.GetStatic<ImageSource>(0);

            Guard.AssertIsNotNull(land, "Land tile 0 was not found.");
            Guard.AssertIsNotNull(@static, "Static 0 was not found.");
        }

        [TestMethod]
        public void TestAsciiFontWriteableBitmapAdapter()
        {
            var factory = new ASCIIFontFactory(Install, Container);
            var text = factory.GetText<ImageSource>(0, "This is a test", 0);

            Guard.AssertIsNotNull(text, "ASCII Font was not created.");
        }

        [TestMethod]
        public void TestUnicodeFontWriteableBitmapAdapter()
        {
            var factory = new UnicodeFontFactory(Install, Container);
            var text = factory.GetText<ImageSource>(0, "This is a test", 0);

            Guard.AssertIsNotNull(text, "Unicode Font was not created.");
        }
    }
}