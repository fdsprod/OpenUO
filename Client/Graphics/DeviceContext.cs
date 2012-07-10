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
using Client.Diagnostics;
using OpenUO.Core.Diagnostics;
using SharpDX;
using SharpDX.Direct3D9;
using OpenUO.Core;
#if DEBUG
using SharpDX.Diagnostics;

#endif

namespace Client.Graphics
{
    public delegate void DeviceContextEventHandler(DeviceContext context);

    public sealed class DeviceContext
    {
        private readonly object _syncContext = new object();

        private Viewport _viewport;
        private Device _device;
        private IntPtr _formHandle;
        private int _streamLength;
        private ShaderPass _activePass;
        private Capabilities _capabilities;
        private VertexDeclarationBuilder _vertexDeclarationBuilder;

        public readonly PerformanceMonitor PerformanceMonitor;
        internal PresentParameters PresentationParameters;

        internal ShaderPass ActivePass
        {
            get { return _activePass; }
            set
            {
                if (_activePass == value)
                    return;

                if (_activePass != null)
                    _activePass.End();

                _activePass = value; 
            }
        }

        internal VertexDeclarationBuilder VertexDeclarationBuilder
        {
            get { return _vertexDeclarationBuilder; }
        }

        public object SyncContext
        {
            get { return _syncContext; }
        }

        public long AvailableTextureMemory
        {
            get { return _device.AvailableTextureMemory; }
        }

        public Capabilities Capabilities
        {
            get { return _capabilities; }
        }

        public ClipStatus ClipStatus
        {
            get { return _device.ClipStatus; }
        }

        public CreationParameters CreationParameters
        {
            get { return _device.CreationParameters; }
        }

        public int CurrentTexturePalette
        {
            get { return _device.CurrentTexturePalette; }
            set { _device.CurrentTexturePalette = value; }
        }

        public Surface DepthStencilSurface
        {
            get { return _device.DepthStencilSurface; }
            set { _device.DepthStencilSurface = value; }
        }

        public Direct3D Direct3D
        {
            get { return _device.Direct3D; }
        }

        public DriverLevel DriverLevel
        {
            get { return _device.DriverLevel; }
        }

        public IndexBuffer Indices
        {
            get { return _device.Indices; }
            set { _device.Indices = value; }
        }

        public Material Material
        {
            get { return _device.Material; }
            set { _device.Material = value; }
        }

        public float NPatchMode
        {
            get { return _device.NPatchMode; }
            set { _device.NPatchMode = value; }
        }

        public PixelShader PixelShader
        {
            get { return _device.PixelShader; }
            set { _device.PixelShader = value; }
        }

        public string PixelShaderProfile
        {
            get { return _device.PixelShaderProfile; }
        }

        public Rectangle ScissorRect
        {
            get { return _device.ScissorRect; }
            set { _device.ScissorRect = value; }
        }

        public bool ShowCursor
        {
            get { return _device.ShowCursor; }
            set { _device.ShowCursor = value; }
        }

        public bool SoftwareVertexProcessing
        {
            get { return _device.SoftwareVertexProcessing; }
            set { _device.SoftwareVertexProcessing = value; }
        }

        public int SwapChainCount
        {
            get { return _device.SwapChainCount; }
        }

        public VertexDeclaration VertexDeclaration
        {
            get { return _device.VertexDeclaration; }
            set { _device.VertexDeclaration = value; }
        }

        public VertexFormat VertexFormat
        {
            get { return _device.VertexFormat; }
            set { _device.VertexFormat = value; }
        }

        public VertexShader VertexShader
        {
            get { return _device.VertexShader; }
            set { _device.VertexShader = value; }
        }

        public string VertexShaderProfile
        {
            get { return _device.VertexShaderProfile; }
        }
        
        public Viewport Viewport
        {
            get { return _viewport; }
            set
            {
                _viewport = value;
                _device.Viewport = value; 
            }
        }

        public event DeviceContextEventHandler Lost;
        public event DeviceContextEventHandler Reset;

        private Surface _backBuffer;

