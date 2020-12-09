using System.Collections;
using System.Collections.Generic;
using Core.Net;
using Core.Pool;
using UnityEngine;

namespace Core.Net
{
	public interface INetCommand : IRecycleObject
	{
		int instance { get; }
		void Handle( IPack pack );
	}
}