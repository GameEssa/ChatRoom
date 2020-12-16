using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UguiDialogue : MonoBehaviour
{
	[SerializeField]
	private TMP_Text _senderText = null;

	[SerializeField]
	private TMP_Text _messageText = null;

	[SerializeField]
	private string _senderFormat = null;

	public void Initialize(string sender, string message)
	{
		if ( _senderText != null )
		{
			_senderText.text = string.Format( _senderFormat, sender );
		}

		if ( _messageText != null )
		{
			_messageText.text = message;
		}
	}
}
