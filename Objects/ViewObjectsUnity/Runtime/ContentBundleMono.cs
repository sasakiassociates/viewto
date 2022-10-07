using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace ViewObjects.Unity
{

	public class ContentBundleMono : ViewObjectMono
	{
		[SerializeField] List<Content> viewContents;

		public List<TContent> GetContent<TContent>() where TContent : Content
		{
			var res = new List<TContent>();

			foreach (var vc in viewContents)
			{
				if (vc == null || vc is not TContent target)
					continue;

				res.Add(target);
			}

			return res;
		}

		public List<IContent> Contents
		{
			get => viewContents.Valid() ? viewContents.Cast<IContent>().ToList() : new List<IContent>();
			set
			{
				viewContents = new List<Content>();
				foreach (var v in value)
				{
					Content mono = null;
					if (v is Content contentMono)
						mono = contentMono;
					else
					{
						mono = new GameObject(v.ViewId).AddComponent<Content>();
						mono.ViewId = v.ViewId;
						mono.Color = v.Color;
						mono.ContentType = v.ContentType;
						mono.ContentLayerMask = v.GetLayerMask();
					}

					mono.transform.SetParent(transform);
					viewContents.Add(mono);
				}
			}
		}

		public void Prime(Material material, Action<Content> OnAfterPrime = null, Action<GameObject> OnContentObjPrimed = null)
		{
			foreach (var mono in viewContents)
			{
				var matInstance = Instantiate(material);
				mono.PrimeMeshData(matInstance, OnContentObjPrimed);
				OnAfterPrime?.Invoke(mono);
			}
		}

		void Purge()
		{
			if (viewContents.Valid())
				for (var i = Contents.Count - 1; i >= 0; i--)
					if (Application.isPlaying)
						Destroy(viewContents[i].gameObject);
					else
						DestroyImmediate(viewContents[i].gameObject);

			viewContents = new List<Content>();
		}
	}

}