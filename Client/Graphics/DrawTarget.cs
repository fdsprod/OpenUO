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


namespace Client.Graphics
{
    public abstract class DrawTarget
    {
        private readonly DeviceContext _context;

        public DeviceContext Context
        {
            get { return _context; }
        } 

        protected DrawTarget(DeviceContext context)
        {
            _context = context;
            _context.Lost += new DeviceContextEventHandler(OnContextResetting);
            _context.Reset += new DeviceContextEventHandler(OnContextReset);
        }

        protected virtual void OnContextReset(DeviceContext context)
        {

        }

        protected virtual void OnContextResetting(DeviceContext context)
        {

        }
    }
}
