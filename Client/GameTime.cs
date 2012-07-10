#region License Header
/***************************************************************************
 *   Copyright (c) 2011 OpenUO Software Team.
 *   All Right Reserved.
 *
 *   $Id: GameClock.cs 14 2011-10-31 07:03:12Z fdsprod@gmail.com $:
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 ***************************************************************************/
#endregion

using System;
using Client.Diagnostics;

namespace Client
{
    public sealed class GameTime
    {
        public const float MAX_TIME_DELTA = 1 / 25.0f;

        const float _fpsMeasureTimeFrame = 0.5f;

        private PerfTime _lastFrameTime;
        private PerfTime _lastFpsTime;
        private PerfTime _pauseStarted;
        private float _totalGameTime;
        private float _frameTime;
        private float _fps;
        private uint _frameCounter;
        private uint _fpsFrameCounter;
        private bool _paused;

        internal bool Paused
        {
            get { return _paused; }
        }

        public float TotalGameTime
        {
            get { return _totalGameTime; }
        }

        public float FrameTime
        {
            get { return _frameTime; }
        }

        public float FrameTimeMs
        {
            get { return _frameTime * 1000.0f; }
        }

        public float Fps
        {
            get { return _fps; }
        }

        internal GameTime()
        {
            _lastFrameTime = PerfTime.Now;
            _lastFpsTime = _lastFrameTime;
        }

        internal void UpdateFrame()
        {
            PerfTime thisFrameSnapshot = PerfTime.Now;
            PerfTimeSpan timeDelta = thisFrameSnapshot - _lastFrameTime;
            PerfTimeSpan fpsTimeDelta = thisFrameSnapshot - _lastFpsTime;

            _frameCounter++;
            _fpsFrameCounter++;

            if (fpsTimeDelta.Seconds >= _fpsMeasureTimeFrame)
            {
                _fps = (_fpsFrameCounter) / (float)fpsTimeDelta.Seconds;
                _fpsFrameCounter = 0;
                _lastFpsTime = thisFrameSnapshot;
            }

            _frameTime = Math.Min((float)timeDelta.Seconds, MAX_TIME_DELTA);
            _totalGameTime += _frameTime;
            _lastFrameTime = thisFrameSnapshot;
        }

        internal void Pause()
        {
            if (!_paused)
            {
                _paused = true;
                _pauseStarted = PerfTime.Now;
            }
        }

        internal void Resume()
        {
            if (_paused)
            {
                PerfTimeSpan pauseDelta = PerfTime.Now - _pauseStarted;

                _lastFrameTime += pauseDelta;
                _lastFpsTime += pauseDelta;
                _paused = false;
            }
        }
    }
}
