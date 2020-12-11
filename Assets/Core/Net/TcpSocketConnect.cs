using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Data.Message;
using Google.Protobuf;

namespace Core.Net
{
	public class TcpSocketConnect : IConnect
	{
		private Socket _socket;

		private NetworkStream _stream;

		private string _ip;

		private int _port;

		private bool _connect = false;

		private PackBytesPipe _pipe = null;

		private byte[] _reveives = new byte[4];

		private int _lengthPos = 0;

		private Pack _readPack = null;

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
			_pipe = new PackBytesPipe();
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

		internal void DoUpdate()
		{
			if ( _socket == null || _stream == null )
			{
				return;
			}

			if ( _stream.DataAvailable == false || _stream.CanRead == false )
			{
				return;
			}

			/*if ( _readPack == null )
			{
				Debug.Log( _socket.Available );
				var count = _stream.Read( _reveives, _lengthPos, 4 - _lengthPos );
				_lengthPos += count;
				if ( _lengthPos >= 4 )
				{

					var size = BitConverter.ToInt32( _reveives , 0 );
					Array.Reverse(_reveives);
					_readPack = new Pack( size );
					_lengthPos = 0;
					Debug.Log(size);
				}
			}*/

			Debug.Log(_socket.Available);
			byte[] read = new byte[_socket.Available];
			var count = _stream.Read(read, 0, _socket.Available);
			var dialogue = Data.Message.Dialogue.Parser.ParseFrom( read );

			Debug.Log( dialogue.ToString() );
			//if ( _readPack != null )
			//{
			//	var complete = _readPack.Read( _stream );

			//	if ( complete == true )
			//	{
			//		this.SampleOutput( _readPack );
			//		_readPack = null;
			//	}

			//}

		}

		private void SampleOutput( Pack pack )
		{
			pack.Parse();
			var msg = Data.Message.Dialogue.Parser.ParseFrom( pack.data );
			Debug.Log( msg.ToString() );
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

			_pipe.Clear();
			_pipe.Put( total );
			_pipe.Put( tagLength );
			_pipe.Put( tagBytes );
			_pipe.Put( bytes );

			Debug.Log( $"{_pipe.position} : buffer write" );
			//_stream.Write( _convert.cacheBytes, 0, _convert.position);

			byte[] buffer = new byte[total];
			Array.Copy( _pipe.cacheBytes, 0, buffer, 0, total );

			_socket.Send( buffer );
		}

		public void SendMessage( Google.Protobuf.IMessage message )
		{
			var length = message.CalculateSize();
			var bytes = message.ToByteArray();
			_pipe.Clear();
			_pipe.Put( length );
			_pipe.Put( bytes );
			try
			{
				_stream.Write(_pipe.cacheBytes, 0, _pipe.position);
			}
			catch (Exception e)
			{
				throw new Exception($" Send message to server error :\n { e.StackTrace }");
			}
		}
	}
}