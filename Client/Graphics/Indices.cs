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
    public sealed class Indices<TIndexType> : DeviceResource, IIndices, IDeviceIndexBuffer
        where TIndexType : struct
    {
        private Implementation _buffer;
        private IndexBuffer _indexBuffer;
        private Usage _usage;

        public int MinIndex
        {
            get
            { 
                ValidateState(); 
                return _buffer.i_min; 
            }
        }

        public int MaxIndex
        {
            get
            {
                ValidateState(); 
                return _buffer.i_max;
            }
        }

        public bool Is16bit
        {
            get
            {
                ValidateDisposed();
                return _buffer.Stride == 2;
            }
        }

        public bool IsSigned
        {
            get
            {
                ValidateDisposed();
                return !(_buffer.Type == typeof(uint) || _buffer.Type == typeof(ushort));
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

        public Usage Usage
        {
            get { return _usage; }
            set { _usage = value; }
        }

        public Usage ResourceUsage
        {
            get { return _usage; }
            set
            {
                if (_indexBuffer != null)
                    throw new InvalidOperationException("Cannot set Usage when resource is in use");

                _usage = value;
                _buffer.sequentialWriteFlag = (value & Usage.Dynamic) == Usage.Dynamic;
            }
        }

        internal Type BufferIndexType
        {
            get { return typeof(TIndexType); }
        }

        internal override ResourceType ResourceType
        {
            get { return ResourceType.IndexBuffer; }
        }

        internal override bool InUse
        {
            get { return _buffer != null && _indexBuffer != null; }
        }

        internal override bool IsDisposed
        {
            get { return _buffer == null && _indexBuffer == null; }
        }

        public Indices(DeviceContext context, params TIndexType[] indices)
            : this(context, Usage.None, (IEnumerable<TIndexType>)indices) { }

        public Indices(DeviceContext context, IEnumerable<TIndexType> indices)
            : this(context, Usage.None, indices) { }
        
        private Indices(DeviceContext context, Usage usage, IEnumerable<TIndexType> indices)
            : base(context)
        {
            Type indexType = typeof(TIndexType);

            if (indexType != typeof(Int16) &&
                indexType != typeof(UInt16) &&
                indexType != typeof(Int32) &&
                indexType != typeof(UInt32))
                throw new ArgumentException("DataType for IndexBuffer<> must be of type int,uint,short or ushort");

            _usage = usage;
            _buffer = new Implementation(indices);

            SetDirty();
        }

        ~Indices()
        {
            Dispose();
        }

        private void ValidateDirty()
        {
            //if ((_usage & Usage.Dynamic) == 0)
            //    throw new InvalidOperationException("ResourceUsage lacks ResourceUsage.Dynamic flag");
        }

        private void ValidateDisposed()
        {
            if (_buffer == null)
                throw new ObjectDisposedException("this");
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

		protected override void Dispose(bool disposing)
        {
            if (_buffer != null)
            {
                _buffer.Dispose();
                _buffer = null;
            }

            if (_indexBuffer != null)
            {
                _indexBuffer.Dispose();
                _indexBuffer = null;
            }

            GC.SuppressFinalize(this);
        }

        public IndexBuffer GetIndexBuffer(DrawState state)
        {
            ValidateDisposed();

            if (_indexBuffer == null)
            {
                int size = 32;

                if (_buffer.CountKnown)
                    size = _buffer.Count;

                Type indexType = BufferIndexType;
                
                if (size == 0)
                    throw new ArgumentException(string.Format("Indices<{0}> data size is zero", typeof(TIndexType).Name));

                _indexBuffer = new IndexBuffer(Context, _buffer.Stride * size, Usage.WriteOnly, Pool.Managed, Is16bit);
            }
            
            if (_buffer.IsDirty)
                _buffer.UpdateDirtyRegions(state, _indexBuffer);

            return _indexBuffer;
        }

        internal override int GetAllocatedDeviceBytes()
        {
            if (_indexBuffer != null)
                return _indexBuffer.Description.Size;
            return 0;
        }

        internal override int GetAllocatedManagedBytes()
        {
            int count = _buffer != null ? _buffer.GetCount() : 0;
            return Math.Max(0, count);
        }

        private void ValidateState()
        {
            ValidateDisposed();

            if (!_buffer.complete)
                throw new ArgumentException("Value is not available until buffer has been processed");
        }

        internal sealed class Implementation : Buffer<TIndexType>
        {
            internal bool sequentialWriteFlag;
            static IndexBufferProcessor.ProcessMinMax processor = IndexBufferProcessor.Method<TIndexType>();

            public TIndexType min = default(TIndexType), max = default(TIndexType);
            public int i_min, i_max;
            public bool complete;

            public Implementation(IEnumerable<TIndexType> vertices)
                : base(vertices)
            {
                IndexBufferProcessor.Init(this);
            }

            protected override void WriteBlock(DrawState state, TIndexType[] data, int startIndex, int start, int length, object target)
            {
                IndexBuffer buffer = (IndexBuffer)target;

                state.UnbindBuffer(buffer);

                if (start == 0 && length == ((IndexBuffer)target).Description.Size)
                {
                    min = default(TIndexType);
                    max = default(TIndexType);
                }

                processor(data, length, this);
#if DEBUG
                state.Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.IndexBufferBytesCopied, Stride * length);
#endif
                DataStream stream = buffer.Lock(startIndex * Stride, length * Stride, LockFlags.None);
                stream.WriteRange(data);
                buffer.Unlock();
            }
            protected override void WriteComplete()
            {
                IndexBufferProcessor.Update(this, out i_min, out i_max);
                complete = true;
            }
        }

        internal override void WarmOverride(DrawState state)
        {
            if (_indexBuffer == null)
                ((IDeviceIndexBuffer)this).GetIndexBuffer(state);
        }
    }
}
