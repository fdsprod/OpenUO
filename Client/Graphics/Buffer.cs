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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Client.Graphics
{
    abstract class Buffer<DataType> : IDisposable
        where DataType : struct
    {
        private IEnumerable<DataType> _data;
        private bool _isDisposed;
        private static int _typeStride;
        private int _count = -1, _instanceStride = _typeStride;
        private bool _isList;
        private List<DirtyRange> _dirtyRanges;

        private sealed class GeometryBuffer
        {
            const int BufferSize = 32;
            public static DataType[] Buffer = new DataType[BufferSize];
        }

        static Buffer()
        {
            _typeStride = System.Runtime.InteropServices.Marshal.SizeOf(typeof(DataType));
            ValidateType(typeof(DataType));
        }

        private static void ValidateType(Type t)
        {
            if (t.IsClass)
                throw new ArgumentException(string.Format("Type '{0}' contains non-valuetype member(s)", typeof(DataType)));

            foreach (FieldInfo f in t.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public))
                if (f.FieldType != t)
                    ValidateType(f.FieldType);
        }
        
        private struct DirtyRange
        {
            public int Start, Length;

            public DirtyRange(int start, int length)
            { 
                Length = length;
                Start = start; 
            }

            public bool Merge(DirtyRange range)
            {
                DirtyRange r = this;

                if (range.Start <= Start + Length &&
                    Start <= range.Start + range.Length)
                {
                    int end = Math.Max(Start + Length, range.Start + range.Length);
                    Start = Math.Min(Start, range.Start);
                    Length = end - Start;
                    return true;
                }
                return false;
            }
        }

        public bool IsDirty
        {
            get { return _dirtyRanges != null && _dirtyRanges.Count > 0; }
        }

        public void UpdateDirtyRegions(DrawState state, object target)
        {
            int writeRange = -1;

            foreach (DirtyRange range in _dirtyRanges)
            {
                if ((range.Start == 0 && range.Length == Count) || !_isList)
                    writeRange = Math.Max(writeRange, (range.Length + range.Start) * Stride);
                else
                    WriteBuffer(state, range.Start, range.Length * Stride, target);
            }

            if (writeRange != -1)
                WriteBuffer(state, 0, writeRange, target);

            _dirtyRanges.Clear();
        }

        public void ClearDirtyRange()
        {
            if (_dirtyRanges != null)
                _dirtyRanges.Clear();
        }

        public Buffer(IEnumerable<DataType> data)
        {
            if (data == null)
                throw new ArgumentNullException();

            _data = data;
            _count = GetCount();
        }

        internal int GetCount()
        {
            IEnumerable<DataType> data = _data;

            if (data is ICollection<DataType>)
                return (data as ICollection<DataType>).Count;

            if (data is ICollection)
                return (data as ICollection).Count;

            if (data is Array)
                return (data as Array).Length;

            return -1;
        }

        public bool CountKnown
        {
            get { return _count != -1; }
        }

        public IEnumerable<DataType> Data
        {
            get { return _data; }
        }
        
        public bool Disposed
        {
            get { return _isDisposed; }
        }

        public Type Type
        {
            get { return typeof(DataType); }
        }

        public int Stride
        {
            get { return _instanceStride; }
        }

        public void ClearBuffer()
        {
            _data = null;
        }

        public void Dispose()
        {
            _data = null;
            _isDisposed = true;
        }

        public void AddDirtyRange(int startIndex, int count, Type sourceType, bool fillEntireRange)
        {
            if ((startIndex < 0 || count <= 0) && !fillEntireRange)
                throw new ArgumentException("Invalid range specified");

            IEnumerable<DataType> data = _data;

            //if (data == null)
            //    throw new InvalidOperationException("Cannot set a dirty region when source data has been garbage collected");

            _isList = true;

            if (data is IList == false)
            {
                if (data is ICollection == false || !(startIndex == 0 && count == (data as ICollection).Count && !fillEntireRange))
                {
                    if (!(data is ICollection == false && data is IEnumerable && fillEntireRange))
                        throw new InvalidOperationException(sourceType.Name + "<" + sourceType.GetGenericArguments()[0].Name + "> source data must implement the IList<" + sourceType.GetGenericArguments()[0].Name + "> interface to set a dirty subrange");
                }
                else
                {
                    if (data is ICollection && (startIndex == 0 && count == (data as ICollection).Count))
                        fillEntireRange = true;
                }
                _isList = false;
            }

            IList dataList = data as IList;

            if (!fillEntireRange && ((startIndex + count) > dataList.Count))
                throw new ArgumentException("Invalid range specified");

            if (_dirtyRanges == null)
                _dirtyRanges = new List<DirtyRange>();

            if (fillEntireRange)
            {
                _dirtyRanges.Clear();
                if (data is ICollection)
                    _dirtyRanges.Add(new DirtyRange(0, (data as ICollection).Count));
                else
                    _dirtyRanges.Add(new DirtyRange(0, -1));//special case as length is not yet known
            }
            else
            {
                DirtyRange range = new DirtyRange(startIndex, count);

                for (int i = 0; i < _dirtyRanges.Count; i++)
                    if (_dirtyRanges[i].Merge(range))
                        return;

                _dirtyRanges.Add(range);
            }
        }

        public int Count
        {
            get
            {
                if (_count == -1)
                    throw new ArgumentException("Count is not available until the data has been processed");

                return _count;
            }
        }

        internal void OverrideSize(int stride, int count)
        {
            _count = count;
            _instanceStride = stride;
        }

        protected abstract void WriteBlock(DrawState state, DataType[] data, int sourceStartIndex, int writeOffsetBytes, int copyElements, object target);
        protected abstract void WriteComplete();

        public int WriteBuffer(DrawState state, int startIndex, int bytesToWrite, object target)
        {
            IEnumerable<DataType> array = _data;

            if (array is DataType[])
            {
                DataType[] data = (DataType[])array;
                //	int size = stride * data.Length;
                //	if (size <= maxsize)
                {
                    WriteBlock(state, data, startIndex, startIndex * _instanceStride, bytesToWrite / _instanceStride, target);
                    WriteComplete();
                }
                return bytesToWrite / _instanceStride;
            }

            DataType[] writeBuffer = null;
            //not an array, is the count known?
            if (_count != -1)
            {
                //it is.. try and make a copy first
                try
                {
                    writeBuffer = new DataType[bytesToWrite / _instanceStride];
                    //easy to copy an IList
                    if (startIndex == 0 && writeBuffer.Length == _count && array is IList<DataType>)
                        (array as IList<DataType>).CopyTo(writeBuffer, 0);
                    else
                    {
                        //not so easy
                        int inc = 0;
                        int offset = startIndex;
                        int range = bytesToWrite / _instanceStride;

                        foreach (DataType item in array)
                        {
                            if (offset-- > 0)
                                continue;
                            if (--range < 0)
                                break;
                            writeBuffer[inc++] = item;
                        }
                    }
                }
                catch (OutOfMemoryException)
                {
                    //couldn't allocate big enough copy array, so do it bit by bit
                    writeBuffer = null;
                }

                if (writeBuffer != null)
                {
                    WriteBlock(state, writeBuffer, 0, startIndex * _instanceStride, bytesToWrite / _instanceStride, target);
                    WriteComplete();
                    return bytesToWrite / _instanceStride;
                }
            }

            //copy block by block
            writeBuffer = GeometryBuffer.Buffer;

            lock (writeBuffer)
            {
                int inc = 0;
                int written = 0;
                int offset = startIndex;
                int range = bytesToWrite / _instanceStride;
                int loopCount = 0;

                foreach (DataType item in array)
                {
                    loopCount++;

                    if (offset-- > 0)
                        continue;
                    if (--range < 0)
                    {
                        if (_count == -1)
                            continue;
                        break;
                    }

                    writeBuffer[inc++] = item;

                    if (inc == writeBuffer.Length)
                    {
                        WriteBlock(state, writeBuffer, 0, startIndex * _instanceStride, inc, target);

                        startIndex += inc;
                        written += writeBuffer.Length;
                        inc = 0;
                    }
                }
                if (inc != 0)
                {
                    WriteBlock(state, writeBuffer, 0, startIndex * _instanceStride, inc, target);
                    written += inc;
                }
                if (_count == -1)
                    _count = loopCount;
                WriteComplete();
                return written;
            }
        }


    }
}
