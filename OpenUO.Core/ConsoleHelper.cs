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
using System.Collections.Generic;

namespace OpenUO.Core
{
    public static class ConsoleHelper
    {
        private static readonly Stack<ConsoleColor> _consoleColors = new Stack<ConsoleColor>();

        /// <summary>
        /// Pushes the color to the console
        /// </summary>
        public static void PushColor(ConsoleColor color)
        {
            try
            {
                _consoleColors.Push(Console.ForegroundColor);
                Console.ForegroundColor = color;
            }
            catch
            {
            }
        }

        /// <summary>
        /// Pops the color of the console to the previous value.
        /// </summary>
        public static ConsoleColor PopColor()
        {
            try
            {
                Console.ForegroundColor = _consoleColors.Pop();
            }
            catch
            {

            }

            return Console.ForegroundColor;
        }
    }
}