        public DeviceContext(IntPtr handle, int backBufferWidth, int backBufferHeight, bool fullScreen, bool verticalSync)
        {
            PerformanceMonitor = new PerformanceMonitor();

            _formHandle = handle;
            PresentationParameters = new PresentParameters()
            {
                BackBufferFormat = Format.X8R8G8B8,
                BackBufferCount = 1,
                BackBufferWidth = backBufferWidth,
                BackBufferHeight = backBufferHeight,
                MultiSampleType = MultisampleType.None,
                SwapEffect = SwapEffect.Discard,
                EnableAutoDepthStencil = true,
                AutoDepthStencilFormat = Format.D24S8,
                PresentFlags = PresentFlags.DiscardDepthStencil,
                PresentationInterval = PresentInterval.Immediate,
                Windowed = !fullScreen,
                DeviceWindowHandle = _formHandle
            };

            try
            {
                Direct3D library = new Direct3D();

                if (library.AdapterCount == 0)
                    throw new Exception("Unable to find an appropriate Direct3D adapter.");

                AdapterDetails adapterDetails = library.GetAdapterIdentifier(0);
                AdapterInformation adapterInformation = library.Adapters[0];

                _device = new Device(library, 0, DeviceType.Hardware, _formHandle, CreateFlags.HardwareVertexProcessing, PresentationParameters);
                _viewport = _device.Viewport;

                _capabilities = _device.Capabilities;

                Tracer.Info("Adapter: {0}", adapterDetails.DeviceName);
                Tracer.Info("Driver: {0}, v{1}", adapterDetails.Driver, adapterDetails.DriverVersion);
                Tracer.Info("Max Texture Size: {0}x{1}", _capabilities.MaxTextureWidth, _capabilities.MaxTextureHeight);
                Tracer.Info("Max Texture Repeat: {0}", _capabilities.MaxTextureRepeat);
                Tracer.Info("Max Texture Aspect Ratio: {0}", _capabilities.MaxTextureAspectRatio);
                Tracer.Info("Texture Size Options: {0}", _capabilities.TextureCaps);
            }
            catch (Exception e)
            {
                Tracer.Error(e);
                return;
            }

            _vertexDeclarationBuilder = new VertexDeclarationBuilder(this);
        }

        public void Dispose()
        {
            if (_device != null)
                _device.Dispose();

            GC.SuppressFinalize(this);
        }

        public void Resize(int width, int height)
        {
            PresentationParameters.BackBufferWidth = width;
            PresentationParameters.BackBufferHeight = height;
        }

        public void InvokeReset()
        {
            var handler = Lost;

            if (handler != null)
                handler(this);

            try
            {
                SetRenderTarget(null);

                if (_backBuffer != null)
                {
                    _backBuffer.Dispose(); 
                    _backBuffer = null;
                }

#if DEBUG_REFERENCES
                IEnumerable<ObjectReference> references = ObjectTracker.FindActiveObjects();

                foreach (var reference in references)
                {
                    if (reference.IsAlive)
                        Tracer.Warn(
                            "{0} reference still alive, if object is not managed, consider calling dispose when the device context is resetting.\n\n{0} Created at:\n{1}",
                            reference.Object.GetType(), 
                            reference.StackTrace); 
                }
#endif

                _device.Reset(PresentationParameters);
                _viewport = _device.Viewport;
            }
            catch (SharpDXException e)
            {
#if DEBUG
                string errorMessage = ErrorManager.GetErrorMessage(e.ResultCode.Code);

                Tracer.Error(errorMessage);
#endif
                Tracer.Error(e);
                throw;
            }

            handler = Reset;

            if (handler != null)
                handler(this);
        }

        public bool BeginScene()
        {
            if (_device.TestCooperativeLevel().Success && _device.BeginScene().Success)
            {
#if DEBUG
                PerformanceMonitor.Reset();
                PerformanceMonitor.StartTimer(DeviceTimers.RenderTimer);
#endif
                return true;
            }

            return false;
        }

        public void EndScene()
        {
            ActivePass = null;

            _device.EndScene();
        }

