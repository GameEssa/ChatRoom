using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class TestSocket : MonoBehaviour
{
	private IPAddress _ipAddress;
	private IPEndPoint _port;
	private Socket _socket;
	private void Awake()
	{
		_ipAddress = IPAddress.Parse("127.0.0.1");

		_port = new IPEndPoint( _ipAddress, 8099 );
		Connect();
	}

	void Connect()
	{
		var socket = new Socket( AddressFamily.InterNetwork, SocketType.Stream , ProtocolType.Tcp );
		socket.BeginConnect( _port, ConnectCallback , socket);
	}

	void ConnectCallback( IAsyncResult result )
	{
		_socket = result.AsyncState as Socket;
		Debug.Log( "Connect" );
	}
}
