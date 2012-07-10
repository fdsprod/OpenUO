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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenUO.Core;
using OpenUO.Core.Patterns;

namespace OpenUO.Ultima.UnitTests
{
    [TestClass]
    public class CoreAdapterTests : TestingBase
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
                    _container.RegisterModule<UltimaSDKCoreModule>();
                    _configuredKernelForTest = true;
                }

                return _container;
            }
        }

        [TestMethod]
        public void TestSoundFactory()
        {
            SoundFactory factory = new SoundFactory(Install, Container);
            Sound sound = factory.GetSound<Sound>(1);

            Guard.AssertIsNotNull(sound, "Sound was not created.");

            sound.Play();
        }

        [TestMethod]
        public void TestSkillFactory()
        {
            SkillsFactory factory = new SkillsFactory(Install, Container);
            Skill skill = factory.GetSkill<Skill>(0);

            Guard.AssertIsNotNull(skill, "Skill was not created.");
        }

        [TestMethod]
        public void TestAnimiationDataFactory()
        {
            AnimationDataFactory factory = new AnimationDataFactory(Install, Container);
            AnimationData animData = factory.GetAnimationData<AnimationData>(0);

            Guard.AssertIsNotNull(animData, "Animation Data was not created.");
        }

        [TestMethod]
        public void TestRadarColors()
        {
            RadarColors colors = new RadarColors(Install);

            Guard.Assert(colors.Length > 0, "Radarcol was not parsed correctly");
        }
    }
}