        public void CalculatePrimitiveCount(PrimitiveType primitiveType, int indexCount, out int primitiveCount)
        {
            switch (primitiveType)
            {
                case PrimitiveType.PointList:
                    primitiveCount = indexCount;
                    break;
                case PrimitiveType.LineList:
                    primitiveCount = indexCount / 2;
                    break;
                case PrimitiveType.TriangleList:
                    primitiveCount = indexCount / 3;
                    break;
                case PrimitiveType.TriangleFan:
                case PrimitiveType.TriangleStrip:
                    primitiveCount = indexCount - 1;
                    break;
                default:
                    primitiveCount = 0;
                    break;
            }
        }

        public void CalculateIndexCount(PrimitiveType primitiveType, int primitiveCount, out int indexCount)
        {
            switch (primitiveType)
            {
                case PrimitiveType.PointList:
                    indexCount = primitiveCount;
                    break;
                case PrimitiveType.LineList:
                    indexCount = primitiveCount * 2;
                    break;
                case PrimitiveType.TriangleList:
                    indexCount = primitiveCount * 3;
                    break;
                case PrimitiveType.TriangleFan:
                case PrimitiveType.TriangleStrip:
                    indexCount = primitiveCount + 1;
                    break;
                default:
                    indexCount = 0;
                    break;
            }
        }

        private void CalculateVertexCount(PrimitiveType primitiveType, int primitiveCount, out int vertexCount)
        {
            switch (primitiveType)
            {
                case PrimitiveType.PointList:
                case PrimitiveType.LineList:
                    vertexCount = primitiveCount;
                    break;
                case PrimitiveType.TriangleList:
                case PrimitiveType.TriangleFan:
                case PrimitiveType.TriangleStrip:
                    vertexCount = primitiveCount * 2;
                    break;
                default:
                    vertexCount = 0;
                    break;
            }
        }

        public DeviceState CheckDeviceState()
        {
            DeviceEx ex = new DeviceEx(_device.NativePointer);
            return ex.CheckDeviceState(_formHandle);
        }

        public Result BeginStateBlock()
        {
            return _device.BeginStateBlock();
        }

        public Result Clear(ClearFlags clearFlags, Color4 color, float zdepth, int stencil)
        {
            return _device.Clear(clearFlags, color, zdepth, stencil);
        }

        public Result Clear(ClearFlags clearFlags, int color, float zdepth, int stencil)
        {
            return _device.Clear(clearFlags, color, zdepth, stencil);
        }

        public Result Clear(ClearFlags clearFlags, Color4 color, float zdepth, int stencil, Rectangle[] rectangles)
        {
            return _device.Clear(clearFlags, color, zdepth, stencil, rectangles);
        }

        public Result Clear(ClearFlags clearFlags, int color, float zdepth, int stencil, Rectangle[] rectangles)
        {
            return _device.Clear(clearFlags, color, zdepth, stencil, rectangles);
        }

        public Result ColorFill(Surface surfaceRef, Color4 color)
        {
            return _device.ColorFill(surfaceRef, color);
        }

        public Result ColorFill(Surface surfaceRef, Rectangle? rectRef, Color4 color)
        {
            return _device.ColorFill(surfaceRef, rectRef, color);
        }

        public Result DeletePatch(int handle)
        {
            return _device.DeletePatch(handle);
        }

        public Result DrawIndexedPrimitive(PrimitiveType primitiveType, int baseVertexIndex, int minVertexIndex, int numVertices, int startIndex, int primCount)
        {
            BeginActivePass();

            Result result = _device.DrawIndexedPrimitive(primitiveType, baseVertexIndex, minVertexIndex, numVertices, startIndex, primCount);
#if DEBUG
            PerformanceMonitor.IncreaseCounter(DeviceCounters.DrawIndexedPrimitive);

            int indexCount;
            CalculateIndexCount(primitiveType, primCount, out indexCount);

            PerformanceMonitor.IncreaseCounter(DeviceCounters.IndicesDrawn, indexCount);
            PerformanceMonitor.IncreaseCounter(DeviceCounters.VerticesDrawn, numVertices);
            PerformanceMonitor.IncreaseCounter(DeviceCounters.PrimitivesDrawn, primCount);
#endif
            return result;
        }

