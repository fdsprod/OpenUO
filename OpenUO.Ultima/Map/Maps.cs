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


namespace OpenUO.Ultima
{
    public class Maps
    {
        public readonly Map Felucca;
        public readonly Map Trammel;
        public readonly Map Ilshenar;
        public readonly Map Malas;
        public readonly Map Tokuno;

        public Maps(InstallLocation install)
        {
            Felucca = new Map(install, 0, 0, 6144, 4096);
            Trammel = new Map(install, 0, 1, 6144, 4096);
            Ilshenar = new Map(install, 2, 2, 2304, 1600);
            Malas = new Map(install, 3, 3, 2560, 2048);
            Tokuno = new Map(install, 4, 4, 1448, 1448);
        }
    }
}
