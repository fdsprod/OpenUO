#region License Header
/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
 #endregion

using System.Collections.Generic;

namespace OpenUO.Ultima.Network
{
	public class BufferPool
	{
		private static List<BufferPool> _pools = new List<BufferPool>();

		public static List<BufferPool> Pools{ get{ return _pools; } set{ _pools = value; } }

		private string _name;

		private int _initialCapacity;
		private int _bufferSize;

		private int _misses;

		private Queue<byte[]> _freeBuffers;

		public void GetInfo( out string name, out int freeCount, out int initialCapacity, out int currentCapacity, out int bufferSize, out int misses )
		{
			lock ( this )
			{
				name = _name;
				freeCount = _freeBuffers.Count;
				initialCapacity = _initialCapacity;
				currentCapacity = _initialCapacity * (1 + _misses);
				bufferSize = _bufferSize;
				misses = _misses;
			}
		}

		public BufferPool( string name, int initialCapacity, int bufferSize )
		{
			_name = name;

			_initialCapacity = initialCapacity;
			_bufferSize = bufferSize;

			_freeBuffers = new Queue<byte[]>( initialCapacity );

			for ( int i = 0; i < initialCapacity; ++i )
				_freeBuffers.Enqueue( new byte[bufferSize] );

			lock ( _pools )
				_pools.Add( this );
		}

		public byte[] AcquireBuffer()
		{
			lock ( this )
			{
				if ( _freeBuffers.Count > 0 )
					return _freeBuffers.Dequeue();

				++_misses;

				for ( int i = 0; i < _initialCapacity; ++i )
					_freeBuffers.Enqueue( new byte[_bufferSize] );

				return _freeBuffers.Dequeue();
			}
		}

		public void ReleaseBuffer( byte[] buffer )
		{
			if ( buffer == null )
				return;

			lock ( this )
				_freeBuffers.Enqueue( buffer );
		}

		public void Free()
		{
			lock ( _pools )
				_pools.Remove( this );
		}
	}
}