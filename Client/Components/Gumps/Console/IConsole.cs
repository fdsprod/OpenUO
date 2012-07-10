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


using Client.Graphics;
namespace Client.Components
{
    public interface IConsole
    {
        bool IsOpen { get; }

        void WriteLine(string format, params object[] args);        
        void PushColor(Color color);
        void PopColor();

        ConsoleLine[] GetHistory(int count);
    }
}
