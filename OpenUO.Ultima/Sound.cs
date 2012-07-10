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
using System.IO;
using System.Media;

namespace OpenUO.Ultima
{
    public class Sound : IDisposable
    {
        private SoundPlayer _player;

        public string Name
        { 
            get;
            private set; 
        }

        public Sound(string name, Stream stream)
        {
            Name = name;
            _player = new SoundPlayer(stream);
        }

        public void Play()
        {
            _player.Play();
        }

        public void PlayLooping()
        {
            _player.PlayLooping();
        }

        public void PlaySync()
        {
            _player.PlaySync();
        }

        public void Stop()
        {
            _player.Stop();
        }

        public void Dispose()
        {
            if (_player == null)
                return;

            _player.Dispose();
        }
    }
}
