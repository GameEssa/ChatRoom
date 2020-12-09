using UnityEngine;
using Core.Net;

namespace Core
{
	public class GetRoomsCommand : NetCommand
	{
		private static readonly string _key = "ChatLobby.GetRooms";

		public string key
		{
			get { return _key; }
		}

		public override void Handle( IPack pack )
		{
			Debug.Log( "GetMessage" );
		}

		public override void Release()
		{
			
		}
	}
}
