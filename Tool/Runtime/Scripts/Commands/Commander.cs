#region

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using ViewObjects;
using ViewObjects.Unity;
using ViewObjects.Viewer;
using ViewTo.Events.Process;
using ViewTo.Events.Report;
using Object = UnityEngine.Object;

#endregion

namespace ViewTo.Connector.Unity.Commands
{
	public static class Commander
	{
		public static StudyLoadedArgs Load(this ViewStudyMono mono, out PrimedRigArgs rigArgs)
		{
			rigArgs = mono.LoadStudyForRig();

			return new StudyLoadedArgs(mono.viewName,
			                           rigArgs.Valid() ? rigArgs.clouds.GetSum() : 0,
			                           mono.CanRun(),
			                           mono.CanVisualize());
		}

		public static bool Valid(this PrimedRigArgs args) => args.globalColors.Valid() && args.globalParams.Valid() && args.clouds.Valid();

		public static StudyInitArgs Initialize(this ViewStudyMono mono)
		{
			StudyInitArgs args = null;

			// NOTE: Viewer Bundles are a bit hacky still, so for now a cube camera is pushed into the study 
			var bun = mono.Get<IViewerBundle>();

			if (bun != null)
				bun.layouts = new List<IViewerLayout>
				{
					new ViewerLayoutCube()
				};

			// studyMono.Get<IResultCloud>()
			// TODO: set this when the conversion happens

			if (mono.Get<IViewContentBundle>() is ContentBundleMono contentBundle)
			{
				contentBundle.Prime(ViewToHub.AnalysisMat);

				args = new StudyInitArgs(new ViewContentLoadedArgs(
					                         contentBundle.GetContentCount<ITargetContent>(),
					                         contentBundle.GetContentCount<IBlockerContent>(),
					                         contentBundle.GetContentCount<IDesignContent>()),
				                         new ViewerBundleInitArgs(bun.layouts.Count, 1)
				);
			}

			return args;
		}

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

			foreach (Transform child in obj.transform) Object.Destroy(child.gameObject);

			foreach (var t in combine) Object.Destroy(t.mesh);

			go.transform.SetParent(obj.transform);
		}

		public static PrimedRigArgs TryLoadStudy(this IRig rig, IViewStudy study)
		{
			// first check if data is there 
			if (study == null)
			{
				Debug.LogWarning("Study is null, cannot load into rig");
				return null;
			}

			if (rig == null)
			{
				Debug.LogWarning("Rig is null, cannot build out rig");
				return null;
			}

			Debug.Log("Running LoadStudy Commnad");

			Debug.Log("Checking for data");
			var checkDataArgs = CheckDataCommand(study);

			if (checkDataArgs == null)
			{
				Debug.Log("Check Data Args did not report, stopping command");
				return null;
			}

			Debug.Log(checkDataArgs.message);
			if (!checkDataArgs.success)
			{
				Debug.Log("Check data completed without success, ending command");
				return null;
			}

			Debug.Log("Check data completed");

			Debug.LogWarning("PROCESS NOT COMPLETE");
			// var cmd = new LoadStudyToRigCommand(study, ref rig);
			// cmd.Run();

			return new PrimedRigArgs(rig);
		}

