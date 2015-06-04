#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   BitmapAdapterTests.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

#region Usings

using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenUO.Core;
using OpenUO.Core.Patterns;
using OpenUO.Ultima.Windows.Forms;

#endregion

namespace OpenUO.Ultima.UnitTests
{
    [TestClass]
    public class BitmapAdapterTests : TestingBase
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
                    _container.RegisterModule<OpenUOBitmapModule>();
                    _configuredKernelForTest = true;
                }

                return _container;
            }
        }

        [TestMethod]
        public void TestAnimationBitmapAdapter()
        {
            var factory = new AnimationFactory(Install, Container);
            var frames = factory.GetAnimation<Bitmap>(1, 0, 0, 0, true);

            Guard.AssertIsNotNull(frames, "Animation 1 was not found.");
            Guard.AssertIsNotLessThan("Frames for animation 1, direction 0 were not found.", 1, frames.Length);
        }

        [TestMethod]
        public void TestTexmapsBitmapAdapter()
        {
            var factory = new TexmapFactory(Install, Container);
            var texmap = factory.GetTexmap<Bitmap>(1);

            Guard.AssertIsNotNull(texmap, "Texmap 0 was not found.");
        }

        [TestMethod]
        public void TestGumpBitmapAdapter()
        {
            var factory = new GumpFactory(Install, Container);
            var gump = factory.GetGump<Bitmap>(0);

            Guard.AssertIsNotNull(gump, "Gump 0 was not found.");
        }

        [TestMethod]
        public void TestArtworkBitmapAdapter()
        {
            var factory = new ArtworkFactory(Install, Container);

            var land = factory.GetLand<Bitmap>(0);
            var @static = factory.GetStatic<Bitmap>(0);

            Guard.AssertIsNotNull(land, "Land tile 0 was not found.");
            Guard.AssertIsNotNull(@static, "Static 0 was not found.");
        }

        [TestMethod]
        public void TestAsciiFontBitmapAdapter()
        {
            var factory = new ASCIIFontFactory(Install, Container);
            var text = factory.GetText<Bitmap>(0, "This is a test", 0);
            Guard.AssertIsNotNull(text, "ASCII Font was not created.");
        }

        [TestMethod]
        public void TestUnicodeFontBitmapAdapter()
        {
            var factory = new UnicodeFontFactory(Install, Container);
            var text = factory.GetText<Bitmap>(0, "This is a test", 0);
            Guard.AssertIsNotNull(text, "Unicode Font was not created.");
        }
    }
}