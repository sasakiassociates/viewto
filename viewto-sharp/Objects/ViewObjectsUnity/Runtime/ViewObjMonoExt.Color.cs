using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using ViewObjects.Common;
using ViewObjects.Contents;
using Random = System.Random;

namespace ViewObjects.Unity
{
	public static partial class ViewObject
	{

		public static List<ViewCloud> GetCloudsByKey(List<string> ids)
		{
			var viewClouds = new List<ViewCloud>();

			if (!ids.Valid())
			{
				return null;
			}

			foreach (var key in ids)
			{
				var obj = TryFetchInScene<ViewCloud>(key);
				if (obj != null)
					viewClouds.Add(obj);
			}

			return viewClouds;
		}

		public static ViewColor ToView(this Color32 value) => new(value.r, value.g, value.b, value.a);

		public static List<Color32> ToUnity(this IEnumerable<ViewColor> value)
		{
			var res = new List<Color32>();
			foreach (var v in value)
				res.Add(v.ToUnity());

			return res;
		}

		public static Color32 ToUnity(this ViewColor value)
		{
			return value != null ? new Color32(value.R, value.G, value.B, value.A) : default;
		}
		
		public static GameObject CombineMeshes(this GameObject obj, Material material)
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
			return go;
		}
	}
}