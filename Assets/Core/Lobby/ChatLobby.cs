using System;
using System.Collections;
using System.Collections.Generic;
using Core.Net;
using Data.Message;
using UnityEngine;

namespace Core
{
	public class ChatLobby : MonoBehaviour
	{
		//private SocketConnect _server;

		private TcpSocketConnect _server;

		private RoomList _roomList;

		private string _id;

		private Room _room;

		public void Initialize()
		{
			
		}

		public void Destroy()
		{
			
		}

		private string _getAllRoomTag = "Net.Message.GetAllRoom";

		private string _enterRoomTag = "Net.Message.EnterRoom";

		private string _sendDialogueTag = "Net.Message.SendDialogue";

		private string _receiveDialogueTag = "Net.Message.ReceiveDialogue";

		private string _exitRoomTag = "et.Message.ExitRoom";

		public void GetAllRoom()
		{
			_server.RegisterHandle( _getAllRoomTag, this.GetAllRoomCallback );

			_server.SendMessage( _getAllRoomTag, null );
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
			if ( pack.tag != _getAllRoomTag)
			{
				return false;
			}

			RoomList rooms = RoomList.Parser.ParseFrom( pack.data );
			_server.UnRegisterHandle( _getAllRoomTag );
			if ( rooms != null)
			{
				_roomList = rooms;
				this.RefreshRoom();
				return true;
			}
			
			return false;
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

			Respond respond = Respond.Parser.ParseFrom(pack.data);

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
			if( pack.tag != _sendDialogueTag )
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
			//DoRefresh
		}
	}
}