#region License Header
/***************************************************************************
 *   Copyright (c) 2011 OpenUO Software Team.
 *   All Right Reserved.
 *
 *   $Id: Engine.cs 15 2011-11-02 07:16:02Z fdsprod@gmail.com $:
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 ***************************************************************************/
#endregion

using System;
using System.Reflection;
using System.Windows.Forms;
using Client.Components;
using Client.Configuration;
using Client.Graphics;
using Client.Patterns.Chaining;
using Client.Ultima;
using OpenUO.Core;
using OpenUO.Core.Diagnostics;
using OpenUO.Core.Patterns;
using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Windows;

namespace Client
{
    public class Engine : IDisposable
    {
        private readonly DeviceContext _context;
        private readonly GameTime _gameTime;
        private readonly RenderForm _form;
        private readonly UpdateState _updateState;
        private readonly IoCContainer _container;
        private readonly IConfiguration _config;
        private readonly World _world;
        private readonly IChain<UpdateState> _updateChain;
        private readonly IWorldRenderChain _worldRenderChain;
        private readonly IUIRenderChain _uiRenderChain;

        private DrawState _drawState;
        private DrawScreenTarget _screenTarget;

        private bool _shouldStop;
        private bool _deviceLost;
        private bool _isFormResizing;

        public Engine(IoCContainer container)
        {
            _container = container;
#if DEBUG_REFERENCES
            SharpDX.Configuration.EnableObjectTracking = true;
            SharpDX.Configuration.EnableReleaseOnFinalizer = true;
#endif
            IDeviceContextService deviceContextService = _container.Resolve<IDeviceContextService>();

            _form = deviceContextService.Form;
            _context = deviceContextService.Context;

            _form.Icon = Resources.openuo;
            _form.Text = string.Format("OpenUO v{0}", new AssemblyInfo(Assembly.GetEntryAssembly()).Version);
            _form.ResizeBegin += OnResizeBegin;
            _form.ResizeEnd += OnResizeEnd;
            _form.FormClosed += OnFormClosed;

            _updateState = new UpdateState();
            _gameTime = new GameTime();
            _world = new World(container);

            container.Resolve<IConsole>().WriteLine("Testing 123");

            _config = _container.Resolve<IConfiguration>();
            _updateChain = _container.Resolve<IChain<UpdateState>>();
            _worldRenderChain = _container.Resolve<IWorldRenderChain>();
            _uiRenderChain = _container.Resolve<IUIRenderChain>();

            _screenTarget = new DrawScreenTarget(_context);

            _updateChain.Freeze();
            _worldRenderChain.Freeze();
            _uiRenderChain.Freeze();
        }

        ~Engine()
        {
            Dispose(false);
        }

        protected virtual void Update(UpdateState state)
        {
            _updateChain.Execute(state);
        }

        protected virtual void OnBeforeDraw()
        {

        }
        protected virtual void OnAfterDraw()
        {

        }

        private void DrawFrame()
        {
            try
            {
                if (_deviceLost)
                {
                    _context.InvokeReset();
                    _deviceLost = false;
#if DEBUG
                    _context.PerformanceMonitor.IncreaseLifetimeCounter(LifetimeCounters.DeviceResets);
#endif
                    return;
                }

                if (_drawState == null)
                {
                    _drawState = new DrawState(_container);
                }

                _drawState.IncrementFrameIndex();
                _drawState.FrameTime = _gameTime.FrameTime;
                _drawState.FrameTimeMs = _gameTime.FrameTimeMs;
                _drawState.TotalGameTime = _gameTime.TotalGameTime;
                _drawState.Paused = _gameTime.Paused;
#if DEBUG
                _context.PerformanceMonitor.StartTimer(DeviceTimers.BeforeDraw);
#endif
                OnBeforeDraw();
#if DEBUG
                _context.PerformanceMonitor.StopTimer(DeviceTimers.BeforeDraw);
                _context.PerformanceMonitor.StartTimer(DeviceTimers.RenderTimer);
#endif
                ushort stackHeight, stateHeight, cameraHeight, preCull, postCull;

                _drawState.GetStackHeight(out stackHeight, out stateHeight, out cameraHeight, out preCull, out postCull);
                _drawState.PrepareForNewFrame();

                if (_drawState.BeginScene())
                {
                    _screenTarget.BeginWorld(_drawState);
                    _worldRenderChain.Execute(_drawState);
                    _drawState.UnbindShader();
                    _screenTarget.EndWorld(_drawState);

                    _screenTarget.BeginUI(_drawState);
                    _uiRenderChain.Execute(_drawState);
                    _drawState.UnbindShader();
                    _screenTarget.EndUI(_drawState);

                    _screenTarget.Combine(_drawState);

                    _drawState.ValidateStackHeight(stackHeight, stateHeight, cameraHeight, preCull, postCull);
                    _drawState.EndScene();
                }
#if DEBUG
                _context.PerformanceMonitor.StopTimer(DeviceTimers.RenderTimer);
                _context.PerformanceMonitor.StartTimer(DeviceTimers.AfterDraw);
#endif
                OnAfterDraw();
#if DEBUG
                _context.PerformanceMonitor.StopTimer(DeviceTimers.AfterDraw);
#endif
                _context.Present();
            }
            catch (SharpDXException e)
            {
                switch (_context.CheckDeviceState())
                {
                    case DeviceState.DeviceHung:
                        throw new Exception("Device hung like a horse.", e);
                    case DeviceState.DeviceLost:
                        _deviceLost = true;
                        break;
                    case DeviceState.DeviceRemoved:
                        throw new Exception("Device was removed", e);
                    case DeviceState.OutOfVideoMemory:
                        throw new Exception("Out of video memory", e);
                    default: throw;
                }
            }
        }

        public void Run()
        {
            try
            {
                _updateState.FrameTime = _gameTime.FrameTime;
                _updateState.FrameTimeMs = _gameTime.FrameTimeMs;
                _updateState.TotalGameTime = _gameTime.TotalGameTime;
                _updateState.Paused = _gameTime.Paused;

                Update(_updateState);

                Tracer.Info("Running Game Loop...");
                RenderLoop.Run(_form, Tick);
            }
            catch (SharpDXException ex)
            {
                Tracer.Error(ex);
                throw;
            }
            catch (Exception e)
            {
                Tracer.Error(e);
                throw;
            }
        }

        public void Tick()
        {
            if (_shouldStop)
                return;
#if DEBUG
            _context.PerformanceMonitor.Reset();
#endif
            _gameTime.UpdateFrame();

            _updateState.FrameTime = _gameTime.FrameTime;
            _updateState.FrameTimeMs = _gameTime.FrameTimeMs;
            _updateState.TotalGameTime = _gameTime.TotalGameTime;
            _updateState.Paused = _gameTime.Paused;
#if DEBUG
            _context.PerformanceMonitor.StartTimer(EngineTimers.Update);
#endif
            Update(_updateState);

            if (!_isFormResizing)
                DrawFrame();
        }

        internal void Stop()
        {
            _shouldStop = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            Stop();
        }

        private void OnResizeBegin(object sender, EventArgs e)
        {
            _isFormResizing = true;
        }

        private void OnResizeEnd(object sender, EventArgs e)
        {
            _isFormResizing = false;

            if (_form.WindowState == FormWindowState.Minimized)
                return;

            _context.Resize(_form.Width, _form.Height);
            _context.InvokeReset();

            _config.SetValue(ConfigSections.Client, ConfigKeys.GameWidth, _form.Width);
            _config.SetValue(ConfigSections.Client, ConfigKeys.GameHeight, _form.Height);
        }
    }
}
