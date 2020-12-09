using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Net
{
	public partial class NetManager
	{
		public static NetManager instance { get; private set; }

		private List<IConnect> _connects = new List<IConnect>();

		private Dictionary<INetCommand, Action> _commands = new Dictionary<INetCommand, Action>();

		public static void InitializeInstance()
		{
			instance = new NetManager();
		}

		public static void DestroyInstance()
		{
			for ( int i = 0; i < instance._connects.Count ; i++ )
			{
				instance.Close( instance._connects[i] );
			}

			instance._connects.Clear();
			instance = null;
		}

		public void ReceivePack( string message, IPack pack )
		{
			if ( string.IsNullOrEmpty( message ) || pack == null )
			{
				Debug.LogError( "receive pack error" );
				return;
			}

			foreach ( var commandPair in _commands)
			{
				if (commandPair.Key.instance == int.Parse(  message ) )
				{
					var command = commandPair.Key;
					var callback = commandPair.Value;

					command.Handle( pack );

					if ( callback != null )
					{
						callback.Invoke();
					}

					this.UnRegisterCommand( command );
				}
			}
		}

		public void RegisterCommand( Type type, Action callback )
		{
			if ( type == null )
			{
				return;
			}

			if ( !type.IsSubclassOf( typeof(INetCommand) ) )
			{
				Debug.LogError( $"this is not a command, {type}" );
				return;
			}

			var command = Pool.ObjectPool.instance.Acquire(type) as INetCommand;

			if ( command == null )
			{
				Debug.LogError( "create command error" );
				return;
			}

			_commands.Add( command, callback );
		}

		public void RegisterCommand( INetCommand command , Action callback )
		{
			if ( command == null )
			{
				return;
			}

			_commands.Add(command, callback);
		}

		public void UnRegisterCommand( INetCommand command )
		{
			if ( _commands.Remove( command ) )
			{
				Pool.ObjectPool.instance.Recycle(command);
			}
		}

		public void Close( IConnect connect )
		 {
			 if ( connect == null )
			 {
				 return;
			 }

			 if ( !_connects.Contains( connect ) )
			 {
				Debug.LogWarning( "use a connect which isn't controlled in NetManager" );
			 }

			 if ( connect.isConnect )
			 {
				 connect.Close();
			 }

			 _connects.Remove( connect );
		 }
	}
}