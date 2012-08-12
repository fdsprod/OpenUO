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
using SharpDX;
using SharpDX.Direct3D9;

namespace Client.Graphics
{
    public sealed class Vertices<TVertexType> : DeviceResource, IVertices, IDeviceVertexBuffer
        where TVertexType : struct
    {
        public static Vertices<TVertexType> CreateRawDataVertices(DeviceContext context, TVertexType[] data, VertexElement[] elements)
        {
            if (typeof(byte) != typeof(TVertexType))
                throw new ArgumentException("Only byte[] raw vertices are supported at this time");
            if (data == null || elements == null)
                throw new ArgumentNullException();
            if (elements.Length == 0)
                throw new ArgumentException();

            int stride = VertexElementAttribute.CalculateVertexStride(elements);

            return new Vertices<TVertexType>(context, data, elements, stride);
        }

        public static Vertices<TVertexType> CreateSingleElementVertices(DeviceContext context, TVertexType[] data, DeclarationUsage elementUsage, int index)
        {
            if (typeof(float) != typeof(TVertexType) &&
                typeof(Vector2) != typeof(TVertexType) &&
                typeof(Vector3) != typeof(TVertexType) &&
                typeof(Vector4) != typeof(TVertexType) &&
                typeof(Half2) != typeof(TVertexType) &&
                typeof(Half4) != typeof(TVertexType))
                throw new ArgumentException("Only float and vector types are supported for single element vertex buffers");

            if (data == null)
                throw new ArgumentNullException();
            if (index >= 16 || index < 0)
                throw new ArgumentException("index");

            DeclarationType format = VertexDeclarationBuilder.DetermineFormat(typeof(TVertexType));
            VertexElement[] elements = new VertexElement[] { new VertexElement(0, 0, format, DeclarationMethod.Default, elementUsage, (byte)index) };

            int stride = VertexElementAttribute.CalculateVertexStride(elements);

            return new Vertices<TVertexType>(context, data, elements, stride);
        }

        private Implementation _buffer;
        private VertexBuffer _vertexBuffer;
        private VertexDeclaration _vertexDeclaration;
        private Usage _usage;

        internal override ResourceType ResourceType
        {
            get { return ResourceType.VertexBuffer; }
        }

        internal override bool InUse
        {
            get { return _buffer != null && _vertexBuffer != null; }
        }
        
        public Type VertexType
        {
            get
            {
                ValidateDisposed();
                return _buffer.Type;
            }
        }

        public int Count
        {
            get
            {
                ValidateDisposed();
                return _buffer.Count;
            }
        }

        public int Stride
        {
            get
            {
                ValidateDisposed();
                return _buffer.Stride;
            }
        }

        public Usage ResourceUsage
        {
            get { return _usage; }
            set
            {
                if (_vertexBuffer != null)
                    throw new InvalidOperationException("Cannot set Usage when resource is in use");

                _usage = value;
                _buffer.SequentialWriteFlag = (value & Usage.Dynamic) == Usage.Dynamic;
            }
        }

        private Vertices(DeviceContext context, TVertexType[] data, VertexElement[] elements, int stride)
            : base(context)
        {
            _usage = Usage.Dynamic;
            _buffer = new Implementation(data, elements);
            _buffer.OverrideSize(stride, (data.Length * _buffer.Stride) / stride);
        }

        public Vertices(DeviceContext context, params TVertexType[] vertices)
            : this(context, Usage.Dynamic, (IEnumerable<TVertexType>)vertices) { }

        public Vertices(DeviceContext context, IEnumerable<TVertexType> vertices)
            : this(context, Usage.Dynamic, vertices) { }

        private Vertices(DeviceContext context, Usage usage, IEnumerable<TVertexType> vertices)
            : base(context)
        {
#if DEBUG
            if (VertexDeclarationBuilder.Instance != null && typeof(TVertexType) != typeof(byte) && VertexDeclarationBuilder.Instance != null) //format used by raw data vertices
                VertexDeclarationBuilder.Instance.GetDeclaration(typeof(TVertexType));
#endif
            if (typeof(TVertexType) == typeof(byte))
                throw new ArgumentException("VertexType");

            if (vertices == null)
                throw new ArgumentNullException("vertices");

            _usage = usage;
            _buffer = new Implementation(vertices);

            SetDirty();
        }

        ~Vertices()
        {
            Dispose();
        }

        internal override int GetAllocatedDeviceBytes()
        {
            if (_vertexBuffer != null)
                return _vertexBuffer.Description.SizeInBytes;

            return 0;
        }

        internal override int GetAllocatedManagedBytes()
        {
            int count = _buffer != null ? _buffer.GetCount() : 0;
            return Math.Max(0, count);
        }

        public VertexDeclaration GetVertexDeclaration(DeviceContext context)
        {
            if (_vertexDeclaration == null)
            {
                if (_buffer.IsRawDataVertices)
                    _vertexDeclaration = context.VertexDeclarationBuilder.GetDeclaration(_buffer.GetVertexElements());
                else
                    _vertexDeclaration = context.VertexDeclarationBuilder.GetDeclaration<TVertexType>();
            }

            return _vertexDeclaration;
        }

        public VertexBuffer GetVertexBuffer(DrawState state)
        {
            if (_vertexBuffer == null)
            {
                int size = 32;

                if (_buffer.CountKnown)
                    size = _buffer.Count;

                if (size == 0)
                    throw new ArgumentException(string.Format("Vertices<{0}> data size is zero", typeof(TVertexType).Name));

                _vertexBuffer = new VertexBuffer(Context, _buffer.Stride * size, Usage.WriteOnly, VertexFormat.None, Pool.Managed);

                if ((_usage & Usage.Dynamic) != Usage.Dynamic)
                    _buffer.ClearBuffer();
            }

            if (_buffer.IsDirty)
            {
                _buffer.UpdateDirtyRegions(state, _vertexBuffer);
            }

            return _vertexBuffer;
        }

        public void SetDirty()
        {
            ValidateDisposed();
            ValidateDirty();

            _buffer.AddDirtyRange(0, _buffer.Count, GetType(), true);
        }

        public void SetDirtyRange(int startIndex, int count)
        {
            ValidateDisposed();
            ValidateDirty();

            _buffer.AddDirtyRange(startIndex, count, GetType(), false);
        }

        public void Draw(DrawState state, IIndices indices, PrimitiveType primitiveType)
        {
            ValidateDisposed();

            IDeviceIndexBuffer devib = (IDeviceIndexBuffer)indices;
            IndexBuffer ib = null;

            if (devib != null)
                ib = devib.GetIndexBuffer(state);

            if (_vertexDeclaration == null)
                _vertexDeclaration = Context.VertexDeclarationBuilder.GetDeclaration<TVertexType>();

            VertexBuffer vb = ((IDeviceVertexBuffer)this).GetVertexBuffer(state);
            VertexDeclaration vd = _vertexDeclaration;
            
            state.VertexDeclaration = vd;
            state.IndexBuffer = ib;
            state.SetStream(0, vb, 0, _buffer.Stride);

            int vertices = _buffer.Count;

            if (indices != null)
                vertices = indices.Count;

            int primitives = 0;

            switch (primitiveType)
            {
                case PrimitiveType.LineList:
                    primitives = vertices / 2;
                    break;
                case PrimitiveType.LineStrip:
                    primitives = vertices - 1;
                    break;
                case PrimitiveType.PointList:
                    primitives = vertices;
                    break;
                case PrimitiveType.TriangleList:
                    primitives = vertices / 3;
                    break;
                case PrimitiveType.TriangleFan:
                case PrimitiveType.TriangleStrip:
                    primitives = vertices - 2;
                    break;
            }

            int vertexCount = 0;

            if (indices != null)
                vertexCount = indices.MaxIndex + 1;
            else
            {
                switch (primitiveType)
                {
                    case PrimitiveType.LineStrip:
                        vertexCount = primitives * 2;
                        break;
                    case PrimitiveType.PointList:
                    case PrimitiveType.LineList:
                    case PrimitiveType.TriangleList:
                        vertexCount = vertices;
                        break;
                    case PrimitiveType.TriangleFan:
                    case PrimitiveType.TriangleStrip:
                        vertexCount = primitives * 3;
                        break;
                }
            }

            state.ApplyRenderStateChanges(vertexCount);

            if (indices != null)
                Context.DrawIndexedPrimitive(primitiveType, 0, indices.MinIndex, (indices.MaxIndex - indices.MinIndex) + 1, 0, primitives);
            else
                Context.DrawPrimitives(primitiveType, 0, primitives);

#if DEBUG
            switch (primitiveType)
            {
                case PrimitiveType.LineList:
                case PrimitiveType.LineStrip:
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.LinesDrawn, primitives);
                    break;
                case PrimitiveType.PointList:
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.PointsDrawn, primitives);
                    break;
                case PrimitiveType.TriangleList:
                case PrimitiveType.TriangleFan:
                case PrimitiveType.TriangleStrip:
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.TrianglesDrawn, primitives);
                    break;
            }
