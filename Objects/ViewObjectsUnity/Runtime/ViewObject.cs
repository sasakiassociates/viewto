using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ViewObjects.Unity
{

	public static partial class ViewObject
	{
		public const int TargetLayer = 7;

		public const int BlockerLayer = 8;

		public const int ProposedLayer = 6;

		public const int CloudLayer = 9;

		public static int GetLayerMask(this IResultCloud _)
		{
			return CloudLayer;
		}

		public static int GetLayerMask(this IContent value)
		{
			return value.ContentType switch
			{
				ContentType.Target => TargetLayer,
				ContentType.Existing => BlockerLayer,
				ContentType.Proposed => ProposedLayer,
				_ => 0
			};
		}

		public static int GetCount(this IViewCloud obj) => obj.Points.Valid() ? obj.Points.Length : 0;

		public static AMono TryFetchInScene<AMono>(string idToFind) where AMono : ViewObjectMono
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

		public static List<Content> TryFetchInScene(ContentType type)
		{
			return Object.FindObjectsOfType<Content>().Where(x => x.ContentType == type).ToList();
		}

		public static AMono TryFetchInScene<AMono>(this IId idToFind) where AMono : ViewObjectMono => TryFetchInScene<AMono>(idToFind.ViewId);

		public static ViewCloud TryFetchInScene(this IId shell)
		{
			return Object.FindObjectsOfType<ViewCloud>().FirstOrDefault(o => o.ViewId != null
			                                                                 && shell.ViewId != null
			                                                                 && o.ViewId.Equals(shell.ViewId));
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