using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Net
{
	public interface IPack
	{
		byte[] data { get; }

		byte[] origin { get; }

		string tag { get; }

		void Parse( byte[] bytes );
	}
}
