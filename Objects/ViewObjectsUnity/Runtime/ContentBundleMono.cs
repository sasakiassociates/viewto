using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace ViewObjects.Unity
{

	public class ContentBundleMono : ViewObjMono, IViewContentBundle
	{
		[SerializeField] List<ContentMono> viewContents;

		public List<TContent> GetContent<TContent>() where TContent : ContentMono
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

		public List<IViewContent> contents
		{
			get => viewContents.Valid() ? viewContents.Cast<IViewContent>().ToList() : new List<IViewContent>();
			set
			{
				viewContents = new List<ContentMono>();
				foreach (var v in value)
				{
					ContentMono mono = null;
					if (v is ContentMono contentMono)
						mono = contentMono;
					else
						mono = v switch
						{
							ITargetContent _ => new GameObject().AddComponent<TargetContentMono>(),
							IBlockerContent _ => new GameObject().AddComponent<BlockerContentMono>(),
							IDesignContent _ => new GameObject().AddComponent<DesignContentMono>(),
							_ => null
						};

					mono.contentLayerMask = v.GetLayerMask();
					mono.transform.SetParent(transform);
					viewContents.Add(mono);
				}
			}
		}

		public void ChangeColors()
		{
			var colors = contents.CreateBundledColors();
			for (var i = 0; i < contents.Count; i++)
				contents[i].viewColor = colors[i];
		}

		public void Prime(Material material, Action<ContentMono> OnAfterPrime = null, Action<ContentObj> OnContentObjPrimed = null)
		{
			ChangeColors();

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
				for (var i = contents.Count - 1; i >= 0; i--)
					if (Application.isPlaying)
						Destroy(viewContents[i].gameObject);
					else
						DestroyImmediate(viewContents[i].gameObject);

			viewContents = new List<ContentMono>();
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
				if (t == null || t.sharedMesh == null) continue;

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