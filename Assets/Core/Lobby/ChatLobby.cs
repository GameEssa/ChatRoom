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

		private string _loginTag = "Net.Message.Login";

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
		private Transform _roomContainer = null;

		[SerializeField]
		private RoomButton _roomButtonPrefab = null;

		[SerializeField]
		private Button _exitRoomButton = null;

		[SerializeField]
		private Transform _dialogueContainer = null;

		[SerializeField]
		private UguiDialogue _dialoguePrefab = null;

		[SerializeField]
		private Button _sendDialogueButton = null;

		[SerializeField]
		private TMPro.TMP_InputField _dialogueInputField = null;

		[SerializeField]
		private Button _appQuitButton = null;

		public void Login()
		{
			_id = "UnKnown";
			if ( _idInputField != null )
			{
				_id = _idInputField.text;
			}

			User user = new User() { UserId = _id };

			_server.RegisterHandle( _loginTag, this.LoginCallback );
			_server.SendMessage( _loginTag, user );
		}

		public void GetAllRoom()
		{
			_refreshRoomButton.interactable = false;

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
			_server.RegisterHandle( _exitRoomTag, this.ExitRoomCallback );

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

		private bool LoginCallback( Pack pack )
		{
			if (pack.tag != _loginTag )
			{
				return false;
			}

			MessageRespond respond = null;
			if ( pack.data != null && pack.data.Length > 0 )
			{
				respond = MessageRespond.Parser.ParseFrom( pack.data );
			}

			if ( respond != null && respond.RespondState == true )
			{
				_loginTransform.gameObject.SetActive( false );
				_lobbyTransform.gameObject.SetActive( true );
				_roomTransform.gameObject.SetActive( false );
				this.GetAllRoom();
				return true;
			}
			else
			{
				return false;
			}
		}

		private bool GetAllRoomCallback( Pack pack )
		{
			if ( pack.tag != _getAllRoomTag )
			{
				return false;
			}

			_refreshRoomButton.interactable = true;
			_server.UnRegisterHandle( _getAllRoomTag );

			RoomList rooms = null;
			if ( pack.data != null && pack.data.Length > 0 )
			{
				rooms = RoomList.Parser.ParseFrom( pack.data );
			}

			if ( rooms != null )
			{
				_roomList = rooms;
				this.RefreshRoom();
				return true;
			}
			else
			{
				return false;
			}
		}

		private bool CreateRoomCallback( Pack pack )
		{
			if( pack.tag != _createRoomTag )
			{
				return false;
			}
			_server.UnRegisterHandle( _createRoomTag );

			Room room = null;
			if ( pack.data != null && pack.data.Length > 0 )
			{
				room = Room.Parser.ParseFrom( pack.data );
			}

			if ( room != null )
			{
				this.GetAllRoom();
				return true;
			}
			else
			{
				Debug.LogWarning( "Already have named room" );
				return false;
			}
			
		}

		private bool EnterRoomCallback( Pack pack )
		{
			if ( pack.tag != _enterRoomTag )
			{
				return false;
			}

			Room room = null;
			if ( pack.data != null )
			{
				room = Room.Parser.ParseFrom( pack.data );
			}

			_server.UnRegisterHandle( _enterRoomTag );
			if ( room != null )
			{
				if( room.Participants.Contains(_id) == false )
				{
					return false;
				}

				//Enter Success
				_loginTransform.gameObject.SetActive( false );
				_lobbyTransform.gameObject.SetActive( false );
				_roomTransform.gameObject.SetActive( true );
				_room = room;
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

			MessageRespond respond = null;
			if( pack.data != null && pack.data.Length > 0 )
			{
				respond = MessageRespond.Parser.ParseFrom( pack.data );
			}

			if ( respond != null && respond.RespondState == true )
			{
				_room = null;
				_loginTransform.gameObject.SetActive( false );
				_lobbyTransform.gameObject.SetActive( true );
				_roomTransform.gameObject.SetActive( false );
				_dialogueContainer.ClearChild();
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
			_server.UnRegisterHandle( _sendDialogueTag );

			MessageRespond respond = null;
			if ( pack.data != null && pack.data.Length > 0 )
			{
				respond = MessageRespond.Parser.ParseFrom( pack.data );
			}

			if ( respond != null && respond.RespondState == true )
			{
				Debug.Log( "发送成功" );
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

			Dialogue dialogue = null;
			if ( pack.data != null && pack.data.Length > 0 )
			{
				dialogue = Dialogue.Parser.ParseFrom( pack.data );
			}

			if ( dialogue != null )
			{
				//Show dialogue
				UguiDialogue uiDialogue = GameObject.Instantiate<UguiDialogue>( _dialoguePrefab, _dialogueContainer );
				uiDialogue.Initialize( dialogue.Sender, dialogue.Message );
				return true;
			}
			else
			{
				return false;
			}
		}

		private void RefreshRoom()
		{
			if ( _roomContainer == null || _roomButtonPrefab == null )
			{
				Debug.LogError( "rooms container is missing or prefab is missing" );
				return;
			}

			_roomContainer.ClearChild();

			foreach ( var room in _roomList.Rooms )
			{
				var roomButton = GameObject.Instantiate<RoomButton>( _roomButtonPrefab, _roomContainer );
				roomButton.Refresh( room, this.OnTapEnterRoomButton );
			}
		}

		private void OnTapEnterRoomButton(string roomName)
		{
			Room fakeRoom = new Room()
			{
				Name = roomName,
				Owner = _id,
			};
			this.EnterRoom( fakeRoom );
		}

		private void OnTapExitRoomButton()
		{
			Room fakeRoom = new Room()
			{
				Name = _room.Name,
				Owner = _id,
			};

			this.ExitRoom( fakeRoom );
		}

		private void OnTapSendDialogueButton()
		{
			if ( _dialogueInputField != null )
			{
				this.SendDialogue( _dialogueInputField.text );
				_dialogueInputField.text = null;
			}
		}

		private void OnTapAppQuitButton()
		{
			UnityEngine.Application.Quit();
		}

		private void ConnectServer()
		{
			_server = new TcpSocketConnect( "127.0.0.1", 8101 );
			StartCoroutine( "CorConnect" );
			_server.Connect();
		}

		private void ConnectServerCallback()
		{
			_loginTransform.gameObject.SetActive( true );
			_lobbyTransform.gameObject.SetActive( false );
			_roomTransform.gameObject.SetActive( false );


			if ( _connectbButton != null )
			{
				_connectbButton.onClick.AddListener( this.Login );
			}

			if ( _createRoomButton != null )
			{
				_createRoomButton.onClick.AddListener( this.CreateRoom );
			}

			if ( _refreshRoomButton != null )
			{
				_refreshRoomButton.onClick.AddListener( this.GetAllRoom );
			}

			if ( _exitRoomButton != null )
			{
				_exitRoomButton.onClick.AddListener( this.OnTapExitRoomButton );
			}

			if ( _sendDialogueButton != null )
			{
				_sendDialogueButton.onClick.AddListener( this.OnTapSendDialogueButton );
			}

			if ( _appQuitButton != null )
			{
				_appQuitButton.onClick.AddListener( this.OnTapAppQuitButton );
			}
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
			this.ConnectServer();
		}

		private void Update()
		{
			if ( _server != null && _server.isConnect == true )
			{
				_server.DoUpdate();
			}
		}

		private void OnApplicationQuit()
		{
			_server.Disconnect();
		}
	}
}