		public static ValidStudyArg CheckDataCommand(this IViewStudy obj)
		{
			var greatSuccess = false;

			try
			{
				if (obj.CanRun())
				{
					var content = obj.Get<IViewContentBundle>();
					var blockersCount = content.GetContentCount<IBlockerContent>();
					var designsCount = content.GetContentCount<IDesignContent>();
					var targetCount = content.GetContentCount<ITargetContent>();

					var clouds = obj.GetAll<IViewCloud>()
						.ToDictionary(cld => cld?.viewID, cld => cld != null && cld.points.Valid() ? cld.points.Length : 0);

					var bundles = obj.GetAll<IViewerBundle>().ToList();
					var globalBundleCount = 0;
					foreach (var bundle in bundles)
					{
						var layouts = bundle.layouts;
						if (layouts.Valid())
							globalBundleCount += layouts.Count;
					}

					var isoTargetCount = 0;
					var isoBundleCount = 0;
					foreach (var target in content.GetContents<ITargetContent>())
					{
						var cl = target.SearchForClouds();
						if (cl.Valid())
							foreach (var c in cl.Where(c => !clouds.ContainsKey(c.objId)))
								clouds.Add(c.objId, c.count);

						if (target.isolate)
						{
							isoTargetCount++;
							isoBundleCount += target.bundles.Valid() ? target.bundles.Count : 0;
						}
					}

					obj.Report(
						blockersCount, designsCount, targetCount, isoTargetCount,
						globalBundleCount, isoBundleCount,
						clouds.Count, clouds.Values.Sum());

					// basic inputs needed for a project to run 
					greatSuccess = clouds.Any() && targetCount > 0 && globalBundleCount > 0;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}

			return new ValidStudyArg(greatSuccess, obj.viewName, greatSuccess ? "Succes" : Study.LoadError.MissingObjects.ToString());
		}

		public static int GetLayoutCount(this List<IViewerBundle> objs)
		{
			var res = 0;

			foreach (var o in objs)
				res += o.layouts.Count;

			return res;
		}

		// private double[] CompileData
		// {
		// 	get
		// 	{
		// 		List<double> tempData = new List<double>();
		// 		foreach (var viewer in viewers)
		// 		{
		// 			var viewData = viewer.ViewerData;
		//
		// 			for (var i = 0; i < viewData.Length; i++)
		// 			{
		// 				var value = viewData[i];
		// 				if (tempData.Count <= i)
		// 					tempData.Add(value);
		// 				else
		// 					tempData[i] += value;
		// 			}
		// 		}
		//
		// 		return tempData.ToArray();
		// 	}
		// }

		/// <summary>
		///   Adds the name of the associated all view content types to a viewer color
		/// </summary>
		/// <param name="colors">View Colors to search for</param>
		/// <returns></returns>
		public static IEnumerable<ViewColorWithName> ConnectWithContent(this List<ViewColor> colors)
		{
			var nameWithColor = new List<ViewColorWithName>();

			foreach (var vc in GetViewColorsFromScene(false))
			foreach (var viewColor in colors)
			{
				if (viewColor.R != vc.R || viewColor.G != vc.G || viewColor.B != vc.B)
					continue;

				nameWithColor.Add(vc);
				break;
			}

			return nameWithColor;
		}

		public static List<ViewColorWithName> GetViewColors(this IViewStudy study, bool targetsOnly = true)
		{
			var colors = new List<ViewColorWithName>();

			if (study is not { isValid: true })
			{
				ViewConsole.Log("Study is not valid, cannot find colors");
				return colors;
			}

			foreach (var obj in study.objs)
			{
				if (obj is not ContentBundleMono bundleMono)
					continue;

				foreach (var c in bundleMono.contents)
				{
					if (c == default)
						continue;

					if (!targetsOnly)
						colors.Add(new ViewColorWithName(c));

					if (targetsOnly && c is ITargetContent)
						colors.Add(new ViewColorWithName(c));
				}
			}

			return colors;
		}

		public static bool TrySetToCloud(this ViewCloudMono cloud, ResultsForCloud container, out ResultCloudMono mono)
		{
			mono = null;

			if (cloud != null)
			{
				// reference game object with view cloud attached
				var go = cloud.gameObject;
				mono = go.AddComponent<ResultCloudMono>();
				mono.viewID = cloud.viewID;
				mono.points = cloud.points;
				mono.data = container.data;
			}

			return mono != null;
		}

		/// <summary>
		///   Gets all available view colors in the active unity scene
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<ViewColorWithName> GetViewColorsFromScene(bool targetsOnly = true)
		{
			var nameWithColor = new List<ViewColorWithName>();

			foreach (var vc in Object.FindObjectsOfType<ContentMono>())
			{
				if (vc.transform.hideFlags != HideFlags.None)
					continue;

				if (targetsOnly && vc is not ITargetContent)
					continue;

				if (nameWithColor.Count == 0)
					nameWithColor.Add(new ViewColorWithName(vc.viewColor, vc.viewName));

				else if (!nameWithColor.Any(x => x.content.Equals(vc.viewName)))
					nameWithColor.Add(new ViewColorWithName(vc.viewColor, vc.viewName));
			}

			return nameWithColor;
		}

		public static ResultStage Convert(this RigStage value)
		{
			return value switch
			{
				RigStage.Target => ResultStage.Potential,
				RigStage.Blocker => ResultStage.Existing,
				RigStage.Design => ResultStage.Proposed,
				RigStage.Complete => ResultStage.Proposed,
				_ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
			};
		}

		public static RigStage Convert(this ResultStage value)
		{
			return value switch
			{
				ResultStage.Potential => RigStage.Target,
				ResultStage.Existing => RigStage.Blocker,
				ResultStage.Proposed => RigStage.Design,
				_ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
			};
		}
	}
}