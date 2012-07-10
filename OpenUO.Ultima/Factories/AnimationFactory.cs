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
    public class AnimationFactory : AdapterFactoryBase
    {
        public AnimationFactory(InstallLocation install, IoCContainer container)
            : base(install, container) { }

        public Frame<T>[] GetAnimation<T>(int body, int action, int direction, int hue, bool preserveHue)
        {
            return GetAdapter<IAnimationStorageAdapter<T>>().GetAnimation(body, action, direction, hue, preserveHue);
        }
    }
}
