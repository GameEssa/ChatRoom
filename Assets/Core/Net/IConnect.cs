using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Net
{
	public interface IConnect
	{

		bool isConnect { get; }

		void Connect();

		void Close();
	}
}