        public Result DrawIndexedUserPrimitives<S, T>(PrimitiveType primitiveType, int minimumVertexIndex, int vertexCount, int primitiveCount, S[] indexData, Format indexDataFormat, T[] vertexData)
            where T : struct
            where S : struct
        {
            BeginActivePass();

            Result result = _device.DrawIndexedUserPrimitives<S, T>(primitiveType, minimumVertexIndex, vertexCount, primitiveCount, indexData, indexDataFormat, vertexData);
#if DEBUG
            PerformanceMonitor.IncreaseCounter(DeviceCounters.DrawIndexedUserPrimitivesST);

            int primCount;
            CalculatePrimitiveCount(primitiveType, indexData.Length, out primCount);

            PerformanceMonitor.IncreaseCounter(DeviceCounters.IndicesDrawn, indexData.Length);
            PerformanceMonitor.IncreaseCounter(DeviceCounters.VerticesDrawn, vertexCount);
            PerformanceMonitor.IncreaseCounter(DeviceCounters.PrimitivesDrawn, primCount);
#endif
            return result;
        }

        public Result DrawIndexedUserPrimitives<S, T>(PrimitiveType primitiveType, int startIndex, int minimumVertexIndex, int vertexCount, int primitiveCount, S[] indexData, Format indexDataFormat, T[] vertexData)
            where T : struct
            where S : struct
        {
            BeginActivePass();

            Result result = _device.DrawIndexedUserPrimitives<S, T>(primitiveType, startIndex, minimumVertexIndex, vertexCount, primitiveCount, indexData, indexDataFormat, vertexData);
#if DEBUG
            PerformanceMonitor.IncreaseCounter(DeviceCounters.DrawIndexedUserPrimitivesST);

            int primCount;
            CalculatePrimitiveCount(primitiveType, indexData.Length, out primCount);

            PerformanceMonitor.IncreaseCounter(DeviceCounters.IndicesDrawn, indexData.Length);
            PerformanceMonitor.IncreaseCounter(DeviceCounters.VerticesDrawn, vertexCount);
            PerformanceMonitor.IncreaseCounter(DeviceCounters.PrimitivesDrawn, primCount);
#endif
            return result;
        }

        public Result DrawIndexedUserPrimitives<S, T>(PrimitiveType primitiveType, int startIndex, int startVertex, int minimumVertexIndex, int vertexCount, int primitiveCount, S[] indexData, Format indexDataFormat, T[] vertexData)
            where T : struct
            where S : struct
        {
            BeginActivePass();

            Result result = _device.DrawIndexedUserPrimitives<S, T>(primitiveType, startIndex, startVertex, minimumVertexIndex, vertexCount, primitiveCount, indexData, indexDataFormat, vertexData);
#if DEBUG
            PerformanceMonitor.IncreaseCounter(DeviceCounters.DrawIndexedUserPrimitivesST);

            int primCount;
            CalculatePrimitiveCount(primitiveType, indexData.Length, out primCount);

            PerformanceMonitor.IncreaseCounter(DeviceCounters.IndicesDrawn, indexData.Length);
            PerformanceMonitor.IncreaseCounter(DeviceCounters.VerticesDrawn, vertexCount);
            PerformanceMonitor.IncreaseCounter(DeviceCounters.PrimitivesDrawn, primCount);
#endif
            return result;
        }

        public Result DrawPrimitives(PrimitiveType primitiveType, int startVertex, int primitiveCount)
        {
            BeginActivePass();

            Result result = _device.DrawPrimitives(primitiveType, startVertex, primitiveCount);
#if DEBUG
            PerformanceMonitor.IncreaseCounter(DeviceCounters.DrawPrimitives);

            int vertexCount;
            CalculateVertexCount(primitiveType, primitiveCount, out vertexCount);

            PerformanceMonitor.IncreaseCounter(DeviceCounters.VerticesDrawn, vertexCount);
            PerformanceMonitor.IncreaseCounter(DeviceCounters.PrimitivesDrawn, primitiveCount);
#endif
            return result;
        }

        public Result DrawUserPrimitives<T>(PrimitiveType primitiveType, int primitiveCount, T[] data) where T : struct
        {
            BeginActivePass();            
#if DEBUG
            PerformanceMonitor.IncreaseCounter(DeviceCounters.DrawUserPrimitivesT);
#endif
            return _device.DrawUserPrimitives<T>(primitiveType, primitiveCount, data);
        }

