using System;
using System.Linq;
using UnityEngine;
using ViewObjects.Cloud;
using Object = UnityEngine.Object;

namespace ViewObjects.Unity
{
	public static partial class ViewObjMonoExt
	{

		public static int GetCount(this IViewCloud obj) => obj.points.Valid() ? obj.points.Length : 0;

		public static AMono TryFetchInScene<AMono>(string idToFind) where AMono : ViewObjMono
		{
			foreach (var monoToCheck in Object.FindObjectsOfType<AMono>())
				if (monoToCheck.GetType().CheckForInterface<IId>())
					try
					{
						if (monoToCheck is IId valueToCheck
						    && valueToCheck.ViewId.Valid()
						    && valueToCheck.ViewId.Equals(idToFind))
							return monoToCheck;
					}
					catch (Exception e)
					{
						Console.WriteLine(e);
						throw;
					}

			return null;
		}

		public static AMono TryFetchInScene<AMono>(this IId idToFind) where AMono : ViewObjMono => TryFetchInScene<AMono>(idToFind.ViewId);

		public static ViewCloudMono TryFetchInScene(this CloudShell shell)
		{
			return Object.FindObjectsOfType<ViewCloudMono>().FirstOrDefault(o => o.ViewId != null
			                                                                     && shell.objId != null
			                                                                     && o.ViewId.Equals(shell.objId));
		}

		public static void ApplyAll(this GameObject obj, Material mat)
		{
			foreach (Transform child in obj.transform)
			{
				var cm = child.gameObject.GetComponent<MeshRenderer>();
				if (cm != null)
					if (mat != null)
					{
						if (Application.isPlaying)
							cm.material = mat;
						else
							cm.sharedMaterial = mat;
					}

				if (child.childCount > 0)
					ApplyAll(child.gameObject, mat);
			}
		}
	}
}