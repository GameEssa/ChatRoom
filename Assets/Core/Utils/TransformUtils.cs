using System.Collections.Generic;
using UnityEngine;

namespace Core.Utils
{
	public static class TransformUtils
	{
		public static void ClearChild(this Transform transform)
		{
			var count = transform.childCount;
			Transform[] childs = new Transform[count];
			for (int i = 0; i < count; i++)
			{
				childs[i] = transform.GetChild(i);
			}

			for (int i = 0; i < childs.Length; i++)
			{
				Object.Destroy(childs[i].gameObject);
			}
		}


		public static List<Transform> GetAllChild(this Transform transform)
		{
			var count = transform.childCount;
			List<Transform> result = new List<Transform>(count);

			for (int i = 0; i < count; i++)
			{
				result.Add(transform.GetChild(i));
			}

			return result;
		}
	}
}