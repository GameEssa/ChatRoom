using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Net
{
	public abstract class Pack : IPack
	{
		public byte[] data { get; private set; }

		public byte[] origin { get; private set; }

		public string tag { get; private set; }

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
	}
}

