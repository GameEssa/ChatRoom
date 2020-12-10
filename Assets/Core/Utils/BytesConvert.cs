using System;
using System.IO;
using System.Text;


namespace Core.Utils
{
	public static class BytesConvert
	{
		public static byte[] GetBytes( Int32  number, bool lendian = true )
		{
			byte[] bytes = BitConverter.GetBytes(number);
			if ( BitConverter.IsLittleEndian == true && lendian == false)
			{
				Array.Reverse(bytes, 0, bytes.Length);
			}

			return bytes;
		}



		public static byte[] GetBytes( string str )
		{
			return Encoding.UTF8.GetBytes( str );
		}
	}
}