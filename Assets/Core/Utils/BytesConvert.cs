using System;
using System.IO;
using System.Text;


namespace Core.Utils
{
	public static class BytesConvert
	{
		public static byte[] GetBytes( Int32  number )
		{
			using ( var memoryStream = new MemoryStream() )
			{
				using ( var writer = new BinaryWriter(memoryStream) )
				{
					writer.Write( number );
					return memoryStream.GetBuffer();
				}
			}
		}

		public static byte[] GetBytes( string str )
		{
			return Encoding.UTF8.GetBytes( str );
		}
	}
}