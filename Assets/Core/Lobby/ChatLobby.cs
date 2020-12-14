using System;
using System.Collections;
using System.Collections.Generic;
using Core.Net;
using Core.Utils;
using Data.Message;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
	public class ChatLobby : MonoBehaviour
	{
		//private SocketConnect _server;

		private TcpSocketConnect _server;

		private RoomList _roomList;

		private string _id;

		private Room _room;

		private string _createRoomTag = "Net.Message.CreateRoom";

		private string _getAllRoomTag = "Net.Message.GetAllRoom";

		private string _enterRoomTag = "Net.Message.EnterRoom";

		private string _exitRoomTag = "Net.Message.ExitRoom";

		private string _sendDialogueTag = "Net.Message.SendDialogue";

		private string _receiveDialogueTag = "Net.Message.ReceiveDialogue";

		[SerializeField]
		private Transform _loginTransform = null;

		[SerializeField]
		private Transform _lobbyTransform = null;

		[SerializeField]
		private Transform _roomTransform = null;

		[SerializeField]
		private Button _connectbButton = null;

		[SerializeField]
		private InputField _idInputField = null;

		[SerializeField]
		private InputField _roomNameInputField = null;

		[SerializeField]
		private Button _createRoomButton = null;

		[SerializeField]
		private Button _refreshRoomButton = null;

		[SerializeField]
		private Transform _roomsContainer = null;

		[SerializeField]
		private RoomButton _roomButtonPrefab = null;

		public void GetAllRoom()
		{
			_server.RegisterHandle( _getAllRoomTag, this.GetAllRoomCallback );

			_server.SendMessage( _getAllRoomTag, null );
		}

		public void CreateRoom()
		{
			var roomName = _id;
			if ( _roomNameInputField != null && !string.IsNullOrEmpty( _roomNameInputField.text) )
			{
				roomName = _roomNameInputField.text;
			}

			_server.RegisterHandle( _createRoomTag, CreateRoomCallback );

			Room room = new Room()
			{
				Name = roomName,
				Owner = _id,
			};

			_server.SendMessage( _createRoomTag, room );
		}

		public void EnterRoom( Room room )
		{
			_server.RegisterHandle( _enterRoomTag, this.EnterRoomCallback );

			_server.SendMessage( _enterRoomTag, room );
		}

		private void ExitRoom( Room room )
		{
			_server.RegisterHandle( _exitRoomTag, this.EnterRoomCallback );

			_server.SendMessage( _exitRoomTag, room );
		}

		public void SendDialogue( string message )
		{
			Dialogue dialogue = new Dialogue()
			{
				Message = message,
				Sender = _id,
				Tag = _room.Name
			};

			_server.RegisterHandle( _sendDialogueTag, SendMessageCallback );
			_server.SendMessage( _sendDialogueTag, dialogue );
		}

		private bool GetAllRoomCallback( Pack pack )
		{
			if ( pack.tag != _getAllRoomTag )
			{
				return false;
			}

			RoomList rooms = RoomList.Parser.ParseFrom( pack.data );
			_server.UnRegisterHandle( _getAllRoomTag );
			if ( rooms != null )
			{
				_roomList = rooms;
				this.RefreshRoom();
				return true;
			}

			return false;
		}

		private bool CreateRoomCallback( Pack pack )
		{
			if( pack.tag != _createRoomTag )
			{
				return false;
			}

			Room room = Room.Parser.ParseFrom( pack.data );
			if ( room != null )
			{
				return true;
			}
			else
			{
				return false;
			}
			
		}

		private bool EnterRoomCallback( Pack pack )
		{
			if ( pack.tag != _enterRoomTag )
			{
				return false;
			}

			Respond respond = Respond.Parser.ParseFrom( pack.data );

			_server.UnRegisterHandle( _enterRoomTag );
			if ( respond.State == State.Ok )
			{
				_server.RegisterHandle( _receiveDialogueTag, this.ReceiveDialogueCallback );
				return true;
			}
			else
			{
				return false;
			}
		}

		private bool ExitRoomCallback( Pack pack )
		{
			if ( pack.tag != _exitRoomTag )
			{
				return false;
			}

			Respond respond = Respond.Parser.ParseFrom( pack.data );

			_server.UnRegisterHandle( _exitRoomTag );
			if ( respond.State == State.Ok )
			{
				_server.UnRegisterHandle( _receiveDialogueTag );
				return true;
			}
			else
			{
				return false;
			}
		}

		private bool SendMessageCallback( Pack pack )
		{
			if ( pack.tag != _sendDialogueTag )
			{
				return false;
			}

			Respond respond = Respond.Parser.ParseFrom( pack.data );
			_server.UnRegisterHandle( _sendDialogueTag );
			if ( respond.State == State.Ok )
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private bool ReceiveDialogueCallback( Pack pack )
		{
			if ( pack.tag != _receiveDialogueTag )
			{
				return false;
			}

			Dialogue dialogue = Dialogue.Parser.ParseFrom( pack.data );

			if ( dialogue != null )
			{
				//Show dialogue
				return true;
			}
			else
			{
				return false;
			}
		}

		private void RefreshRoom()
		{
			if ( _roomsContainer == null || _roomButtonPrefab == null )
			{
				Debug.LogError( "rooms container is missing or prefab is missing" );
				return;
			}

			_roomsContainer.ClearChild();

			foreach ( var room in _roomList.Rooms )
			{
				var roomButton = GameObject.Instantiate<RoomButton>( _roomButtonPrefab, _roomsContainer );
				roomButton.Refresh( room );
			}

		}

		private void ConnectServer()
		{
			_id = "UnKnown";
			if ( _idInputField  != null )
			{
				_id = _idInputField.text;
			}
			_server = new TcpSocketConnect( "127.0.0.1", 8101 );
			StartCoroutine( "CorConnect" );
			_server.Connect();
		}

		private void ConnectServerCallback()
		{
			_loginTransform.gameObject.SetActive( false );
			_lobbyTransform.gameObject.SetActive( true );
			_roomTransform.gameObject.SetActive( false );
		}

		IEnumerator CorConnect()
		{
			while ( _server.isConnect == false )
			{
				yield return null;
			}
			ConnectServerCallback();
		}

		private void Awake()
		{
			if ( _connectbButton != null )
			{
				_connectbButton.onClick.AddListener( this.ConnectServer );
			}

			if ( _createRoomButton != null )
			{
				_createRoomButton.onClick.AddListener( this.CreateRoom );
			}

			if ( _refreshRoomButton != null )
			{
				_refreshRoomButton.onClick.AddListener( this.GetAllRoom );
			}
		}

		private void Update()
		{
			if ( _server != null && _server.isConnect == true )
			{
				_server.DoUpdate();
			}
		}
	}
}