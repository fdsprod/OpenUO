#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   CoreAdapterTests.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

#region Usings

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenUO.Core;
using OpenUO.Core.Patterns;

#endregion

namespace OpenUO.Ultima.UnitTests
{
    [TestClass]
    public class CoreAdapterTests : TestingBase
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
                    _container.RegisterModule<UltimaSDKCoreModule>();
                    _configuredKernelForTest = true;
                }

                return _container;
            }
        }

        [TestMethod]
        public void TestSoundFactory()
        {
            var factory = new SoundFactory(Install, Container);
            var sound = factory.GetSound<Sound>(1);

            Guard.AssertIsNotNull(sound, "Sound was not created.");

            sound.Play();
        }

        [TestMethod]
        public void TestSkillFactory()
        {
            var factory = new SkillsFactory(Install, Container);
            var skill = factory.GetSkill<Skill>(0);

            Guard.AssertIsNotNull(skill, "Skill was not created.");
        }

        [TestMethod]
        public void TestAnimiationDataFactory()
        {
            var factory = new AnimationDataFactory(Install, Container);
            var animData = factory.GetAnimationData<AnimationData>(0);

            Guard.AssertIsNotNull(animData, "Animation Data was not created.");
        }

        [TestMethod]
        public void TestRadarColors()
        {
            var colors = new RadarColors(Install);

            Guard.Assert(colors.Length > 0, "Radarcol was not parsed correctly");
        }
    }
}