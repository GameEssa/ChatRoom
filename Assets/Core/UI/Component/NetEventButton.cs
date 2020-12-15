using System;
using Core.Net;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

namespace Core
{
	[RequireComponent(typeof(Button))]
	public class NetEventButton : MonoBehaviour
	{
		public Type commandType;

		private Button _button;

		private bool _transition = false;

		public Button button
		{
			get
			{
				return _button ?? ( _button = GetComponent<Button>() );
			}
		}

		private void Awake()
		{
			this.button.onClick.AddListener( Request );
		}

		private void Request()
		{
			if ( _transition == true )
			{	
				return;
			}

			this.Register();

			_transition = true;
		}

		private void Respond()
		{

			_transition = false;
		}

		private void Register()
		{
			if ( commandType == null )
			{
				Debug.LogError( "last command is processing" );
				return;
			}

			NetManager.instance.RegisterCommand( commandType, Respond );
		}
	}
}

