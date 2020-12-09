using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace Core.Net
{
	public abstract class SocketConnect : IConnect
	{
		private Socket _socket;
		private NetworkStream _stream;

		private bool _connect = false;

		public bool isConnect
		{
			get { return  _connect; }
		}

		protected void InitialConnect( Socket socket )
		{
			if ( socket == null )
			{
				Debug.LogError( "try use empty socket to initial connect stream" );
			}

			_socket = socket;

			try
			{
				_stream = new NetworkStream( _socket );
				_connect = true;
			}
			catch
			{
				throw new Exception( "create network error" );
			}
		}

		public abstract bool HandleStream();

		public void Close()
		{
			_connect = false;
			try
			{
				if ( _socket.Available > 0 )
				{
					var pack = this.HandleStream();
					while ( pack )
					{
						pack = this.HandleStream();
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
				throw new Exception( "close socket connect error" );
			}
		}

		public void Connect()
		{

		}
	}
}
