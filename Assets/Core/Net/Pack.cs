using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace Core.Net
{
	public class Pack : IPack
	{
		private int _length = 0;

		private int _currentReadPos = 0;

		public byte[] data { get; private set; }

		public byte[] origin { get; private set; }

		public string tag { get; private set; }

		public Pack( int length )
		{
			_length = length;
			origin = new byte[length];
		}

		public void Parse( byte[] bytes )
		{
			if ( bytes == null )
			{
				return;
			}

			var size = sizeof( Int32 );

			if (bytes.Length < 2 * size)
			{
				throw new Exception( "build pack with bytes length less than 8 { head length + tag length }" );
			}

			this.origin = bytes;

			var length = BitConverter.ToInt32( bytes, 0 );
			var tagLength = BitConverter.ToInt32( bytes, size );

			Array.Copy( origin, 2 * size + tagLength , data, 0 , length - (2 * size + tagLength));

			tag = BitConverter.ToString( bytes, 2 * size , tagLength );
		}

		public void Parse()
		{
			if ( origin.Length < 4 )
			{
				Debug.LogError( "current pack length < 4" );
				return;
			}

			this.Parse( origin );
		}

		public bool Read( NetworkStream stream )
		{
			var count = stream.Read( origin, _currentReadPos, _length - _currentReadPos );
			_currentReadPos += count;

			return _currentReadPos == _length;
		}
	}
}

