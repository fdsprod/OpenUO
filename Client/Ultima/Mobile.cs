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
using SharpDX;

namespace Client.Ultima
{
    public class Mobile : IAttachable
    {
        private Vector3 _position;

        public Vector3 Position
        {
            get { return _position; }
            set
            {
                if (_position != value)
                {
                    _position = value;
                    OnPositionChanged();
                }
            }
        }

        public event EventHandler PositionChanged;

        private void OnPositionChanged()
        {
            var handler = PositionChanged;

            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
