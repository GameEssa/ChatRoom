using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent( typeof( Button ) )]
public class RoomButton : MonoBehaviour
{
	[SerializeField]
	private Text _nameText = null;

	[SerializeField]
	private string _nameFormat = null;

	[SerializeField]
	private Text _ownerText = null;

	[SerializeField]
	private string _ownerFormat = null;

	private System.Action<string> _clickCallback = null;

	private string _roomName;

	private Button _button = null;

	public Button ContactButton
	{
		get
		{
			return _button ?? ( _button = GetComponent<Button>() );
		}
	}

	public void Refresh(Data.Message.Room room, System.Action<string> callback)
	{
		if( room == null )
		{
			Debug.LogWarning( "use empty args to refresh room button" );
			this.gameObject.SetActive( false );
			return;
		}

		_clickCallback = callback;
		_roomName = room.Name;

		ContactButton.onClick.AddListener( OnTapButton );

		if ( _nameText != null )
		{
			_nameText.text = string.Format( _nameFormat, room.Name );
		}

		if ( _ownerText != null )
		{
			_ownerText.text = string.Format( _ownerFormat, room.Owner );
		}
	}

	private void OnTapButton()
	{
		if( !string.IsNullOrEmpty(_roomName) && _clickCallback != null )
		{
			_clickCallback.Invoke( _roomName );
		}
	}
}
