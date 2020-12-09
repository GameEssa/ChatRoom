using UnityEngine;
using UnityEditor;

namespace Core.Net
{
	public abstract class NetCommand : INetCommand
	{
		private static int _order = 0;

		public int instance { get; protected set; }

		protected NetCommand()
		{
			instance = _order++;
		}

		public abstract void Handle( IPack pack );

		public abstract void Release();
	}
}
