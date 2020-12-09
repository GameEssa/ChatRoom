using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Net;
using UnityEditor;
using UnityEngine;

namespace Core
{
	[CustomEditor( typeof(NetEventButton) )]
	public class NetEventButtonInspector : Editor
	{
		private string[] _types;

		private int _index;

		private NetEventButton _eventButton;

		private void Awake()
		{
			Type commandInterface = typeof(INetCommand);

			var types = commandInterface.Assembly.GetTypes()
				.Where( t => commandInterface.IsAssignableFrom( t ))
				.Where( t => !t.IsAbstract && !t.IsInterface )
				.Select( t=> t.Name  )
				.ToArray();

			_types = types;

			_eventButton = target as NetEventButton;

			var index = -1;
			if ( _eventButton.commandType != null )
			{
				for ( int i = 0; i < _types.Length ; i++ )
				{
					if ( _eventButton.commandType.Name == _types[i] )
					{
						index = i;
					}
				}
			}

			_index = index;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			_index = EditorGUILayout.Popup( "Command", _index < 0 ? 0 : _index , _types );

			_eventButton.commandType = Type.GetType( _types[_index] );
		}
	}
}