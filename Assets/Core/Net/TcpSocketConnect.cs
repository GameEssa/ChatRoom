using System;
using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Core.Net;

namespace Core.Net
{
	public class TcpSocketConnect : IConnect
	{
		private Socket _socket;

		private NetworkStream _stream;

		private string _ip;

		private int _port;

		private bool _connect = false;

		private MessageConvert _convert = null;

		public bool isConnect
		{
			get { return _connect; }
		}

		public TcpSocketConnect( string ip, int port )
		{
			if ( string.IsNullOrEmpty( ip ) || port <= 0 )
			{
				Debug.LogError( "ip config settings is invalid" );
				return;
			}

			_ip = ip;
			_port = port;
			_convert = new MessageConvert();
		}

		public void Close()
		{
			try
			{
				if (_socket.Available > 0)
				{
					var pack = this.TryPackToHandle();
					while (pack)
					{
						pack = this.TryPackToHandle();
					}
				}

				_stream.Dispose();
				_stream.Close();

				_socket.Close();

				_stream = null;
				_socket = null;
				_connect = false;
			}
			catch
			{
				throw new Exception("close socket connect error");
			}
		}

		public void Connect()
		{
			var endPoint = new IPEndPoint( IPAddress.Parse( _ip ), _port );
			var socket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
			socket.NoDelay = true;
			socket.BeginConnect( endPoint, ConnectCallback, socket );
			_socket = socket;
		}

		private bool TryPackToHandle()
		{
			return false;
		}

		private void ConnectCallback( IAsyncResult result )
		{
			_socket.EndConnect( result );
			if (_socket == null)
			{
				Debug.LogError("try use empty socket to initial connect stream");
				return;
			}

			try
			{
				_stream = new NetworkStream(_socket);
				_connect = true;
			}
			catch
			{
				throw new Exception("create network error");
			}
		}

		public void SendMessage( string tag, byte[] bytes )
		{

			var tagBytes = Encoding.UTF8.GetBytes( tag );
			var tagLength = tagBytes.Length;

			var total = 2 * sizeof( Int32 ) + tagLength + bytes.Length;

			_convert.Clear();
			_convert.Put( total );
			_convert.Put( tagLength );
			_convert.Put( tagBytes );
			_convert.Put( bytes );

			Debug.Log( $"{_convert.position} : buffer write" );
			//_stream.Write( _convert.cacheBytes, 0, _convert.position);

			byte[] buffer = new byte[total];
			Array.Copy( _convert.cacheBytes, 0, buffer, 0, total );

			_socket.Send( buffer );
		}
	}
}