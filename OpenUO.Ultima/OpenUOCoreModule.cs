#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// OpenUOCoreModule.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

using System;

using OpenUO.Core.Patterns;
using OpenUO.Ultima.Adapters;

#endregion

namespace OpenUO.Ultima
{
    public class OpenUOCoreModule : IModule
    {
        public string Name
        {
            get { return "OpenUO Ultima SDK - Core Module"; }
        }

        public void OnLoad(IContainer container)
        {
            container.Register<IAnimationDataStorageAdapter<AnimationData>, AnimationDataStorageAdapter>();
            container.Register<ISkillStorageAdapter<Skill>, SkillStorageAdapter>();
            container.Register<ISoundStorageAdapter<Sound>, SoundStorageAdapter>();
        }

        public void OnUnload(IContainer container)
        {
            throw new NotImplementedException();
        }
    }
}