        public Result DrawUserPrimitives<T>(PrimitiveType primitiveType, int startIndex, int primitiveCount, T[] data) where T : struct
        {
            BeginActivePass();
#if DEBUG
            PerformanceMonitor.IncreaseCounter(DeviceCounters.DrawUserPrimitivesT);
#endif
            return _device.DrawUserPrimitives<T>(primitiveType, startIndex, primitiveCount, data);
        }

        public Result EnableLight(int index, bool enable)
        {
            return _device.EnableLight(index, enable);
        }

        public StateBlock EndStateBlock()
        {
            return _device.EndStateBlock();
        }

        public Result EvictManagedResources()
        {
            return _device.EvictManagedResources();
        }

        public Surface GetBackBuffer(int swapChain, int backBuffer)
        {
            return _device.GetBackBuffer(swapChain, backBuffer);
        }

        public Surface GetRenderTarget(int renderTargetIndex)
        {
            return _device.GetRenderTarget(renderTargetIndex);
        }

        public float GetClipPlane(int index)
        {
            return _device.GetClipPlane(index);
        }

        public DisplayMode GetDisplayMode(int iSwapChain)
        {
            return _device.GetDisplayMode(iSwapChain);
        }

        public Surface GetFrontBufferData(int iSwapChain)
        {
            return _device.GetFrontBufferData(iSwapChain);
        }

        public GammaRamp GetGammaRamp(int iSwapChain)
        {
            return _device.GetGammaRamp(iSwapChain);
        }

        public Light GetLight(int index)
        {
            return _device.GetLight(index);
        }

        public PaletteEntry[] GetPaletteEntries(int paletteNumber)
        {
            return _device.GetPaletteEntries(paletteNumber);
        }

        public bool[] GetPixelShaderBooleanConstant(int startRegister, int count)
        {
            return _device.GetPixelShaderBooleanConstant(startRegister, count);
        }

        public float[] GetPixelShaderFloatConstant(int startRegister, int count)
        {
            return _device.GetPixelShaderFloatConstant(startRegister, count);
        }

        public int[] GetPixelShaderIntegerConstant(int startRegister, int count)
        {
            return _device.GetPixelShaderIntegerConstant(startRegister, count);
        }

        public RasterStatus GetRasterStatus(int iSwapChain)
        {
            return _device.GetRasterStatus(iSwapChain);
        }

        public int GetRenderState(RenderState state)
        {
            return _device.GetRenderState(state);
        }

        public T GetRenderState<T>(RenderState state) where T : struct
        {
            return _device.GetRenderState<T>(state);
        }

        public Result GetRenderTargetData(Surface renderTargetRef, Surface destSurfaceRef)
        {
            return _device.GetRenderTargetData(renderTargetRef, destSurfaceRef);
        }

        public T GetSamplerState<T>(int sampler, SamplerState state) where T : struct
        {
            return _device.GetSamplerState<T>(sampler, state);
        }

        public int GetSamplerState(int sampler, SamplerState state)
        {
            return _device.GetSamplerState(sampler, state);
        }

        public bool GetSetShowCursor(bool bShow)
        {
            return _device.GetSetShowCursor(bShow);
        }

        public Result GetStreamSource(int streamNumber, out VertexBuffer streamDataOut, out int offsetInBytesRef, out int strideRef)
        {
            return _device.GetStreamSource(streamNumber, out streamDataOut, out offsetInBytesRef, out strideRef);
        }

        public Result GetStreamSourceFrequency(int streamNumber, out int settingRef)
        {
            return _device.GetStreamSourceFrequency(streamNumber, out settingRef);
        }

        public SwapChain GetSwapChain(int iSwapChain)
        {
            return _device.GetSwapChain(iSwapChain);
        }

        public BaseTexture GetTexture(int stage)
        {
            return _device.GetTexture(stage);
        }

        public int GetTextureStageState(int stage, TextureStage type)
        {
            return _device.GetTextureStageState(stage, type);
        }

        public T GetTextureStageState<T>(int stage, TextureStage type) where T : struct
        {
            return _device.GetTextureStageState<T>(stage, type);
        }

        public Matrix GetTransform(TransformState state)
        {
            return _device.GetTransform(state);
        }

