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

using Client.Configuration;
using OpenUO.Core.Patterns;
using SharpDX.Windows;

namespace Client.Graphics
{
    internal class DeviceContextManager : IDeviceContextService
    {
        public DeviceContext Context { get; private set; }
        public RenderForm Form { get; private set; }

        public DeviceContextManager(IoCContainer container)
        {
            IConfiguration configuration = container.Resolve<IConfiguration>();

            int width = configuration.GetValue(ConfigSections.Client, ConfigKeys.GameWidth, 1024);
            int height = configuration.GetValue(ConfigSections.Client, ConfigKeys.GameHeight, 768);

            Form = new RenderForm()
            {
                Width = width,
                Height = height
            };

            Context = new DeviceContext(Form.Handle, width, height, false, false);
        }
    }
}
