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

using OpenUO.Core.Patterns;
using OpenUO.Ultima.Adapters;

namespace OpenUO.Ultima
{
    public class UltimaSDKCoreModule : IModule
    {
        public string Name
        {
            get { return "OpenUO Ultima SDK - Core Module"; }
        }

        public void OnLoad(IoCContainer container)
        {
            container.Register<IAnimationDataStorageAdapter<AnimationData>, AnimationDataStorageAdapter>();
            container.Register<ISkillStorageAdapter<Skill>, SkillStorageAdapter>();
            container.Register<ISoundStorageAdapter<Sound>, SoundStorageAdapter>();
        }

        public void OnUnload(IoCContainer container)
        {
            // TODO: Unregister types.
        }
    }
}
