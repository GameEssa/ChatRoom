using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
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

			if (bytes.Length < size)
			{
				throw new Exception( "build pack with bytes length less than 8 { head length + tag length }" );
			}

			this.origin = bytes;
			byte[] tagBytes = new byte[size];
			Array.Copy( origin, 0, tagBytes, 0, size );
			Array.Reverse( tagBytes );

			var tagLength = BitConverter.ToInt32( tagBytes, 0 );
			var dataLength = _length - (size + tagLength);

			data = new byte[dataLength];
			Array.Copy( origin, size + tagLength , data, 0 , dataLength  );

			//tag = BitConverter.ToString( bytes, size , tagLength );
			tag = Encoding.UTF8.GetString( bytes, size, tagLength );
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