        public bool[] GetVertexShaderBooleanConstant(int startRegister, int count)
        {
            return _device.GetVertexShaderBooleanConstant(startRegister, count);
        }

        public float[] GetVertexShaderFloatConstant(int startRegister, int count)
        {
            return _device.GetVertexShaderFloatConstant(startRegister, count);
        }

        public int[] GetVertexShaderIntegerConstant(int startRegister, int count)
        {
            return _device.GetVertexShaderIntegerConstant(startRegister, count);
        }

        public bool IsLightEnabled(int index)
        {
            return _device.IsLightEnabled(index);
        }

        public Result MultiplyTransform(TransformState arg0, ref Matrix arg1)
        {
            return _device.MultiplyTransform(arg0, ref arg1);
        }

        public void Present()
        {
            _device.Present();
        }

        public void Present(Rectangle sourceRectangle, Rectangle destinationRectangle)
        {
            _device.Present(sourceRectangle, destinationRectangle);
        }

        public void Present(Rectangle sourceRectangle, Rectangle destinationRectangle, IntPtr windowOverride)
        {
            _device.Present(sourceRectangle, destinationRectangle, windowOverride);
        }

        public Result ProcessVertices(int srcStartIndex, int destIndex, int vertexCount, VertexBuffer destBufferRef, VertexDeclaration vertexDeclRef, LockFlags flags)
        {
            return _device.ProcessVertices(srcStartIndex, destIndex, vertexCount, destBufferRef, vertexDeclRef, flags);
        }

        public Result ResetStreamSourceFrequency(int stream)
        {
            return _device.ResetStreamSourceFrequency(stream);
        }

        public Result SetClipPlane(int index, Vector4 planeRef)
        {
            return _device.SetClipPlane(index, planeRef);
        }

        public void SetCursorPosition(System.Drawing.Point point, bool flags)
        {
            _device.SetCursorPosition(point, flags);
        }

        public void SetCursorPosition(int x, int y, bool flags)
        {
            _device.SetCursorPosition(x, y, flags);
        }

        public Result SetCursorProperties(System.Drawing.Point point, Surface cursorBitmapRef)
        {
            return _device.SetCursorProperties(point, cursorBitmapRef);
        }

        public Result SetCursorProperties(int xHotSpot, int yHotSpot, Surface cursorBitmapRef)
        {
            return _device.SetCursorProperties(xHotSpot, yHotSpot, cursorBitmapRef);
        }

        public void SetGammaRamp(int swapChain, ref GammaRamp rampRef, bool calibrate)
        {
            _device.SetGammaRamp(swapChain, ref rampRef, calibrate);
        }

        public Result SetLight(int index, ref Light arg1)
        {
            return _device.SetLight(index, ref arg1);
        }

        public Result SetPaletteEntries(int paletteNumber, PaletteEntry[] entriesRef)
        {
            return _device.SetPaletteEntries(paletteNumber, entriesRef);
        }

        public Result SetPixelShaderConstant(int startRegister, bool[] data)
        {
            return _device.SetPixelShaderConstant(startRegister, data);
        }

        public Result SetPixelShaderConstant(int startRegister, float[] data)
        {
            return _device.SetPixelShaderConstant(startRegister, data);
        }

        public Result SetPixelShaderConstant(int startRegister, int[] data)
        {
            return _device.SetPixelShaderConstant(startRegister, data);
        }

        public Result SetPixelShaderConstant(int startRegister, Matrix data)
        {
            return _device.SetPixelShaderConstant(startRegister, data);
        }

        public unsafe Result SetPixelShaderConstant(int startRegister, Matrix* data)
        {
            return _device.SetPixelShaderConstant(startRegister, data);
        }

        public Result SetPixelShaderConstant(int startRegister, Matrix[] data)
        {
            return _device.SetPixelShaderConstant(startRegister, data);
        }

        public Result SetPixelShaderConstant(int startRegister, Vector4[] data)
        {
            return _device.SetPixelShaderConstant(startRegister, data);
        }

        public unsafe Result SetPixelShaderConstant(int startRegister, Matrix* data, int count)
        {
            return _device.SetPixelShaderConstant(startRegister, data, count);
        }

