using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Core.Utils;
using UnityEngine;

namespace Core.Net
{
	public class MessageConvert
	{
		private byte[] _cacheBytes = new byte[8 * 1024];

		private int _position = 0;

		public byte[] cacheBytes
		{
			get
			{
				return  _cacheBytes;
			}
		}

		public int position
		{
			get { return _position; }
		}

		public void Put( Int32 number )
		{
			var toLength = sizeof( Int32 ) + _position;

			if ( toLength > _cacheBytes.Length )
			{
				Debug.LogError( "current pack message is over max size" );
				return;
			}

			var numberBytes = BytesConvert.GetBytes( number );

			Array.Copy( numberBytes, 0, _cacheBytes, _position, sizeof( Int32 ) );
			_position = toLength;
		}

		public void Put( string str )
		{
			if ( string.IsNullOrEmpty( str ) )
			{
				return;
			}

			var strBytes = BytesConvert.GetBytes( str );
			var toLength = strBytes.Length + _position;

			if ( toLength > _cacheBytes.Length )
			{
				Debug.LogError("current pack message is over max size");
				return;
			}

			Array.Copy( strBytes, 0, _cacheBytes, _position, strBytes.Length );
			_position = toLength;
		}

		public void Put( byte[] bytes )
		{
			if ( bytes == null )
			{
				return;
			}

			var toLength = bytes.Length + _position;

			if (toLength > _cacheBytes.Length)
			{
				Debug.LogError("current pack message is over max size");
				return;
			}

			Array.Copy( bytes, 0, _cacheBytes, _position, bytes.Length );
			_position = toLength;
		}

		public void Clear()
		{
			_position = 0;
		}
	}
}