#endif
        }

        public void Draw(DrawState state, IIndices indices, PrimitiveType primitiveType, int primitveCount, int startIndex, int vertexOffset)
        {
            ValidateDisposed();

            IDeviceIndexBuffer devib = (IDeviceIndexBuffer)indices;
            IndexBuffer ib = null;

            if (devib != null)
                ib = devib.GetIndexBuffer(state);

            if (_vertexDeclaration == null)
                _vertexDeclaration = Context.VertexDeclarationBuilder.GetDeclaration<TVertexType>();

            VertexBuffer vb = ((IDeviceVertexBuffer)this).GetVertexBuffer(state);
            VertexDeclaration vd = _vertexDeclaration;
            
            state.VertexDeclaration = vd;
            state.IndexBuffer = ib;
            state.SetStream(0, vb, 0, _buffer.Stride);

            int vertices = _buffer.Count;

            if (indices != null)
                vertices = indices.Count;

            int primitives = 0;

            switch (primitiveType)
            {
                case PrimitiveType.LineList:
                    primitives = vertices / 2;
                    break;
                case PrimitiveType.LineStrip:
                    primitives = vertices - 1;
                    break;
                case PrimitiveType.PointList:
                    primitives = vertices;
                    break;
                case PrimitiveType.TriangleList:
                    primitives = vertices / 3;
                    break;
                case PrimitiveType.TriangleFan:
                case PrimitiveType.TriangleStrip:
                    primitives = vertices - 2;
                    break;
            }

            int vertexCount = 0;

            if (indices != null)
                vertexCount = indices.MaxIndex + 1;
            else
            {
                switch (primitiveType)
                {
                    case PrimitiveType.LineStrip:
                        vertexCount = primitives * 2;
                        break;
                    case PrimitiveType.PointList:
                    case PrimitiveType.LineList:
                    case PrimitiveType.TriangleList:
                        vertexCount = vertices;
                        break;
                    case PrimitiveType.TriangleFan:
                    case PrimitiveType.TriangleStrip:
                        vertexCount = primitives * 3;
                        break;
                }
            }

            state.ApplyRenderStateChanges(vertexCount);

            if (primitveCount > primitives ||
                primitveCount <= 0)
                throw new ArgumentException("primitiveCount");

            if (indices != null)
                Context.DrawIndexedPrimitive(primitiveType, vertexOffset, indices.MinIndex, (indices.MaxIndex - indices.MinIndex) + 1 - vertexOffset, startIndex, primitveCount);
            else
                Context.DrawPrimitives(primitiveType, vertexOffset, primitveCount);

#if DEBUG
            switch (primitiveType)
            {
                case PrimitiveType.LineList:
                case PrimitiveType.LineStrip:
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.LinesDrawn, primitives);
                    break;
                case PrimitiveType.PointList:
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.PointsDrawn, primitives);
                    break;
                case PrimitiveType.TriangleList:
                case PrimitiveType.TriangleFan:
                case PrimitiveType.TriangleStrip:
                    Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.TrianglesDrawn, primitives);
                    break;
            }