        public Result SetPixelShaderConstant(int startRegister, bool[] data, int offset, int count)
        {
            return _device.SetPixelShaderConstant(startRegister, data, offset, count);
        }

        public Result SetPixelShaderConstant(int startRegister, float[] data, int offset, int count)
        {
            return _device.SetPixelShaderConstant(startRegister, data, offset, count);
        }

        public Result SetPixelShaderConstant(int startRegister, int[] data, int offset, int count)
        {
            return _device.SetPixelShaderConstant(startRegister, data, offset, count);
        }

        public Result SetPixelShaderConstant(int startRegister, Matrix[] data, int offset, int count)
        {
            return _device.SetPixelShaderConstant(startRegister, data, offset, count);
        }

        public Result SetPixelShaderConstant(int startRegister, Vector4[] data, int offset, int count)
        {
            return _device.SetPixelShaderConstant(startRegister, data, offset, count);
        }

        public Result SetRenderState(RenderState renderState, bool enable)
        {
            return _device.SetRenderState(renderState, enable);
        }

        public Result SetRenderState(RenderState renderState, float value)
        {
            return _device.SetRenderState(renderState, value);
        }

        public Result SetRenderState(RenderState state, int value)
        {
            return _device.SetRenderState(state, value);
        }

        public Result SetRenderState<T>(RenderState renderState, T value) where T : struct, IConvertible
        {
            return _device.SetRenderState<T>(renderState, value);
        }

