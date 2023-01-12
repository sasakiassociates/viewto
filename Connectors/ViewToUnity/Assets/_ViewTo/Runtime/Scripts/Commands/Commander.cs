#region

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using ViewObjects;
using ViewObjects.Contents;
using ViewObjects.Unity;
using Object = UnityEngine.Object;

#endregion

namespace ViewTo.Connector.Unity.Commands
{
	public static class Commander
	{

		public static void CombineMeshes(this GameObject obj, Material material)
		{
			var meshFilters = obj.GetComponentsInChildren<MeshFilter>();
			Debug.Log($"Compiling {meshFilters.Length} Meshes");
			// CombineInstance[ ] combine = new CombineInstance[ meshFilters.Length ];

			var combine = new List<CombineInstance>();
			var combinedGroup = new List<List<CombineInstance>>();

			var currentVertexCount = 0;
			foreach (var t in meshFilters)
			{
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

			var go = new GameObject("Mesh");

			var filter = go.AddComponent<MeshFilter>();
			var mesh = filter.sharedMesh = new Mesh();

			// NOTE changing this allows for larger vertex count
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
		}

		public static bool Sim(this ViewColor a, ViewColor b) => a.R == b.R && a.G == b.G && a.B == b.B;

		/// <summary>
		///   Gets all available view colors in the active unity scene
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<ViewColorWithName> GetViewColorsFromScene(this List<ViewColor> colors)
		{
			var nameWithColor = new List<ViewColorWithName>();

			foreach (var content in ViewObject.TryFetchInScene(ContentType.Potential))
			{
				if (content.transform.hideFlags != HideFlags.None)
				{
					continue;
				}

				foreach (var vc in colors)
				{
					if (vc.Sim(content.Color))
					{
						if (nameWithColor.Count == 0)
						{
							nameWithColor.Add(new ViewColorWithName(content));
						}
						else if (!nameWithColor.Any(x => x.id.Equals(content.ViewId)))
						{
							nameWithColor.Add(new ViewColorWithName(content));
						}
					}
				}
			}

			return nameWithColor;
		}

		public static ContentType Convert(this RigStage value)
		{
			return value switch
			{
				RigStage.Target => ContentType.Potential,
				RigStage.Blocker => ContentType.Existing,
				RigStage.Design => ContentType.Proposed,
				RigStage.Complete => ContentType.Proposed,
				_ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
			};
		}

		public static RigStage Convert(this ContentType value)
		{
			return value switch
			{
				ContentType.Potential => RigStage.Target,
				ContentType.Existing => RigStage.Blocker,
				ContentType.Proposed => RigStage.Design,
				_ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
			};
		}
	}
}