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

using System;

namespace OpenUO.Ultima.Network
{
    /// <summary>
    /// Gets the Byte Queue Packet Information as well Data
    /// </summary>
	public class ByteQueue
	{
		private int _head;
		private int _tail;
		private int _size;

		private byte[] _buffer;

        /// <summary>
        /// The length of bytes in the Packet
        /// </summary>
		public int Length{ get{ return _size; } }

        /// <summary>
        /// Creates a new Instance of ByteQueue
        /// </summary>
		public ByteQueue()
		{
            _buffer = new byte[0x800];
		}

        /// <summary>
        /// Clear the Byte Queue
        /// </summary>
		public void Clear()
		{
			_head = 0;
			_tail = 0;
			_size = 0;
		}

        /// <summary>
        /// Set the Capacity for the ByteQueue
        /// </summary>
        /// <param name="capacity">the capacity</param>
		private void SetCapacity( int capacity ) 
		{
			byte[] newBuffer = new byte[capacity];

			if ( _size > 0 )
			{
				if ( _head < _tail )
				{
					Buffer.BlockCopy( _buffer, _head, newBuffer, 0, _size );
				}
				else
				{
					Buffer.BlockCopy( _buffer, _head, newBuffer, 0, _buffer.Length - _head );
					Buffer.BlockCopy( _buffer, 0, newBuffer, _buffer.Length - _head, _tail );
				}
			}

			_head = 0;
			_tail = _size;
			_buffer = newBuffer;
		}

		public byte GetPacketID()
		{
            if ( _size >= 1 )
            {
				return _buffer[_head];
            }

			return 0xff;
		}

		public int GetPacketLength()
		{
			if ( _size >= 5 )
            {
                return (_buffer[(_head + 1) % _buffer.Length] << 8) | (_buffer[(_head + 2) % _buffer.Length]);
			}

			return 0;
		}

		public int Dequeue( byte[] buffer, int offset, int size )
		{
			if ( size > _size )
				size = _size;

			if ( size == 0 )
				return 0;

			if ( _head < _tail )
			{
				Buffer.BlockCopy( _buffer, _head, buffer, offset, size );
			}
			else
			{
				int rightLength = ( _buffer.Length - _head );

				if ( rightLength >= size )
				{
					Buffer.BlockCopy( _buffer, _head, buffer, offset, size );
				}
				else
				{
					Buffer.BlockCopy( _buffer, _head, buffer, offset, rightLength );
					Buffer.BlockCopy( _buffer, 0, buffer, offset + rightLength, size - rightLength );
				}
			}

			_head = ( _head + size ) % _buffer.Length;
			_size -= size;

			if ( _size == 0 )
			{
				_head = 0;
				_tail = 0;
			}

			return size;
		}

		public void Enqueue( byte[] buffer, int offset, int size )
		{
			if ( (_size + size) > _buffer.Length )
                SetCapacity( (_size + size + 0x7FF) & ~0x7FF );

			if ( _head < _tail )
			{
				int rightLength = ( _buffer.Length - _tail );

				if ( rightLength >= size )
				{
					Buffer.BlockCopy( buffer, offset, _buffer, _tail, size );
				}
				else
				{
					Buffer.BlockCopy( buffer, offset, _buffer, _tail, rightLength );
					Buffer.BlockCopy( buffer, offset + rightLength, _buffer, 0, size - rightLength );
				}
			}
			else
			{
				Buffer.BlockCopy( buffer, offset, _buffer, _tail, size );
			}

			_tail = ( _tail + size ) % _buffer.Length;
			_size += size;
		}
	}
}