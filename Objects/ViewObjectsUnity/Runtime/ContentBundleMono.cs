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

		public void Prime(Material material, Action<Content> OnAfterPrime = null, Action<ContentObj> OnContentObjPrimed = null)
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

	public static class SimpleHelper
	{
		public static ContentObj CombineMeshes(this GameObject obj, Material material)
		{
			var meshFilters = obj.GetComponentsInChildren<MeshFilter>();
			Debug.Log($"Compiling {meshFilters.Length} Meshes");

			var combine = new List<CombineInstance>();
			var combinedGroup = new List<List<CombineInstance>>();

			var currentVertexCount = 0;
			foreach (var t in meshFilters)
			{
				if (t == null || t.sharedMesh == null)
					continue;

				var temp = Object.Instantiate(t.sharedMesh);
				if (currentVertexCount + temp.vertexCount >= int.MaxValue)
				{
					// reset 
					Debug.Log($"Reset triggered at {currentVertexCount}");
					combinedGroup.Add(combine);
					combine = new List<CombineInstance>();
					currentVertexCount = 0;
				}

				var c = new CombineInstance
				{
					mesh = temp,
					transform = t.transform.localToWorldMatrix
				};

				combine.Add(c);
				currentVertexCount += temp.vertexCount;
			}

			var go = new GameObject("Mesh")
			{
				isStatic = true
			};

			var filter = go.AddComponent<MeshFilter>();
			var mesh = filter.sharedMesh = new Mesh();

			mesh.indexFormat = IndexFormat.UInt32;
			mesh.name = "combined";

			mesh.CombineMeshes(combine.ToArray());

			var rend = go.AddComponent<MeshRenderer>();
			rend.sharedMaterial = material;
			rend.shadowCastingMode = ShadowCastingMode.Off;
			rend.allowOcclusionWhenDynamic = rend.receiveShadows = false;

			foreach (Transform child in obj.transform)
				Object.Destroy(child.gameObject);

			foreach (var t in combine)
				Object.Destroy(t.mesh);

			go.transform.SetParent(obj.transform);
			return go.AddComponent<ContentObj>();
		}
	}

}