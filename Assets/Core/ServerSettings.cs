using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
	[CreateAssetMenu( menuName = "Assets/Settings/Server" )]
	public class ServerSettings : ScriptableObject
	{
		[SerializeField]
		private string _ip = null;

		[SerializeField]
		private int _port = 0;

		public string ip
		{
			get { return _ip; }
		}

		public int port
		{
			get { return _port; }
		}
	}
}