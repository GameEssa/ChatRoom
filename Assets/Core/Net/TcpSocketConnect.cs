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

		private Dictionary<string, Func<Pack, bool>> handles = new Dictionary<string, Func<Pack, bool>>();

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

		public void Connect()
		{
			var endPoint = new IPEndPoint(IPAddress.Parse(_ip), _port);
			var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			socket.NoDelay = true;
			socket.BeginConnect(endPoint, ConnectCallback, socket );
			_socket = socket;
		}

		private void ConnectCallback( IAsyncResult result )
		{
			_socket.EndConnect(result);
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

		public void Disconnect()
		{
			try
			{	
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

		public void DoUpdate()
		{
			if ( _socket == null || _stream == null || !_socket.Connected)
			{
				return;
			}

			while ( true )
			{

				if ( _stream.DataAvailable == false || _stream.CanRead == false )
				{
					return;
				}

				if ( _readPack == null )
				{
					var count = _stream.Read( _reveives, _lengthPos, 4 - _lengthPos );
					_lengthPos += count;
					if (_lengthPos >= 4)
					{
						//big to little
						Array.Reverse( _reveives );
						var size = BitConverter.ToInt32( _reveives, 0 );
						_readPack = new Pack( size - sizeof(Int32) );
						_lengthPos = 0;
					}
					else
					{
						//current can not create a pack wait next packet
						break;
					}
				}

				if ( _readPack != null )
				{
					var complete = _readPack.Read( _stream );
					if ( complete == true )
					{
						this.TryPackToHandle( _readPack );
						_readPack = null;
					}
					else
					{
						//current can not create a pack wait next packet
						break;
					}
				}
			}
		}

		public void RegisterHandle( string tag, Func<Pack, bool> func )
		{
			if ( handles.ContainsKey(tag) )
			{
				Debug.LogWarning( "registerHandle twice" );
				return;
			}

			handles.Add( tag, func );
		}

		public void UnRegisterHandle( string tag )
		{
			handles.Remove( tag );
		}

		public void SendMessage( string tag, Google.Protobuf.IMessage message )
		{
			if ( string.IsNullOrEmpty(tag) )
			{
				return;
			}

			var tagBytes = Encoding.UTF8.GetBytes(tag);
			var tagLength = tagBytes.Length;

			var length = 0;
			byte[] bytes = null;

			if ( message != null )
			{
				length = message.CalculateSize();
				bytes = message.ToByteArray();
			}

			var totalLength = length + tagLength + 2 * sizeof( Int32 ) ;

			_pipe.Clear();
			_pipe.Put( totalLength );

			_pipe.Put( tagLength );
			_pipe.Put( tag );

			if (bytes != null)
			{
				_pipe.Put(bytes);
			}

			try
			{
				_stream.Write(_pipe.cacheBytes, 0, _pipe.position);
			}
			catch (Exception e)
			{
				throw new Exception($" Send message to server error :\n { e.StackTrace }");
			}
		}

		private bool TryPackToHandle( Pack pack )
		{
			pack.Parse();

			if (string.IsNullOrEmpty(pack.tag))
			{
				return false;
			}

			if (handles.TryGetValue(pack.tag, out var func) == false)
			{
				return false;
			}

			return func.Invoke(pack);
		}
	}
}