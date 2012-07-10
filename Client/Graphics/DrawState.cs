#region License Header
/***************************************************************************
 *   Copyright (c) 2011 OpenUO Software Team.
 *   All Right Reserved.
 *
 *   $Id: DrawState.cs 14 2011-10-31 07:03:12Z fdsprod@gmail.com $:
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 ***************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using OpenUO.Core.Patterns;
using SharpDX;
using SharpDX.Direct3D9;

namespace Client.Graphics
{
    public partial class DrawState : State, ICuller
    {
        private const int StackHeight = 128;
        
        private readonly DeviceContext _context;
        private readonly Stack<ICamera> _cameraStack;
        private readonly WorldStackProvider _world;
        private readonly ViewProjectionProvider _view;
        private readonly ViewProjectionProvider _projection;
        private readonly ICullPrimitive[] _preCullers = new ICullPrimitive[StackHeight];
        private readonly ICullPrimitive[] _postCullers = new ICullPrimitive[StackHeight];
        private readonly DeviceRenderState[] _renderStateStack = new DeviceRenderState[StackHeight];
        private readonly MatrixCalc _projectionInverse;
        private readonly MatrixCalc _projectionTranspose;
        private readonly MatrixCalc _viewInverse;
        private readonly MatrixCalc _viewProjectionInverse;
        private readonly MatrixCalc _viewProjection;
        private readonly MatrixCalc _viewProjectionTranspose;
        private readonly MatrixCalc _viewTranspose;
        private readonly MatrixCalc _worldInverse;
        private readonly MatrixCalc _worldProjectionInverse;
        private readonly MatrixCalc _worldProjection;
        private readonly MatrixCalc _worldProjectionTranspose;
        private readonly MatrixCalc _worldTranspose;
        private readonly MatrixCalc _worldViewInverse;
        private readonly MatrixCalc _worldView;
        private readonly MatrixCalc _worldViewProjectionInverse;
        private readonly MatrixCalc _worldViewProjection;
        private readonly MatrixCalc _worldViewProjectionTranspose;
        private readonly MatrixCalc _worldViewTranspose;

        public readonly Dictionary<string, object> UserObjects;

        private DeviceRenderState _internalState = new DeviceRenderState();
        private DeviceRenderStateContainer _visibleState = new DeviceRenderStateContainer();

        private VertexDeclaration _vertexDeclaration;
        private IndexBuffer _indexBuffer;
        private VertexStream[] _vertexStreams;
        private IShaderManager _shaderManager;
        private ICamera _camera;
        private int _renderStateStackIndex = 0;
        private bool cameraInvertsCullMode;
        private int frameIndex;
        private ushort _preCullerCount = 0;
        private ushort _postCullerCount = 0;

        internal readonly Dictionary<string, ISemantic> SemanticMappings;

        public int FrameIndex
        {
            get { return frameIndex; }
        }
        
        public ICamera Camera
        {
            get { return _camera; }
        }

        public DeviceContext Context
        {
            get { return _context; }
        }
        
        internal IndexBuffer IndexBuffer
        {
            get { return _indexBuffer; }
            set
            {
                if (_indexBuffer != value)
                {
                    _indexBuffer = value;
                    Context.Indices = value;
                }
            }
        }

        internal VertexDeclaration VertexDeclaration
        {
            get
            {
                return _vertexDeclaration;
            }
            set
            {
                if (_vertexDeclaration != value)
                {
                    Context.VertexDeclaration = value;
                    _vertexDeclaration = value;
                }
            }
        }

        public DeviceRenderStateContainer RenderState
        {
            get { return _visibleState; }
        }

        public Matrix WVP { get { return _worldViewProjection.Value; } }

        public DrawState(IoCContainer container)
        {
            IDeviceContextService deviceContextService = container.Resolve<IDeviceContextService>();

            _context = deviceContextService.Context;
            _shaderManager = container.Resolve<IShaderManager>();
            _cameraStack = new Stack<ICamera>(16);
            _vertexStreams = new VertexStream[Context.Capabilities.MaxStreams];

            _world = new WorldStackProvider(128, this);
            _view = new ViewProjectionProvider(false, this);
            _projection = new ViewProjectionProvider(true, this);
            _projectionInverse = Inverse(_projection);
            _projectionTranspose = Transpose(_projection);
            _viewTranspose = Transpose(_view);
            _viewInverse = Inverse(_view);
            _worldInverse = Inverse(_world);
            _worldTranspose = Transpose(_world);
            _viewProjection = Mult(_view, _projection);
            _worldProjection = Mult(_world, _projection);
            _worldView = Mult(_world, _view);
            _viewProjectionInverse = Inverse(_viewProjection);
            _viewProjectionTranspose = Transpose(_viewProjection);
            _worldProjectionInverse = Inverse(_worldProjection);
            _worldProjectionTranspose = Transpose(_worldProjection);
            _worldViewTranspose = Transpose(_worldView);
            _worldViewInverse = Inverse(_worldView);
            _worldViewProjection = Mult(_world, _viewProjection);
            _worldViewProjectionInverse = Inverse(_worldViewProjection);
            _worldViewProjectionTranspose = Transpose(_worldViewProjection);
            
            SemanticMappings = new Dictionary<string, ISemantic>();

            RegisterShaderSemanticMapping("WORLD", _world);
            RegisterShaderSemanticMapping("WORLDTRANSPOSE",  _worldTranspose);
            RegisterShaderSemanticMapping("WORLDINVERSE",  _worldInverse);
            RegisterShaderSemanticMapping("VIEW",  _view);
            RegisterShaderSemanticMapping("VIEWTRANSPOSE",  _viewTranspose);
            RegisterShaderSemanticMapping("VIEWINVERSE",  _viewInverse);
            RegisterShaderSemanticMapping("PROJECTION",  _projection);
            RegisterShaderSemanticMapping("PROJECTIONTRANSPOSE",  _projectionTranspose);
            RegisterShaderSemanticMapping("PROJECTIONINVERSE",  _projectionInverse);
            RegisterShaderSemanticMapping("WORLDPROJECTION",  _worldProjection);
            RegisterShaderSemanticMapping("WORLDPROJECTIONTRANSPOSE",  _worldProjectionTranspose);
            RegisterShaderSemanticMapping("WORLDPROJECTIONINVERSE",  _worldProjectionInverse);
            RegisterShaderSemanticMapping("VIEWPROJECTION",  _viewProjection);
            RegisterShaderSemanticMapping("VIEWPROJECTIONTRANSPOSE",  _viewProjectionTranspose);
            RegisterShaderSemanticMapping("VIEWPROJECTIONINVERSE",  _viewProjectionInverse);
            RegisterShaderSemanticMapping("WORLDVIEW",  _worldView);
            RegisterShaderSemanticMapping("WORLDVIEWTRANSPOSE",  _worldViewTranspose);
            RegisterShaderSemanticMapping("WORLDVIEWINVERSE",  _worldViewInverse);
            RegisterShaderSemanticMapping("WORLDVIEWPROJECTION",  _worldViewProjection);
            RegisterShaderSemanticMapping("WORLDVIEWPROJECTIONTRANSPOSE",  _worldViewProjectionTranspose);
            RegisterShaderSemanticMapping("WORLDVIEWPROJECTIONINVERSE",  _worldViewProjectionInverse);            
        }

        public T GetUserObject<T>(string key)
        {
            return (T)UserObjects[key];
        }

        private MatrixCalc Inverse(MatrixSource provider)
        {
            return new MatrixCalc(MatrixOp.Inverse, provider, null, this);
        }

        private MatrixCalc Transpose(MatrixSource provider)
        {
            return new MatrixCalc(MatrixOp.Transpose, provider, null, this);
        }

        private MatrixCalc Mult(MatrixSource provider, MatrixSource source)
        {
            return new MatrixCalc(MatrixOp.Multiply, provider, source, this);
        }

        #region Scene
        internal void IncrementFrameIndex()
        {
            frameIndex++;
        }

        internal bool BeginScene()
        {
            return _context.BeginScene();
        }

        internal void EndScene()
        {
            _context.EndScene();
        }

        internal void PrepareForNewFrame()
        {
            _vertexDeclaration = null;
            IndexBuffer = null;

            for (int i = 0; i < _vertexStreams.Length; i++)
            {
                if (_vertexStreams[i].vb == null)
                    continue;

                SetStream(i, null, 0, 0);

                _vertexStreams[i].vb = null;
                _vertexStreams[i].offset = 0;
                _vertexStreams[i].stride = 0;
            }

            _cameraStack.Clear();
            _camera = null;

            _view.Reset();
            _projection.Reset();
            _world.Reset();

            _renderStateStackIndex = 0;
        }
        #endregion

        #region Camera
        public void PushCamera(ICamera camera)
        {
            if (camera == null)
                throw new ArgumentNullException();
            if (camera is Camera2D)
                (camera as Camera2D).Begin(this);

            _cameraStack.Push(this.Camera);

            Vector2 v = new Vector2(Context.Viewport.Width, Context.Viewport.Height);
            SetCamera(camera, ref v);
        }

        public void PushCamera(ICamera camera, Vector2 drawTargetDimmensions)
        {
            if (camera == null)
                throw new ArgumentNullException();

            if (camera is Camera2D)
                (camera as Camera2D).Begin(this);

            _cameraStack.Push(this.Camera);

            SetCamera(camera, ref drawTargetDimmensions);
        }

        private void SetCamera(ICamera value, ref Vector2 targetSize)
        {
            _camera = value;

            if (value != null)
                BeginCamera(value, ref targetSize);
            else
            {
                _view.Reset();
                _projection.Reset();
            }
        }

        internal void BeginCamera(ICamera camera, ref Vector2 targetSize)
        {
#if DEBUG
            Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.SetCameraCount);
#endif
            if (targetSize.X == 0 || targetSize.Y == 0)
            {
                throw new ArgumentException("When pushing a camera, a render target size must be specified if a draw target isn't currently active");
            }

            _projection.SetProjectionCamera(camera, ref targetSize);
            _view.SetViewCamera(camera);

            if (cameraInvertsCullMode != camera.ReverseBackfaceCulling)
            {
                switch (_internalState.DepthColourCull.CullMode)
                {
                    case Cull.Counterclockwise:
                        _internalState.DepthColourCull.CullMode = Cull.Clockwise;
                        break;
                    case Cull.Clockwise:
                        _internalState.DepthColourCull.CullMode = Cull.Counterclockwise;
                        break;
                }

                cameraInvertsCullMode = camera.ReverseBackfaceCulling;
            }
        }

        private void InvalidCamera()
        {
            throw new ArgumentNullException("DrawState.Camera == null.\nThe operation being performed requires a _camera to be active - this usually means there isn't an active draw target.\nNOTE: Drawing may not be performed directly in the Application Draw() method - no DrawTargets are active in this method, so direct rendering cannot be performed.");
        }
        #endregion

        #region Push / Pops
        public void PopCamera()
        {
            Vector2 v = new Vector2(Context.Viewport.Width, Context.Viewport.Height);
            SetCamera(_cameraStack.Pop(), ref v);
        }

        public void PushWorldMatrixMultiply(ref Matrix matrix)
        {
            _world.PushMult(ref matrix);
        }

        public void PushWorldTranslateMultiply(ref Vector3 translate)
        {
            _world.PushMultTrans(ref translate);
        }

        public void PushWorldTranslateMultiply(Vector3 translate)
        {
            _world.PushMultTrans(ref translate);
        }

        public void PushWorldMatrix(ref Matrix matrix)
        {
            _world.Push(ref matrix);
        }

        public void SetWorldMatrix(ref Matrix matrix)
        {
            _world.Set(ref matrix);
        }

        public void PushWorldTranslate(ref Vector3 translate)
        {
            _world.PushTrans(ref translate);
        }

        public void PushWorldTranslate(Vector3 translate)
        {
            _world.PushTrans(ref translate);
        }

        public void PopWorldMatrix()
        {
            _world.Pop();
        }

        public void PushPreCuller(ICullPrimitive culler)
        {
            if (culler == null)
                throw new ArgumentNullException();

            _preCullers[_preCullerCount++] = culler;
        }

        public void PopPreCuller()
        {
            if (_preCullerCount == 0)
                throw new InvalidOperationException("Stack is empty");

            _preCullers[--_preCullerCount] = null;
        }

        public void PushPostCuller(ICullPrimitive culler)
        {
            if (culler == null)
                throw new ArgumentNullException();

            _postCullers[_postCullerCount++] = culler;
        }

        public void PopPostCuller()
        {
            if (_postCullerCount == 0)
                throw new InvalidOperationException("Stack is empty");

            _postCullers[--_postCullerCount] = null;
        }

        public void PushRenderState()
        {
            _renderStateStack[_renderStateStackIndex++] = _visibleState.State;
        }

        public void SetRenderState(ref DeviceRenderState state)
        {
            _visibleState.State = state;
        }

        public void PushRenderState(ref DeviceRenderState newState)
        {
            PushRenderState();
            _visibleState.State = newState;
        }

        public void PopRenderState()
        {
            _visibleState.State = _renderStateStack[checked(--_renderStateStackIndex)];
        }
        #endregion

        #region Render State
        public void ApplyRenderStateChanges()
        {
            ApplyRenderStateChanges(0);
        }

        internal void ApplyRenderStateChanges(int vertexCount)
        {
            _visibleState.State.ApplyState(ref _internalState, _context, this);
        }
        #endregion

        #region Buffers
        internal void UnbindBuffer(VertexBuffer vb)
        {
            if (_vertexStreams == null)
                return;

            for (int i = 0; i < _vertexStreams.Length; i++)
            {
                if (_vertexStreams[i].vb == null)
                    return;

                if (_vertexStreams[i].vb == vb)
                {
                    SetStream(i, null, 0, 0);

                    _vertexStreams[i].vb = null;
                    _vertexStreams[i].offset = 0;
                    _vertexStreams[i].stride = 0;
                }
            }

        }

        internal void UnbindBuffer(IndexBuffer ib)
        {
            if (IndexBuffer == ib)
                IndexBuffer = null;
        }

        internal void SetStream(int index, VertexBuffer vb, int offsetInBytes, int stride)
        {
            if (_vertexStreams[index].vb != vb ||
                _vertexStreams[index].offset != offsetInBytes ||
                _vertexStreams[index].stride != stride)
            {
                Context.SetStreamSource(index, vb, offsetInBytes, stride);

                _vertexStreams[index].vb = vb;
                _vertexStreams[index].offset = offsetInBytes;
                _vertexStreams[index].stride = stride;
            }
        }
        #endregion

        #region Shader
        public void RegisterShaderSemanticMapping(string semanticName, MatrixSource source)
        {            
            if (!SemanticMappings.ContainsKey(semanticName))
                SemanticMappings.Add(semanticName, new MatrixSourceSemantic(semanticName, this, source));
            else
                SemanticMappings[semanticName] = new MatrixSourceSemantic(semanticName, this, source);
        }

        public void RegisterShaderSemanticMapping(string semanticName, BaseTexture texture)
        {
            if (!SemanticMappings.ContainsKey(semanticName))
                SemanticMappings.Add(semanticName, new TextureSemantic(semanticName, texture));
            else
                SemanticMappings[semanticName] = new TextureSemantic(semanticName, texture);
        }

        public void RegisterShaderSemanticMapping<TValue>(string semanticName, SharpDX.Func<TValue> valueDelegate)
            where TValue : struct
        {
            if (!SemanticMappings.ContainsKey(semanticName))
                SemanticMappings.Add(semanticName, new StructSemantic<TValue>(semanticName, valueDelegate));
            else
                SemanticMappings[semanticName] = new StructSemantic<TValue>(semanticName, valueDelegate);
        }

        public TShader GetShader<TShader>() where TShader : Shader
        {
            return _shaderManager.GetShader<TShader>();
        }
        #endregion

        public void GetWorldMatrix(out Matrix matrix)
        {
            matrix = _world.Value;
        }

        void ICuller.GetWorldPosition(out Vector3 position)
        {
            position.X = _world.Value.M41;
            position.Y = _world.Value.M42;
            position.Z = _world.Value.M43;
        }

        private void Transform(ref Vector3 position, ref Matrix matrix, out Vector3 pos)
        {
            Vector4 result;
            Vector3.Transform(ref position, ref matrix, out result);

            pos.X = result.X;
            pos.Y = result.Y;
            pos.Z = result.Z;
        }

        internal void GetStackHeight(out ushort worldHeight, out ushort stateHeight, out ushort cameraHeight, out ushort preCullerCount, out ushort postCullerCount)
        {
            worldHeight = (ushort)_world._top;
            stateHeight = (ushort)_renderStateStackIndex;
            cameraHeight = (ushort)_cameraStack.Count;
            postCullerCount = (ushort)_postCullerCount;
            preCullerCount = (ushort)_preCullerCount;
        }

        internal void ValidateStackHeight(ushort worldHeight, ushort stateHeight, ushort cameraHeight, ushort preCullerCount, ushort postCullerCount)
        {
            if (_world._top != worldHeight ||
                _renderStateStackIndex != stateHeight ||
                _cameraStack.Count != cameraHeight ||
                _preCullerCount != preCullerCount ||
                _postCullerCount != postCullerCount)
            {
                string str = "World matrix, _camera or render state stack corruption detected during method call";
                throw new InvalidOperationException(str);
            }
        }

        #region Culler
        bool ICuller.TestBox(Vector3 min, Vector3 max)
        {
            if (_world._isIdentity)
                return ((ICuller)this).TestWorldBox(ref min, ref max);
#if DEBUG
            Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxCount);

            if (_camera == null)
                InvalidCamera();
#endif
            for (int i = _preCullerCount - 1; i >= 0; i--)
            {
                if (!_preCullers[i].TestWorldBox(ref min, ref max, ref _world.Value))
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxPreCulledCount);
#endif
                    return false;
                }
            }

            if (!FrustumCull.BoxInFrustum(_camera.GetCullingPlanes(), ref min, ref max, ref _world.Value))
            {
#if DEBUG
                Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxCulledCount);
#endif
                return false;
            }

            for (int i = 0; i < _postCullerCount; i++)
            {
                if (!_postCullers[i].TestWorldBox(ref min, ref max, ref _world.Value))
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxPostCulledCount);
#endif
                    return false;
                }
            }
            return true;
        }

        bool ICuller.TestBox(ref Vector3 min, ref Vector3 max)
        {
            if (_world._isIdentity)
                return ((ICuller)this).TestWorldBox(ref min, ref max);
#if DEBUG
            Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxCount);
#endif
#if DEBUG
            if (_camera == null)
                InvalidCamera();
#endif

            for (int i = _preCullerCount - 1; i >= 0; i--)
            {
                if (!_preCullers[i].TestWorldBox(ref min, ref max, ref _world.Value))
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxPreCulledCount);
#endif
                    return false;
                }
            }

            if (!FrustumCull.BoxInFrustum(_camera.GetCullingPlanes(), ref min, ref max, ref _world.Value))
            {
#if DEBUG
                Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxCulledCount);
#endif
                return false;
            }

            for (int i = 0; i < _postCullerCount; i++)
            {
                if (!_postCullers[i].TestWorldBox(ref min, ref max, ref _world.Value))
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxPostCulledCount);
#endif
                    return false;
                }
            }

            return true;
        }

        bool ICuller.TestBox(Vector3 min, Vector3 max, ref Matrix localMatrix)
        {
            if (_world._isIdentity)
                return ((ICuller)this).TestWorldBox(ref min, ref max, ref localMatrix);

            Matrix matrix;

            matrix.M11 = (((localMatrix.M11 * _world.Value.M11) + (localMatrix.M12 * _world.Value.M21)) + (localMatrix.M13 * _world.Value.M31)) + (localMatrix.M14 * _world.Value.M41);
            matrix.M12 = (((localMatrix.M11 * _world.Value.M12) + (localMatrix.M12 * _world.Value.M22)) + (localMatrix.M13 * _world.Value.M32)) + (localMatrix.M14 * _world.Value.M42);
            matrix.M13 = (((localMatrix.M11 * _world.Value.M13) + (localMatrix.M12 * _world.Value.M23)) + (localMatrix.M13 * _world.Value.M33)) + (localMatrix.M14 * _world.Value.M43);
            matrix.M14 = (((localMatrix.M11 * _world.Value.M14) + (localMatrix.M12 * _world.Value.M24)) + (localMatrix.M13 * _world.Value.M34)) + (localMatrix.M14 * _world.Value.M44);
            matrix.M21 = (((localMatrix.M21 * _world.Value.M11) + (localMatrix.M22 * _world.Value.M21)) + (localMatrix.M23 * _world.Value.M31)) + (localMatrix.M24 * _world.Value.M41);
            matrix.M22 = (((localMatrix.M21 * _world.Value.M12) + (localMatrix.M22 * _world.Value.M22)) + (localMatrix.M23 * _world.Value.M32)) + (localMatrix.M24 * _world.Value.M42);
            matrix.M23 = (((localMatrix.M21 * _world.Value.M13) + (localMatrix.M22 * _world.Value.M23)) + (localMatrix.M23 * _world.Value.M33)) + (localMatrix.M24 * _world.Value.M43);
            matrix.M24 = (((localMatrix.M21 * _world.Value.M14) + (localMatrix.M22 * _world.Value.M24)) + (localMatrix.M23 * _world.Value.M34)) + (localMatrix.M24 * _world.Value.M44);
            matrix.M31 = (((localMatrix.M31 * _world.Value.M11) + (localMatrix.M32 * _world.Value.M21)) + (localMatrix.M33 * _world.Value.M31)) + (localMatrix.M34 * _world.Value.M41);
            matrix.M32 = (((localMatrix.M31 * _world.Value.M12) + (localMatrix.M32 * _world.Value.M22)) + (localMatrix.M33 * _world.Value.M32)) + (localMatrix.M34 * _world.Value.M42);
            matrix.M33 = (((localMatrix.M31 * _world.Value.M13) + (localMatrix.M32 * _world.Value.M23)) + (localMatrix.M33 * _world.Value.M33)) + (localMatrix.M34 * _world.Value.M43);
            matrix.M34 = (((localMatrix.M31 * _world.Value.M14) + (localMatrix.M32 * _world.Value.M24)) + (localMatrix.M33 * _world.Value.M34)) + (localMatrix.M34 * _world.Value.M44);
            matrix.M41 = (((localMatrix.M41 * _world.Value.M11) + (localMatrix.M42 * _world.Value.M21)) + (localMatrix.M43 * _world.Value.M31)) + (localMatrix.M44 * _world.Value.M41);
            matrix.M42 = (((localMatrix.M41 * _world.Value.M12) + (localMatrix.M42 * _world.Value.M22)) + (localMatrix.M43 * _world.Value.M32)) + (localMatrix.M44 * _world.Value.M42);
            matrix.M43 = (((localMatrix.M41 * _world.Value.M13) + (localMatrix.M42 * _world.Value.M23)) + (localMatrix.M43 * _world.Value.M33)) + (localMatrix.M44 * _world.Value.M43);
            matrix.M44 = (((localMatrix.M41 * _world.Value.M14) + (localMatrix.M42 * _world.Value.M24)) + (localMatrix.M43 * _world.Value.M34)) + (localMatrix.M44 * _world.Value.M44);

#if DEBUG
            Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxCount);

            if (_camera == null)
                InvalidCamera();
#endif
            for (int i = _preCullerCount - 1; i >= 0; i--)
            {
                if (!_preCullers[i].TestWorldBox(ref min, ref max, ref matrix))
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxPreCulledCount);
#endif
                    return false;
                }
            }

            if (!FrustumCull.BoxInFrustum(_camera.GetCullingPlanes(), ref min, ref max, ref matrix))
            {
#if DEBUG
                Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxCulledCount);
#endif
                return false;
            }

            for (int i = 0; i < _postCullerCount; i++)
            {
                if (!_postCullers[i].TestWorldBox(ref min, ref max, ref matrix))
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxPostCulledCount);
#endif
                    return false;
                }
            }
            return true;
        }

        bool ICuller.TestBox(ref Vector3 min, ref Vector3 max, ref Matrix localMatrix)
        {
            if (_world._isIdentity)
                return ((ICuller)this).TestWorldBox(ref min, ref max, ref localMatrix);

            Matrix matrix;

            matrix.M11 = (((localMatrix.M11 * _world.Value.M11) + (localMatrix.M12 * _world.Value.M21)) + (localMatrix.M13 * _world.Value.M31)) + (localMatrix.M14 * _world.Value.M41);
            matrix.M12 = (((localMatrix.M11 * _world.Value.M12) + (localMatrix.M12 * _world.Value.M22)) + (localMatrix.M13 * _world.Value.M32)) + (localMatrix.M14 * _world.Value.M42);
            matrix.M13 = (((localMatrix.M11 * _world.Value.M13) + (localMatrix.M12 * _world.Value.M23)) + (localMatrix.M13 * _world.Value.M33)) + (localMatrix.M14 * _world.Value.M43);
            matrix.M14 = (((localMatrix.M11 * _world.Value.M14) + (localMatrix.M12 * _world.Value.M24)) + (localMatrix.M13 * _world.Value.M34)) + (localMatrix.M14 * _world.Value.M44);
            matrix.M21 = (((localMatrix.M21 * _world.Value.M11) + (localMatrix.M22 * _world.Value.M21)) + (localMatrix.M23 * _world.Value.M31)) + (localMatrix.M24 * _world.Value.M41);
            matrix.M22 = (((localMatrix.M21 * _world.Value.M12) + (localMatrix.M22 * _world.Value.M22)) + (localMatrix.M23 * _world.Value.M32)) + (localMatrix.M24 * _world.Value.M42);
            matrix.M23 = (((localMatrix.M21 * _world.Value.M13) + (localMatrix.M22 * _world.Value.M23)) + (localMatrix.M23 * _world.Value.M33)) + (localMatrix.M24 * _world.Value.M43);
            matrix.M24 = (((localMatrix.M21 * _world.Value.M14) + (localMatrix.M22 * _world.Value.M24)) + (localMatrix.M23 * _world.Value.M34)) + (localMatrix.M24 * _world.Value.M44);
            matrix.M31 = (((localMatrix.M31 * _world.Value.M11) + (localMatrix.M32 * _world.Value.M21)) + (localMatrix.M33 * _world.Value.M31)) + (localMatrix.M34 * _world.Value.M41);
            matrix.M32 = (((localMatrix.M31 * _world.Value.M12) + (localMatrix.M32 * _world.Value.M22)) + (localMatrix.M33 * _world.Value.M32)) + (localMatrix.M34 * _world.Value.M42);
            matrix.M33 = (((localMatrix.M31 * _world.Value.M13) + (localMatrix.M32 * _world.Value.M23)) + (localMatrix.M33 * _world.Value.M33)) + (localMatrix.M34 * _world.Value.M43);
            matrix.M34 = (((localMatrix.M31 * _world.Value.M14) + (localMatrix.M32 * _world.Value.M24)) + (localMatrix.M33 * _world.Value.M34)) + (localMatrix.M34 * _world.Value.M44);
            matrix.M41 = (((localMatrix.M41 * _world.Value.M11) + (localMatrix.M42 * _world.Value.M21)) + (localMatrix.M43 * _world.Value.M31)) + (localMatrix.M44 * _world.Value.M41);
            matrix.M42 = (((localMatrix.M41 * _world.Value.M12) + (localMatrix.M42 * _world.Value.M22)) + (localMatrix.M43 * _world.Value.M32)) + (localMatrix.M44 * _world.Value.M42);
            matrix.M43 = (((localMatrix.M41 * _world.Value.M13) + (localMatrix.M42 * _world.Value.M23)) + (localMatrix.M43 * _world.Value.M33)) + (localMatrix.M44 * _world.Value.M43);
            matrix.M44 = (((localMatrix.M41 * _world.Value.M14) + (localMatrix.M42 * _world.Value.M24)) + (localMatrix.M43 * _world.Value.M34)) + (localMatrix.M44 * _world.Value.M44);
#if DEBUG
            Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxCount);

            if (_camera == null)
                InvalidCamera();
#endif

            for (int i = _preCullerCount - 1; i >= 0; i--)
            {
                if (!_preCullers[i].TestWorldBox(ref min, ref max, ref matrix))
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxPreCulledCount);
#endif
                    return false;
                }
            }

            if (!FrustumCull.BoxInFrustum(_camera.GetCullingPlanes(), ref min, ref max, ref matrix))
            {
#if DEBUG
                Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxCulledCount);
#endif
                return false;
            }

            for (int i = 0; i < _postCullerCount; i++)
            {
                if (!_postCullers[i].TestWorldBox(ref min, ref max, ref matrix))
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxPostCulledCount);
#endif
                    return false;
                }
            }

            return true;
        }

        bool ICuller.TestSphere(float radius)
        {
#if DEBUG
            Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestSphereCount);

            if (_camera == null)
                InvalidCamera();
#endif
            Vector3 pos;
            pos.X = _world.Value.M41;
            pos.Y = _world.Value.M42;
            pos.Z = _world.Value.M43;

            for (int i = _preCullerCount - 1; i >= 0; i--)
            {
                if (!_preCullers[i].TestWorldSphere(radius, ref pos))
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestSpherePreCulledCount);
#endif
                    return false;
                }
            }

            if (!FrustumCull.SphereInFrustum(_camera.GetCullingPlanes(), radius, ref pos))
            {
#if DEBUG
                Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestSphereCulledCount);
#endif
                return false;
            }

            for (int i = 0; i < _postCullerCount; i++)
            {
                if (!_postCullers[i].TestWorldSphere(radius, ref pos))
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestSpherePostCulledCount);
#endif
                    return false;
                }
            }

            return true;
        }

        bool ICuller.TestSphere(float radius, Vector3 position)
        {
            return ((ICuller)this).TestSphere(radius, ref position);
        }

        bool ICuller.TestSphere(float radius, ref Vector3 position)
        {
#if DEBUG
            Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestSphereCount);

            if (_camera == null)
                InvalidCamera();
#endif
            Vector3 pos;
            if (_world._isIdentity)
                pos = position;
            else
                Transform(ref position, ref _world.Value, out pos);

            for (int i = _preCullerCount - 1; i >= 0; i--)
            {
                if (!_preCullers[i].TestWorldSphere(radius, ref pos))
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestSpherePreCulledCount);
#endif
                    return false;
                }
            }

            if (!FrustumCull.SphereInFrustum(_camera.GetCullingPlanes(), radius, ref pos))
            {
#if DEBUG
                Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestSphereCulledCount);
#endif
                return false;
            }

            for (int i = 0; i < _postCullerCount; i++)
            {
                if (!_postCullers[i].TestWorldSphere(radius, ref pos))
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestSpherePostCulledCount);
#endif
                    return false;
                }
            }

            return true;
        }

        bool ICullPrimitive.TestWorldBox(Vector3 min, Vector3 max, ref Matrix world)
        {
#if DEBUG
            Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxCount);
#endif
            for (int i = _preCullerCount - 1; i >= 0; i--)
            {
                if (!_preCullers[i].TestWorldBox(ref min, ref max, ref world))
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxPreCulledCount);
#endif
                    return false;
                }
            }

            if (!FrustumCull.BoxInFrustum(_camera.GetCullingPlanes(), ref min, ref max, ref world))
            {
#if DEBUG
                Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxCulledCount);
#endif
                return false;
            }

            for (int i = 0; i < _postCullerCount; i++)
            {
                if (!_postCullers[i].TestWorldBox(ref min, ref max, ref world))
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxPostCulledCount);
#endif
                    return false;
                }
            }
            return true;
        }

        bool ICullPrimitive.TestWorldBox(ref Vector3 min, ref Vector3 max, ref Matrix world)
        {
#if DEBUG
            Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxCount);
#endif
            for (int i = _preCullerCount - 1; i >= 0; i--)
            {
                if (!_preCullers[i].TestWorldBox(ref min, ref max, ref world))
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxPreCulledCount);
#endif
                    return false;
                }
            }

            if (!FrustumCull.BoxInFrustum(_camera.GetCullingPlanes(), ref min, ref max, ref world))
            {
#if DEBUG
                Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxCulledCount);
#endif
                return false;
            }

            for (int i = 0; i < _postCullerCount; i++)
            {
                if (!_postCullers[i].TestWorldBox(ref min, ref max, ref world))
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxPostCulledCount);
#endif
                    return false;
                }
            }
            return true;
        }

        bool ICullPrimitive.TestWorldBox(Vector3 min, Vector3 max)
        {
#if DEBUG
            Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxCount);
#endif
            for (int i = _preCullerCount - 1; i >= 0; i--)
            {
                if (!_preCullers[i].TestWorldBox(ref min, ref max))
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxPreCulledCount);
#endif
                    return false;
                }
            }

            if (!FrustumCull.AABBInFrustum(_camera.GetCullingPlanes(), ref min, ref max))
            {
#if DEBUG
                Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxCulledCount);
#endif
                return false;
            }

            for (int i = 0; i < _postCullerCount; i++)
            {
                if (!_postCullers[i].TestWorldBox(ref min, ref max))
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxPostCulledCount);
#endif
                    return false;
                }
            }
            return true;
        }

        bool ICullPrimitive.TestWorldBox(ref Vector3 min, ref Vector3 max)
        {
#if DEBUG
            Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxCount);
#endif
            for (int i = _preCullerCount - 1; i >= 0; i--)
            {
                if (!_preCullers[i].TestWorldBox(ref min, ref max))
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxPreCulledCount);
#endif
                    return false;
                }
            }

            if (!FrustumCull.AABBInFrustum(_camera.GetCullingPlanes(), ref min, ref max))
            {
#if DEBUG
                Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxCulledCount);
#endif
                return false;
            }

            for (int i = 0; i < _postCullerCount; i++)
            {
                if (!_postCullers[i].TestWorldBox(ref min, ref max))
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxPostCulledCount);
#endif
                    return false;
                }
            }
            return true;
        }

        bool ICullPrimitive.TestWorldSphere(float radius, Vector3 position)
        {
#if DEBUG
            Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestSphereCount);
#endif

            for (int i = _preCullerCount - 1; i >= 0; i--)
            {
                if (!_preCullers[i].TestWorldSphere(radius, ref position))
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestSpherePreCulledCount);
#endif
                    return false;
                }
            }

            if (!FrustumCull.SphereInFrustum(_camera.GetCullingPlanes(), radius, ref position))
            {
#if DEBUG
                Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestSphereCulledCount);
#endif
                return false;
            }

            for (int i = 0; i < _postCullerCount; i++)
            {
                if (!_postCullers[i].TestWorldSphere(radius, ref position))
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestSpherePostCulledCount);
#endif
                    return false;
                }
            }
            return true;
        }

        bool ICullPrimitive.TestWorldSphere(float radius, ref Vector3 position)
        {
#if DEBUG
            Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestSphereCount);
#endif
            for (int i = _preCullerCount - 1; i >= 0; i--)
            {
                if (!_preCullers[i].TestWorldSphere(radius, ref position))
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestSpherePreCulledCount);
#endif
                    return false;
                }
            }

            if (!FrustumCull.SphereInFrustum(_camera.GetCullingPlanes(), radius, ref position))
            {
#if DEBUG
                Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestSphereCulledCount);
#endif
                return false;
            }

            for (int i = 0; i < _postCullerCount; i++)
            {
                if (!_postCullers[i].TestWorldSphere(radius, ref position))
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestSpherePostCulledCount);
#endif
                    return false;
                }
            }
            return true;
        }

        ContainmentType ICuller.IntersectBox(Vector3 min, Vector3 max)
        {
            if (_world._isIdentity)
                return ((ICuller)this).IntersectWorldBox(ref min, ref max);

            ContainmentType type; bool intersect = false;
#if DEBUG
            Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxCount);

            if (_camera == null)
                InvalidCamera();
#endif
            for (int i = _preCullerCount - 1; i >= 0; i--)
            {
                type = _preCullers[i].IntersectWorldBox(ref min, ref max, ref _world.Value);
                if (type == ContainmentType.Disjoint)
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxPreCulledCount);
#endif
                    return type;
                }
                if (type == ContainmentType.Intersects)
                    intersect = true;
            }

            type = FrustumCull.BoxIntersectsFrustum(_camera.GetCullingPlanes(), ref min, ref max, ref _world.Value);
            if (type == ContainmentType.Disjoint)
            {
#if DEBUG
                Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxCulledCount);
#endif
                return type;
            }
            if (type == ContainmentType.Intersects)
                intersect = true;

            for (int i = 0; i < _postCullerCount; i++)
            {
                type = _postCullers[i].IntersectWorldBox(ref min, ref max, ref _world.Value);
                if (type == ContainmentType.Disjoint)
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxPostCulledCount);
#endif
                    return type;
                }
                if (type == ContainmentType.Intersects)
                    intersect = true;
            }
            return intersect ? ContainmentType.Intersects : ContainmentType.Contains;
        }

        ContainmentType ICuller.IntersectBox(ref Vector3 min, ref Vector3 max)
        {
            if (_world._isIdentity)
                return ((ICuller)this).IntersectWorldBox(ref min, ref max);

            ContainmentType type; bool intersect = false;
#if DEBUG
            Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxCount);

            if (_camera == null)
                InvalidCamera();
#endif

            for (int i = _preCullerCount - 1; i >= 0; i--)
            {
                type = _preCullers[i].IntersectWorldBox(ref min, ref max, ref _world.Value);
                if (type == ContainmentType.Disjoint)
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxPreCulledCount);
#endif
                    return type;
                }
                if (type == ContainmentType.Intersects)
                    intersect = true;
            }

            type = FrustumCull.BoxIntersectsFrustum(_camera.GetCullingPlanes(), ref min, ref max, ref _world.Value);
            if (type == ContainmentType.Disjoint)
            {
#if DEBUG
                Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxCulledCount);
#endif
                return type;
            }
            if (type == ContainmentType.Intersects)
                intersect = true;

            for (int i = 0; i < _postCullerCount; i++)
            {
                type = _postCullers[i].IntersectWorldBox(ref min, ref max, ref _world.Value);
                if (type == ContainmentType.Disjoint)
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxPostCulledCount);
#endif
                    return type;
                }
                if (type == ContainmentType.Intersects)
                    intersect = true;
            }

            return intersect ? ContainmentType.Intersects : ContainmentType.Contains;
        }

        ContainmentType ICuller.IntersectBox(Vector3 min, Vector3 max, ref Matrix localMatrix)
        {
            if (_world._isIdentity)
                return ((ICuller)this).IntersectWorldBox(ref min, ref max, ref localMatrix);

            Matrix matrix;

            matrix.M11 = (((localMatrix.M11 * _world.Value.M11) + (localMatrix.M12 * _world.Value.M21)) + (localMatrix.M13 * _world.Value.M31)) + (localMatrix.M14 * _world.Value.M41);
            matrix.M12 = (((localMatrix.M11 * _world.Value.M12) + (localMatrix.M12 * _world.Value.M22)) + (localMatrix.M13 * _world.Value.M32)) + (localMatrix.M14 * _world.Value.M42);
            matrix.M13 = (((localMatrix.M11 * _world.Value.M13) + (localMatrix.M12 * _world.Value.M23)) + (localMatrix.M13 * _world.Value.M33)) + (localMatrix.M14 * _world.Value.M43);
            matrix.M14 = (((localMatrix.M11 * _world.Value.M14) + (localMatrix.M12 * _world.Value.M24)) + (localMatrix.M13 * _world.Value.M34)) + (localMatrix.M14 * _world.Value.M44);
            matrix.M21 = (((localMatrix.M21 * _world.Value.M11) + (localMatrix.M22 * _world.Value.M21)) + (localMatrix.M23 * _world.Value.M31)) + (localMatrix.M24 * _world.Value.M41);
            matrix.M22 = (((localMatrix.M21 * _world.Value.M12) + (localMatrix.M22 * _world.Value.M22)) + (localMatrix.M23 * _world.Value.M32)) + (localMatrix.M24 * _world.Value.M42);
            matrix.M23 = (((localMatrix.M21 * _world.Value.M13) + (localMatrix.M22 * _world.Value.M23)) + (localMatrix.M23 * _world.Value.M33)) + (localMatrix.M24 * _world.Value.M43);
            matrix.M24 = (((localMatrix.M21 * _world.Value.M14) + (localMatrix.M22 * _world.Value.M24)) + (localMatrix.M23 * _world.Value.M34)) + (localMatrix.M24 * _world.Value.M44);
            matrix.M31 = (((localMatrix.M31 * _world.Value.M11) + (localMatrix.M32 * _world.Value.M21)) + (localMatrix.M33 * _world.Value.M31)) + (localMatrix.M34 * _world.Value.M41);
            matrix.M32 = (((localMatrix.M31 * _world.Value.M12) + (localMatrix.M32 * _world.Value.M22)) + (localMatrix.M33 * _world.Value.M32)) + (localMatrix.M34 * _world.Value.M42);
            matrix.M33 = (((localMatrix.M31 * _world.Value.M13) + (localMatrix.M32 * _world.Value.M23)) + (localMatrix.M33 * _world.Value.M33)) + (localMatrix.M34 * _world.Value.M43);
            matrix.M34 = (((localMatrix.M31 * _world.Value.M14) + (localMatrix.M32 * _world.Value.M24)) + (localMatrix.M33 * _world.Value.M34)) + (localMatrix.M34 * _world.Value.M44);
            matrix.M41 = (((localMatrix.M41 * _world.Value.M11) + (localMatrix.M42 * _world.Value.M21)) + (localMatrix.M43 * _world.Value.M31)) + (localMatrix.M44 * _world.Value.M41);
            matrix.M42 = (((localMatrix.M41 * _world.Value.M12) + (localMatrix.M42 * _world.Value.M22)) + (localMatrix.M43 * _world.Value.M32)) + (localMatrix.M44 * _world.Value.M42);
            matrix.M43 = (((localMatrix.M41 * _world.Value.M13) + (localMatrix.M42 * _world.Value.M23)) + (localMatrix.M43 * _world.Value.M33)) + (localMatrix.M44 * _world.Value.M43);
            matrix.M44 = (((localMatrix.M41 * _world.Value.M14) + (localMatrix.M42 * _world.Value.M24)) + (localMatrix.M43 * _world.Value.M34)) + (localMatrix.M44 * _world.Value.M44);

            ContainmentType type; bool intersect = false;
#if DEBUG
            Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxCount);

            if (_camera == null)
                InvalidCamera();
#endif
            for (int i = _preCullerCount - 1; i >= 0; i--)
            {
                type = _preCullers[i].IntersectWorldBox(ref min, ref max, ref matrix);
                if (type == ContainmentType.Disjoint)
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxPreCulledCount);
#endif
                    return type;
                }
                if (type == ContainmentType.Intersects)
                    intersect = true;
            }

            type = FrustumCull.BoxIntersectsFrustum(_camera.GetCullingPlanes(), ref min, ref max, ref matrix);
            if (type == ContainmentType.Disjoint)
            {
#if DEBUG
                Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxCulledCount);
#endif
                return type;
            }
            if (type == ContainmentType.Intersects)
                intersect = true;

            for (int i = 0; i < _postCullerCount; i++)
            {
                type = _postCullers[i].IntersectWorldBox(ref min, ref max, ref matrix);
                if (type == ContainmentType.Disjoint)
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxPostCulledCount);
#endif
                    return type;
                }
                if (type == ContainmentType.Intersects)
                    intersect = true;
            }
            return intersect ? ContainmentType.Intersects : ContainmentType.Contains;
        }

        ContainmentType ICuller.IntersectBox(ref Vector3 min, ref Vector3 max, ref Matrix localMatrix)
        {
            if (_world._isIdentity)
                return ((ICuller)this).IntersectWorldBox(ref min, ref max, ref localMatrix);

            Matrix matrix;

            matrix.M11 = (((localMatrix.M11 * _world.Value.M11) + (localMatrix.M12 * _world.Value.M21)) + (localMatrix.M13 * _world.Value.M31)) + (localMatrix.M14 * _world.Value.M41);
            matrix.M12 = (((localMatrix.M11 * _world.Value.M12) + (localMatrix.M12 * _world.Value.M22)) + (localMatrix.M13 * _world.Value.M32)) + (localMatrix.M14 * _world.Value.M42);
            matrix.M13 = (((localMatrix.M11 * _world.Value.M13) + (localMatrix.M12 * _world.Value.M23)) + (localMatrix.M13 * _world.Value.M33)) + (localMatrix.M14 * _world.Value.M43);
            matrix.M14 = (((localMatrix.M11 * _world.Value.M14) + (localMatrix.M12 * _world.Value.M24)) + (localMatrix.M13 * _world.Value.M34)) + (localMatrix.M14 * _world.Value.M44);
            matrix.M21 = (((localMatrix.M21 * _world.Value.M11) + (localMatrix.M22 * _world.Value.M21)) + (localMatrix.M23 * _world.Value.M31)) + (localMatrix.M24 * _world.Value.M41);
            matrix.M22 = (((localMatrix.M21 * _world.Value.M12) + (localMatrix.M22 * _world.Value.M22)) + (localMatrix.M23 * _world.Value.M32)) + (localMatrix.M24 * _world.Value.M42);
            matrix.M23 = (((localMatrix.M21 * _world.Value.M13) + (localMatrix.M22 * _world.Value.M23)) + (localMatrix.M23 * _world.Value.M33)) + (localMatrix.M24 * _world.Value.M43);
            matrix.M24 = (((localMatrix.M21 * _world.Value.M14) + (localMatrix.M22 * _world.Value.M24)) + (localMatrix.M23 * _world.Value.M34)) + (localMatrix.M24 * _world.Value.M44);
            matrix.M31 = (((localMatrix.M31 * _world.Value.M11) + (localMatrix.M32 * _world.Value.M21)) + (localMatrix.M33 * _world.Value.M31)) + (localMatrix.M34 * _world.Value.M41);
            matrix.M32 = (((localMatrix.M31 * _world.Value.M12) + (localMatrix.M32 * _world.Value.M22)) + (localMatrix.M33 * _world.Value.M32)) + (localMatrix.M34 * _world.Value.M42);
            matrix.M33 = (((localMatrix.M31 * _world.Value.M13) + (localMatrix.M32 * _world.Value.M23)) + (localMatrix.M33 * _world.Value.M33)) + (localMatrix.M34 * _world.Value.M43);
            matrix.M34 = (((localMatrix.M31 * _world.Value.M14) + (localMatrix.M32 * _world.Value.M24)) + (localMatrix.M33 * _world.Value.M34)) + (localMatrix.M34 * _world.Value.M44);
            matrix.M41 = (((localMatrix.M41 * _world.Value.M11) + (localMatrix.M42 * _world.Value.M21)) + (localMatrix.M43 * _world.Value.M31)) + (localMatrix.M44 * _world.Value.M41);
            matrix.M42 = (((localMatrix.M41 * _world.Value.M12) + (localMatrix.M42 * _world.Value.M22)) + (localMatrix.M43 * _world.Value.M32)) + (localMatrix.M44 * _world.Value.M42);
            matrix.M43 = (((localMatrix.M41 * _world.Value.M13) + (localMatrix.M42 * _world.Value.M23)) + (localMatrix.M43 * _world.Value.M33)) + (localMatrix.M44 * _world.Value.M43);
            matrix.M44 = (((localMatrix.M41 * _world.Value.M14) + (localMatrix.M42 * _world.Value.M24)) + (localMatrix.M43 * _world.Value.M34)) + (localMatrix.M44 * _world.Value.M44);

            ContainmentType type; bool intersect = false;
#if DEBUG
            Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxCount);

            if (_camera == null)
                InvalidCamera();
#endif

            for (int i = _preCullerCount - 1; i >= 0; i--)
            {
                type = _preCullers[i].IntersectWorldBox(ref min, ref max, ref matrix);
                if (type == ContainmentType.Disjoint)
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxPreCulledCount);
#endif
                    return type;
                }
                if (type == ContainmentType.Intersects)
                    intersect = true;
            }

            type = FrustumCull.BoxIntersectsFrustum(_camera.GetCullingPlanes(), ref min, ref max, ref matrix);
            if (type == ContainmentType.Disjoint)
            {
#if DEBUG
                Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxCulledCount);
#endif
                return type;
            }
            if (type == ContainmentType.Intersects)
                intersect = true;

            for (int i = 0; i < _postCullerCount; i++)
            {
                type = _postCullers[i].IntersectWorldBox(ref min, ref max, ref matrix);
                if (type == ContainmentType.Disjoint)
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxPostCulledCount);
#endif
                    return type;
                }
                if (type == ContainmentType.Intersects)
                    intersect = true;
            }

            return intersect ? ContainmentType.Intersects : ContainmentType.Contains;
        }

        ContainmentType ICuller.IntersectSphere(float radius)
        {
            ContainmentType type; bool intersect = false;
#if DEBUG
            Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestSphereCount);

            if (_camera == null)
                InvalidCamera();
#endif
            Vector3 pos;

            pos.X = _world.Value.M41;
            pos.Y = _world.Value.M42;
            pos.Z = _world.Value.M43;

            for (int i = _preCullerCount - 1; i >= 0; i--)
            {
                type = _preCullers[i].IntersectWorldSphere(radius, ref pos);
                if (type == ContainmentType.Disjoint)
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestSpherePreCulledCount);
#endif
                    return type;
                }
                if (type == ContainmentType.Intersects)
                    intersect = true;
            }

            type = FrustumCull.SphereIntersectsFrustum(_camera.GetCullingPlanes(), radius, ref pos);
            if (type == ContainmentType.Disjoint)
            {
#if DEBUG
                Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestSphereCulledCount);
#endif
                return type;
            }
            if (type == ContainmentType.Intersects)
                intersect = true;

            for (int i = 0; i < _postCullerCount; i++)
            {
                type = _postCullers[i].IntersectWorldSphere(radius, ref pos);
                if (type == ContainmentType.Disjoint)
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestSpherePostCulledCount);
#endif
                    return type;
                }
                if (type == ContainmentType.Intersects)
                    intersect = true;
            }

            return intersect ? ContainmentType.Intersects : ContainmentType.Contains;
        }

        ContainmentType ICuller.IntersectSphere(float radius, Vector3 position)
        {
            return ((ICuller)this).IntersectSphere(radius, ref position);
        }

        ContainmentType ICuller.IntersectSphere(float radius, ref Vector3 position)
        {
            ContainmentType type; bool intersect = false;
#if DEBUG
            Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestSphereCount);

            if (_camera == null)
                InvalidCamera();
#endif
            Vector3 pos;
            Transform(ref position, ref _world.Value, out pos);

            for (int i = _preCullerCount - 1; i >= 0; i--)
            {
                type = _preCullers[i].IntersectWorldSphere(radius, ref pos);
                if (type == ContainmentType.Disjoint)
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestSpherePreCulledCount);
#endif
                    return type;
                }
                if (type == ContainmentType.Intersects)
                    intersect = true;
            }

            type = FrustumCull.SphereIntersectsFrustum(_camera.GetCullingPlanes(), radius, ref pos);

            if (type == ContainmentType.Disjoint)
            {
#if DEBUG
                Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestSphereCulledCount);
#endif
                return type;
            }
            if (type == ContainmentType.Intersects)
                intersect = true;

            for (int i = 0; i < _postCullerCount; i++)
            {
                type = _postCullers[i].IntersectWorldSphere(radius, ref pos);

                if (type == ContainmentType.Disjoint)
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestSpherePostCulledCount);
#endif
                    return type;
                }

                if (type == ContainmentType.Intersects)
                    intersect = true;
            }

            return intersect ? ContainmentType.Intersects : ContainmentType.Contains;
        }

        ContainmentType ICullPrimitive.IntersectWorldBox(Vector3 min, Vector3 max, ref Matrix world)
        {
            ContainmentType type; bool intersect = false;
#if DEBUG
            Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxCount);
#endif
            for (int i = _preCullerCount - 1; i >= 0; i--)
            {
                type = _preCullers[i].IntersectWorldBox(ref min, ref max, ref world);
                if (type == ContainmentType.Disjoint)
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxPreCulledCount);
#endif
                    return type;
                }
                if (type == ContainmentType.Intersects)
                    intersect = true;
            }


            type = FrustumCull.BoxIntersectsFrustum(_camera.GetCullingPlanes(), ref min, ref max, ref world);
            if (type == ContainmentType.Disjoint)
            {
#if DEBUG
                Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxCulledCount);
#endif
                return type;
            }
            if (type == ContainmentType.Intersects)
                intersect = true;

            for (int i = 0; i < _postCullerCount; i++)
            {
                type = _postCullers[i].IntersectWorldBox(ref min, ref max, ref world);
                if (type == ContainmentType.Disjoint)
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxPostCulledCount);
#endif
                    return type;
                }
                if (type == ContainmentType.Intersects)
                    intersect = true;
            }
            return intersect ? ContainmentType.Intersects : ContainmentType.Contains;
        }

        ContainmentType ICullPrimitive.IntersectWorldBox(ref Vector3 min, ref Vector3 max, ref Matrix world)
        {
            ContainmentType type; bool intersect = false;
#if DEBUG
            Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxCount);
#endif
            for (int i = _preCullerCount - 1; i >= 0; i--)
            {
                type = _preCullers[i].IntersectWorldBox(ref min, ref max, ref world);
                if (type == ContainmentType.Disjoint)
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxPreCulledCount);
#endif
                    return type;
                }
                if (type == ContainmentType.Intersects)
                    intersect = true;
            }

            type = FrustumCull.BoxIntersectsFrustum(_camera.GetCullingPlanes(), ref min, ref max, ref world);
            if (type == ContainmentType.Disjoint)
            {
#if DEBUG
                Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxCulledCount);
#endif
                return type;
            }
            if (type == ContainmentType.Intersects)
                intersect = true;

            for (int i = 0; i < _postCullerCount; i++)
            {
                type = _postCullers[i].IntersectWorldBox(ref min, ref max, ref world);
                if (type == ContainmentType.Disjoint)
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxPostCulledCount);
#endif
                    return type;
                }
                if (type == ContainmentType.Intersects)
                    intersect = true;
            }
            return intersect ? ContainmentType.Intersects : ContainmentType.Contains;
        }

        ContainmentType ICullPrimitive.IntersectWorldBox(Vector3 min, Vector3 max)
        {
            ContainmentType type; bool intersect = false;
#if DEBUG
            Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxCount);
#endif
            for (int i = _preCullerCount - 1; i >= 0; i--)
            {
                type = _preCullers[i].IntersectWorldBox(ref min, ref max);
                if (type == ContainmentType.Disjoint)
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxPreCulledCount);
#endif
                    return type;
                }
                if (type == ContainmentType.Intersects)
                    intersect = true;
            }

            type = FrustumCull.AABBIntersectsFrustum(_camera.GetCullingPlanes(), ref min, ref max);
            if (type == ContainmentType.Disjoint)
            {
#if DEBUG
                Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxCulledCount);
#endif
                return type;
            }
            if (type == ContainmentType.Intersects)
                intersect = true;

            for (int i = 0; i < _postCullerCount; i++)
            {
                type = _postCullers[i].IntersectWorldBox(ref min, ref max);
                if (type == ContainmentType.Disjoint)
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxPostCulledCount);
#endif
                    return type;
                }
                if (type == ContainmentType.Intersects)
                    intersect = true;
            }
            return intersect ? ContainmentType.Intersects : ContainmentType.Contains;
        }

        ContainmentType ICullPrimitive.IntersectWorldBox(ref Vector3 min, ref Vector3 max)
        {
            ContainmentType type; bool intersect = false;
#if DEBUG
            Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxCount);
#endif
            for (int i = _preCullerCount - 1; i >= 0; i--)
            {
                type = _preCullers[i].IntersectWorldBox(ref min, ref max);
                if (type == ContainmentType.Disjoint)
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxPreCulledCount);
#endif
                    return type;
                }
                if (type == ContainmentType.Intersects)
                    intersect = true;
            }

            type = FrustumCull.AABBIntersectsFrustum(_camera.GetCullingPlanes(), ref min, ref max);
            if (type == ContainmentType.Disjoint)
            {
#if DEBUG
                Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxCulledCount);
#endif
                return type;
            }
            if (type == ContainmentType.Intersects)
                intersect = true;

            for (int i = 0; i < _postCullerCount; i++)
            {
                type = _postCullers[i].IntersectWorldBox(ref min, ref max);
                if (type == ContainmentType.Disjoint)
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestBoxPostCulledCount);
#endif
                    return type;
                }
                if (type == ContainmentType.Intersects)
                    intersect = true;
            }
            return intersect ? ContainmentType.Intersects : ContainmentType.Contains;
        }

        ContainmentType ICullPrimitive.IntersectWorldSphere(float radius, Vector3 position)
        {
            ContainmentType type; bool intersect = false;
#if DEBUG
            Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestSphereCount);
#endif

            for (int i = _preCullerCount - 1; i >= 0; i--)
            {
                type = _preCullers[i].IntersectWorldSphere(radius, ref position);
                if (type == ContainmentType.Disjoint)
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestSpherePreCulledCount);
#endif
                    return type;
                }
                if (type == ContainmentType.Intersects)
                    intersect = true;
            }

            type = FrustumCull.SphereIntersectsFrustum(_camera.GetCullingPlanes(), radius, ref position);
            if (type == ContainmentType.Disjoint)
            {
#if DEBUG
                Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestSphereCulledCount);
#endif
                return type;
            }
            if (type == ContainmentType.Intersects)
                intersect = true;

            for (int i = 0; i < _postCullerCount; i++)
            {
                type = _postCullers[i].IntersectWorldSphere(radius, ref position);
                if (type == ContainmentType.Disjoint)
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestSpherePostCulledCount);
#endif
                    return type;
                }
                if (type == ContainmentType.Intersects)
                    intersect = true;
            }
            return intersect ? ContainmentType.Intersects : ContainmentType.Contains;
        }

        ContainmentType ICullPrimitive.IntersectWorldSphere(float radius, ref Vector3 position)
        {
            ContainmentType type; bool intersect = false;
#if DEBUG
            Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestSphereCount);
#endif
            for (int i = _preCullerCount - 1; i >= 0; i--)
            {
                type = _preCullers[i].IntersectWorldSphere(radius, ref position);
                if (type == ContainmentType.Disjoint)
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestSpherePreCulledCount);
#endif
                    return type;
                }
                if (type == ContainmentType.Intersects)
                    intersect = true;
            }

            type = FrustumCull.SphereIntersectsFrustum(_camera.GetCullingPlanes(), radius, ref position);
            if (type == ContainmentType.Disjoint)
            {
#if DEBUG
                Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestSphereCulledCount);
#endif
                return type;
            }
            if (type == ContainmentType.Intersects)
                intersect = true;

            for (int i = 0; i < _postCullerCount; i++)
            {
                type = _postCullers[i].IntersectWorldSphere(radius, ref position);
                if (type == ContainmentType.Disjoint)
                {
#if DEBUG
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.DefaultCullerTestSpherePostCulledCount);
#endif
                    return type;
                }
                if (type == ContainmentType.Intersects)
                    intersect = true;
            }
            return intersect ? ContainmentType.Intersects : ContainmentType.Contains;
        }
        #endregion

        #region Nested Class/Structs
        private class MatrixSourceSemantic : ISemantic
        {            
            private MatrixSource _source;
            private DrawState _state;

            public string Name 
            {
                get; 
                private set; 
            }

            public MatrixSourceSemantic(string name, DrawState state, MatrixSource source)
            {
                Name = name;
                _state = state;
                _source = source;
            }
            
            public void Apply(ShaderParameter parameter)
            {
                _source.UpdateValue(_state.frameIndex);
                parameter.SetValue(_source.Value);
            }
        }

        private class StructSemantic<T> : ISemantic
             where T : struct
        {
            private SharpDX.Func<T> _valueDelegate;

            public string Name
            {
                get;
                private set;
            }

            public T Value
            {
                get { return _valueDelegate(); }
            }

            public StructSemantic(string name, SharpDX.Func<T> valueDelegate)
            {
                Name = name;
                _valueDelegate = valueDelegate;
            }

            public void Apply(ShaderParameter parameter)
            {
                parameter.SetValue<T>(Value);
            }
        }

        private class TextureSemantic : ISemantic
        {
            private BaseTexture _texture;

            public string Name
            {
                get;
                private set;
            }

            public BaseTexture Value
            {
                get { return _texture; }
            }

            public TextureSemantic(string name, BaseTexture texture)
            {
                Name = name;
                _texture = texture;
            }

            public void Apply(ShaderParameter parameter)
            {
                parameter.SetTexture(_texture);
            }
        }

        private struct VertexStream
        {
            public VertexBuffer vb;
            public int offset;
            public int stride;
        }
        #endregion

        internal void UnbindShader()
        {
            Context.ActivePass = null;
        }
    }
}