        public void SetRenderTarget(params RenderTarget2D[] targets)
        {
            if (targets != null)
            {
                Guard.AssertIsNotLessThan("Must pass in 1 or more targets", 1, targets.Length);
                Guard.AssertIsNotGreaterThan(string.Format("Must pass no more than {0} targets", _capabilities.SimultaneousRTCount), _capabilities.SimultaneousRTCount, targets.Length);
            }

            if (_backBuffer == null)
                _backBuffer = GetRenderTarget(0);

            if (targets == null)
            {
                _device.SetRenderTarget(0, _backBuffer);
            }
            else
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    RenderTarget2D target = targets[i];

                    if (target == null)
                        _device.SetRenderTarget(i, null);
                    else
                        _device.SetRenderTarget(i, target.Surface);
                }
            }
        }

        public Result SetSamplerState(int sampler, SamplerState type, float value)
        {
            return _device.SetSamplerState(sampler, type, value);
        }

        public Result SetSamplerState(int sampler, SamplerState type, int value)
        {
            return _device.SetSamplerState(sampler, type, value);
        }

        public Result SetSamplerState(int sampler, SamplerState type, TextureAddress textureAddress)
        {
            return _device.SetSamplerState(sampler, type, textureAddress);
        }

        public Result SetSamplerState(int sampler, SamplerState type, TextureFilter textureFilter)
        {
            return _device.SetSamplerState(sampler, type, textureFilter);
        }

        public Result SetStreamSource(int streamNumber, VertexBuffer streamDataRef, int offsetInBytes, int stride)
        {
            _streamLength = stride;
            return _device.SetStreamSource(streamNumber, streamDataRef, offsetInBytes, stride);
        }

        public Result SetStreamSourceFrequency(int stream, int frequency, StreamSource source)
        {
            return _device.SetStreamSourceFrequency(stream, frequency, source);
        }

        public Result SetTexture(int stage, BaseTexture textureRef)
        {
            return _device.SetTexture(stage, textureRef);
        }

        public Result SetTextureStageState(int stage, TextureStage type, float value)
        {
            return _device.SetTextureStageState(stage, type, value);
        }

        public Result SetTextureStageState(int stage, TextureStage type, int value)
        {
            return _device.SetTextureStageState(stage, type, value);
        }

        public Result SetTextureStageState(int stage, TextureStage type, TextureArgument textureArgument)
        {
            return _device.SetTextureStageState(stage, type, textureArgument);
        }

        public Result SetTextureStageState(int stage, TextureStage type, TextureOperation textureOperation)
        {
            return _device.SetTextureStageState(stage, type, textureOperation);
        }

        public Result SetTextureStageState(int stage, TextureStage type, TextureTransform textureTransform)
        {
            return _device.SetTextureStageState(stage, type, textureTransform);
        }

        public Result SetTransform(int index, ref Matrix matrixRef)
        {
            return _device.SetTransform(index, ref matrixRef);
        }

        public Result SetTransform(TransformState state, ref Matrix matrixRef)
        {
            return _device.SetTransform(state, ref matrixRef);
        }

        public Result SetVertexShaderConstant(int startRegister, bool[] data)
        {
            return _device.SetVertexShaderConstant(startRegister, data);
        }

        public Result SetVertexShaderConstant(int startRegister, float[] data)
        {
            return _device.SetVertexShaderConstant(startRegister, data);
        }

        public Result SetVertexShaderConstant(int startRegister, int[] data)
        {
            return _device.SetVertexShaderConstant(startRegister, data);
        }

        public Result SetVertexShaderConstant(int startRegister, Matrix data)
        {
            return _device.SetVertexShaderConstant(startRegister, data);
        }

        public unsafe Result SetVertexShaderConstant(int startRegister, Matrix* data)
        {
            return _device.SetVertexShaderConstant(startRegister, data);
        }

        public Result SetVertexShaderConstant(int startRegister, Matrix[] data)
        {
            return _device.SetVertexShaderConstant(startRegister, data);
        }

        public Result SetVertexShaderConstant(int startRegister, Vector4[] data)
        {
            return _device.SetVertexShaderConstant(startRegister, data);
        }

        public unsafe Result SetVertexShaderConstant(int startRegister, Matrix* data, int count)
        {
            return _device.SetVertexShaderConstant(startRegister, data, count);
        }

        public Result SetVertexShaderConstant(int startRegister, bool[] data, int offset, int count)
        {
            return _device.SetVertexShaderConstant(startRegister, data, offset, count);
        }

        public Result SetVertexShaderConstant(int startRegister, float[] data, int offset, int count)
        {
            return _device.SetVertexShaderConstant(startRegister, data, offset, count);
        }

        public Result SetVertexShaderConstant(int startRegister, int[] data, int offset, int count)
        {
            return _device.SetVertexShaderConstant(startRegister, data, offset, count);
        }

        public Result SetVertexShaderConstant(int startRegister, Matrix[] data, int offset, int count)
        {
            return _device.SetVertexShaderConstant(startRegister, data, offset, count);
        }

        public Result SetVertexShaderConstant(int startRegister, Vector4[] data, int offset, int count)
        {
            return _device.SetVertexShaderConstant(startRegister, data, offset, count);
        }

        public Result StretchRectangle(Surface sourceSurfaceRef, Surface destSurfaceRef, TextureFilter filter)
        {
            return _device.StretchRectangle(sourceSurfaceRef, destSurfaceRef, filter);
        }

        public Result StretchRectangle(Surface sourceSurfaceRef, Rectangle? sourceRectRef, Surface destSurfaceRef, Rectangle? destRectRef, TextureFilter filter)
        {
            return _device.StretchRectangle(sourceSurfaceRef, sourceRectRef, destSurfaceRef, destRectRef, filter);
        }

        public Result TestCooperativeLevel()
        {
            return _device.TestCooperativeLevel();
        }

        public Result UpdateSurface(Surface sourceSurfaceRef, Surface destinationSurfaceRef)
        {
            return _device.UpdateSurface(sourceSurfaceRef, destinationSurfaceRef);
        }

        public Result UpdateSurface(Surface sourceSurfaceRef, Rectangle? sourceRectRef, Surface destinationSurfaceRef, System.Drawing.Point? destPointRef)
        {
            return _device.UpdateSurface(sourceSurfaceRef, sourceRectRef, destinationSurfaceRef, destPointRef);
        }

        public Result UpdateTexture(BaseTexture sourceTextureRef, BaseTexture destinationTextureRef)
        {
            return _device.UpdateTexture(sourceTextureRef, destinationTextureRef);
        }

        public Result ValidateDevice(int numPassesRef)
        {
            return _device.ValidateDevice(numPassesRef);
        }

        public static implicit operator Device(DeviceContext a)
        {
            return a._device;
        }

        private void BeginActivePass()
        {
            Guard.AssertIsNotNull(ActivePass, "There is no active shader pass applied.");

            ActivePass.Apply();
        }
    }
}
