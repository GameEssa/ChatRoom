using UnityEngine;
using System.Collections;

namespace Core
{
	public class Application : MonoBehaviour
	{
		private void Awake()
		{
			DontDestroyOnLoad( this.gameObject );

			Net.NetManager.InitializeInstance();
		}





		private void OnDestroy()
		{
			Net.NetManager.DestroyInstance();
		}
	}
}
