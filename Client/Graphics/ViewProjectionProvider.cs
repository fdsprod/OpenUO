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


using SharpDX;
namespace Client.Graphics
{
    sealed class ViewProjectionProvider : MatrixSource
    {
        private int _camIndex;
        private ICamera _camera;
        private bool _proj;
#if DEBUG
        private DrawState _state;
#endif

        public ViewProjectionProvider(bool projection, DrawState state)
        {
            _proj = projection;
#if DEBUG
            _state = state;
#endif
        }

        public override sealed void UpdateValue(int frame)
        {
        }

        public void Reset()
        {
            _camIndex = 0;
            Value = Matrix.Identity;
        }

        public void SetProjectionCamera(ICamera value, ref Vector2 drawTargetSize)
        {
            if (value != _camera)
            {
                _camera = value;
                _camIndex = 0;
            }

            if (_camera != null)
            {
                if (_camera.GetProjectionMatrix(ref Value, ref drawTargetSize, ref _camIndex))
                {
#if DEBUG
                    _state.Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.MatrixProjectionChanged);
#endif
                    _index++;
                }
            }
        }

        public void SetViewCamera(ICamera value)
        {
            if (value != _camera)
            {
                _camera = value;
                _camIndex = 0;
            }

            if (_camera != null)
            {
                if (_camera.GetViewMatrix(ref Value, ref this._camIndex))
                {
#if DEBUG
                    _state.Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.MatrixViewChanged);
#endif
                    _index++;
                }
            }
        }
    }
}