#endif
        }

		protected override void Dispose(bool disposing)
        {
            if (_buffer != null)
            {
                _buffer.Dispose();
                _buffer = null;
            }

            if (_vertexBuffer != null)
            {
                _vertexBuffer.Dispose();
                _vertexBuffer = null;
            }

            _vertexDeclaration = null;
            GC.SuppressFinalize(this);
        }
        
        private void ValidateDirty()
        {
            //if ((_usage & Usage.Dynamic) == 0)
            //    throw new InvalidOperationException("Usage lacks Usage.Dynamic flag");
        }

        private void ValidateDisposed()
        {
            if (_buffer == null)
                throw new ObjectDisposedException("this");
        }

        public bool IsImplementationUserSpecifiedVertexElements(out VertexElement[] elements)
        {
            elements = _buffer.GetVertexElements();
            return _buffer.IsRawDataVertices;
        }

        private sealed class Implementation : Buffer<TVertexType>
        {
            readonly VertexElement[] _elements;

            internal bool SequentialWriteFlag;

            public Implementation(IEnumerable<TVertexType> vertices)
                : base(vertices) { }

            public Implementation(TVertexType[] data, VertexElement[] elements)
                : base(data)
            {
                _elements = elements;
            }

            public bool IsRawDataVertices
            {
                get { return _elements != null; }
            }

            protected override void WriteBlock(DrawState state, TVertexType[] data, int startIndex, int start, int length, object target)
            {
                VertexBuffer buffer = (VertexBuffer)target;

                state.UnbindBuffer(buffer);
#if DEBUG
                state.Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.VertexBufferByesCopied, Stride * length);
#endif
                DataStream stream = buffer.Lock(startIndex * Stride, length * Stride, LockFlags.None);
                stream.WriteRange(data);
                buffer.Unlock();
            }

            protected override void WriteComplete()
            {
            }

            public VertexElement[] GetVertexElements()
            {
                return _elements;
            }
        }

        internal override void WarmOverride(DrawState state)
        {
            if (_vertexBuffer == null)
                ((IDeviceVertexBuffer)this).GetVertexBuffer(state);
            if (_vertexDeclaration == null)
                ((IDeviceVertexBuffer)this).GetVertexDeclaration(state.Context);
        }

        internal override bool IsDisposed
        {
            get { return _vertexBuffer == null; }
        }
    }
}
