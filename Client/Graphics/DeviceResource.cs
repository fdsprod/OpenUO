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


using System;
namespace Client.Graphics
{
    public abstract class DeviceResource : Resource, IDisposable
    {
        protected readonly DeviceContext Context;

        protected DeviceResource(DeviceContext context)
        {
            Context = context;
            Context.Lost += OnContextLost;
            Context.Reset += OnContextReset;
        }

        ~DeviceResource()
        {
            Dispose();
        }

        protected virtual void OnContextLost(DeviceContext context)
        {

        }

        protected virtual void OnContextReset(DeviceContext context)
        {

        }
        
        protected virtual void Dispose(bool disposing)
        {
        }

        public void Dispose()
        {
            Dispose(true);
            Context.Lost -= OnContextLost;
            Context.Reset -= OnContextReset;
            GC.SuppressFinalize(this);
        }
    }
}
