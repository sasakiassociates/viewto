using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ViewObjects.Unity
{
	public static partial class ViewObjMonoExt
	{

		public static bool CheckForInterface<IFace>(this Type objType)
		{
			return objType.GetInterfaces().Any(x => x == typeof(IFace));
		}

		public static void CheckAndAdd<TObj>(this List<IViewContent> values, List<IViewContent> items) where TObj : IViewContent
		{
			if (items.Valid())
				values.AddRange(items);
		}

		public static void ClearList<TBehaviour>(List<TBehaviour> list) where TBehaviour : MonoBehaviour
		{
			for (var i = list.Count - 1; i >= 0; i--) SafeDestroy(list[i].gameObject);
		}

		public static void SafeDestroy(Object obj)
		{
			if (Application.isPlaying)
				Object.Destroy(obj);
			else
				Object.DestroyImmediate(obj);
		}

		public static List<GameObject> GatherKids(this Transform obj)
		{
			var items = new List<GameObject>();
			foreach (Transform child in obj)
			{
				items.Add(child.gameObject);
				if (child.childCount > 0)
					items.AddRange(child.GatherKids());
			}

			return items;
		}

		public static void SetMeshVisibilityRecursive(this GameObject obj, bool status)
		{
			var mr = obj.GetComponent<MeshRenderer>();
			if (mr != null)
				mr.enabled = status;

			foreach (Transform child in obj.transform) child.gameObject.SetMeshVisibilityRecursive(status);
		}

		public static void SetLayerRecursively(this GameObject obj, int layer)
		{
			obj.layer = layer;

			foreach (Transform child in obj.transform) child.gameObject.SetLayerRecursively(layer);
		}
	}
}