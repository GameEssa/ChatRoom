using System;
using System.Net.Sockets;
using System.Text;
using Core.Net;
using Data.Message;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestSocketConnect : MonoBehaviour
{
	private TcpSocketConnect tcpConnect ;
    // Start is called before the first frame update
    private Socket socket;

    private bool send = false;

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
	    if ( tcpConnect.isConnect && send == false)
	    {
			//var msg = BytesConvert.GetBytes( "Hello World" );
			//tcpConnect.SendMessage( "Essa", msg);

			byte[] bytes = Encoding.UTF8.GetBytes("中文");
			var myString = Encoding.UTF8.GetString(bytes);
			Data.Message.Dialogue dilDialogue = new Dialogue()
			{
				Tag = "send_dialogue",
				Sender = "Essa",
				//SenderTime = Timestamp.FromDateTime(DateTime.UtcNow),
				Message = myString
			};
			tcpConnect.SendMessage( "dialogue", dilDialogue );
			send = true;

	    }


    }

	private void Update()
	{
		tcpConnect.DoUpdate();
	}
}
