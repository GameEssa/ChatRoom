using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Configuration;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Core.Net;
using Core.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestSocketConnect : MonoBehaviour
{
	private TcpSocketConnect tcpConnect ;
    // Start is called before the first frame update
    private Socket socket;

	private void Awake()
	{

	}

	private void AcceptCallback( IAsyncResult result )
	{
		socket = result.AsyncState as Socket;
	}

	void Start()
    {
	    tcpConnect = new TcpSocketConnect( "127.0.0.1", 8101 );
	    tcpConnect.Connect();
    }

    public void Send( InputAction.CallbackContext context )
    {
	    if ( tcpConnect.isConnect )
	    {
		    var msg = BytesConvert.GetBytes( "Hello World" );
		    tcpConnect.SendMessage( "Essa", msg);
	    }
    }
}
