using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomButton : MonoBehaviour
{
	[SerializeField]
	private Text _nameText;

	[SerializeField]
	private string _nameFormat;

	[SerializeField]
	private Text _ownerText;

	[SerializeField]
	private string _ownerFormat;


	private string _roomName;

	public void Refresh(Data.Message.Room room)
	{
		if( room == null )
		{
			Debug.LogWarning( "use empty args to refresh room button" );
			this.gameObject.SetActive( false );
			return;
		}

		if ( _nameText != null )
		{
			_nameText.text = string.Format( _nameFormat, room.Name );
		}

		if ( _ownerText != null )
		{
			_ownerText.text = string.Format( _ownerFormat, room.Owner );
		}
	}
}
