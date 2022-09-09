#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sasaki.Unity;
using UnityEngine;
using UnityEngine.Events;
using ViewObjects;
using ViewObjects.Rig;
using ViewObjects.Unity;
using ViewTo.Connector.Unity.Commands;
using ViewTo.Events.Report;

#endregion

namespace ViewTo.Connector.Unity
{

	public interface IRigSystem : IRig
	{
		public List<ViewerBundleSystem> viewers { get; }

		public ViewerBundleSystem activeViewer { get; }

		public bool isReady { get; }
	}

	public class RigSystem : MonoBehaviour, IRigSystem
	{

		[SerializeField] RigStage _stage;

		[SerializeField] ViewStudyMono _study;

		[SerializeField] [HideInInspector] RigData _data;

		int _activeViewerIndex;

		Stopwatch _timer;

		public RigStage stage
		{
			get => _stage;
			private set => _stage = value;
		}

		public ViewerBundleSystem activeViewer
		{
			get => viewers.Valid(_activeViewerIndex) ? viewers[_activeViewerIndex] : null;
		}

		public List<ViewerBundleSystem> viewers
		{
			get => _data.viewers;
			private set => _data.viewers = value;
		}

		public bool isReady
		{
			get => Application.isPlaying && viewers.Valid() && activeViewer != null;
		}
		

		public void Run(int startPoint = 0)
		{
			if (!isReady)
			{
				ViewConsole.Log("Not Ready");
				return;
			}

			activeViewer.autoRun = true;

			activeViewer.OnStageChange += SetStageChange;
			activeViewer.OnComplete += CompileViewerData;

			_timer ??= new Stopwatch();
			_timer.Start();

			StartCoroutine(RunSafe());
		}

		public IEnumerator RunSafe()
		{
			yield return new WaitForEndOfFrame();

			activeViewer.Run();

			yield return null;
		}

		/// <summary>
		///   Builds
		/// </summary>
		public void Build(ViewStudyMono obj)
		{
			if (!LoadStudy(obj, out var args))
			{
				ViewConsole.Log("Rig did not build correctly");
				return;
			}

			// store study
			_study = obj;

			name = $"Rig-{_study.viewName}";

			// store all colors from scene
			_data.activeColorsInScene = _study.GetViewColors();

			// will search through scene for view clouds with matching id
			clouds = args.clouds;

			// stores colors from view content bundle
			globalColors = args.globalColors;

			// store Params
			globalParams = args.globalParams;

			if (viewers != null)
				ViewObjMonoExt.ClearList(viewers);

			// TODO: Add in non global viewer types
			CreateGlobalRunner();

			CheckViewerSystemSetup();

			OnReady?.Invoke();
		}

		public void TrySetPoint(int index)
		{
			if (activeViewer != null)
				activeViewer.SetToPoint(index);
		}

		public void TrySetStage(ResultStage activeMask)
		{
			ViewConsole.Log($"Setting Stage to {activeMask}");

			foreach (var v in viewers)
				v.stage = activeMask.Convert();
		}

		void CompileViewerData(IFinderSystemData arg)
		{
			_timer.Stop();
			ViewConsole.Log($"Total Time for {activeViewer.name}- {_timer.Elapsed}");

			activeViewer.OnStageChange -= SetStageChange;
			CheckNextViewer();

			ViewConsole.Log($"Compiling data {arg.name}");
		}

		void SetStageChange(RigStage arg)
		{
			stage = arg;
			OnStageChange?.Invoke(stage);
		}

		bool LoadStudy(ViewStudyMono obj, out PrimedRigArgs rigArgs)
		{
			rigArgs = null;

			if (obj == null || !obj.isValid)
			{
				ViewConsole.Warn("Not a valid view study to load");
				return false;
			}

			var initArgs = obj.Initialize();

			ViewConsole.Log(initArgs.message);

			OnContentLoaded?.Invoke(initArgs.contentArgs);

			var studyLoaded = obj.Load(out rigArgs);

			// trigger public event
			OnStudyLoaded?.Invoke(studyLoaded);

			return rigArgs.Valid();
		}

		void CheckViewerSystemSetup()
		{
			var activeViewerIndex = -1;
			for (var i = 0; i < viewers.Count; i++)
			{
				if (viewers[i] == null)
					continue;

				// basic setup things
				viewers[i].transform.SetParent(transform);
				viewers[i].OnDataReadyForCloud += ResultDataCompleted;

				if (viewers[i].isGlobal)
				{
					if (activeViewerIndex >= 0)
					{
						ViewConsole.Warn("More than one global viewer is set");
						return;
					}

					activeViewerIndex = i;
				}
			}

			// there should always be a global viewer, but just in case we will set it to the first viewer to use
			_activeViewerIndex = activeViewerIndex >= 0 ? activeViewerIndex : 0;
		}

		void ResultDataCompleted(ResultsForCloud data)
		{
			// set the result data to the study
			_study.TrySetResults(data);

			// let any other subscriptions know of the data being completed 
			OnDataReadyForCloud?.Invoke(data);
		}

		void CheckNextViewer()
		{
			_activeViewerIndex++;
			if (viewers.Valid(_activeViewerIndex))
			{
				ViewConsole.Log($"Next Viewer[{_activeViewerIndex}] being used: {activeViewer.name}");
				Run();
			}
			else
			{
				ViewConsole.Log("Rig Complete");
				OnComplete?.Invoke();
			}
		}

		void CreateGlobalRunner()
		{
			var global = ViewerBundleSystem.CreateGlobal(
				GetGlobalLayouts(globalParams),
				GetCloudsByKey(clouds?.Keys.ToList()),
				globalColors.ConnectWithContent().ToList(),
				_study.GetContent<DesignContentMono>()
			);

			if (global == null)
			{
				ViewConsole.Error("Global Runner was not built correctly!");
				return;
			}

			viewers ??= new List<ViewerBundleSystem>();
			viewers.Add(global);
		}

		static List<IViewerLayout> GetGlobalLayouts(List<IRigParam> rigParams)
		{
			var globalLayouts = new List<IViewerLayout>();

			if (!rigParams.Valid())
			{
				ViewConsole.Warn("No active Global Params-Skipping Build");
				return globalLayouts;
			}

			foreach (var rp in rigParams)
			{
				if (rp == null) continue;

				if (!rp.bundles.Valid())
				{
					ViewConsole.Warn($"{rp.GetType()} does not have any bundles");
					continue;
				}

				foreach (var b in rp.bundles)
					if (b.layouts.Valid())
						globalLayouts.AddRange(b.layouts);
			}

			return globalLayouts;
		}

		static List<ViewCloudMono> GetCloudsByKey(List<string> ids)
		{
			var viewClouds = new List<ViewCloudMono>();

			if (!ids.Valid())
			{
				ViewConsole.Warn("No valid clouds available to use for global viewer");
				return null;
			}

			foreach (var key in ids)
			{
				var obj = ViewObjMonoExt.TryFetchInScene<ViewCloudMono>(key);
				if (obj != null)
					viewClouds.Add(obj);
			}

			return viewClouds;
		}

		[Serializable]
		internal struct RigData
		{
			public List<ViewColorWithName> activeColorsInScene;
			public List<ViewColor> globalColors;
			public List<RigParamData> rigParams;
			public List<ViewerBundleSystem> viewers;
			public string[] cloudIds;
		}

		#region IRig

		public List<IRigParam> globalParams
		{
			get
			{
				var res = new List<IRigParam>();
				foreach (var rp in _data.rigParams)
					if (!rp.isolate)
						res.Add(rp);

				return res;
			}
			set
			{
				if (!value.Valid())
					return;

				_data.rigParams = new List<RigParamData>();

				foreach (var param in value)
				{
					// all params should have a viewer bundle connected to them
					if (param == null || !param.bundles.Valid())
						continue;

					// build out param for so
					var soRigParam = ScriptableObject.CreateInstance<RigParamData>();

					// set clouds and viewer bundles
					foreach (var viewerBundle in param.bundles)
						if (viewerBundle != null)
							soRigParam.bundles.Add(viewerBundle);

					if (param is RigParametersIsolated iso)
					{
						soRigParam.isolate = true;
						soRigParam.contentColors = iso.colors;
					}
					else
					{
						soRigParam.isolate = false;
						soRigParam.contentColors = globalColors;
					}

					_data.rigParams.Add(soRigParam);
				}
			}
		}

		public List<ViewColor> globalColors
		{
			get => _data.globalColors;
			set => _data.globalColors = value;
		}

		public Dictionary<string, CloudPoint[]> clouds
		{
			get
			{
				var res = new Dictionary<string, CloudPoint[]>();

				if (!_data.cloudIds.Valid())
					ViewConsole.Warn("No clouds conencted to this rig");
				else
					foreach (var id in _data.cloudIds)
					{
						var cld = ViewToUtils.TryFetchCloud(id);
						res.Add(id, cld.points);
					}

				return res;
			}
			set
			{
				if (value.Valid())
					_data.cloudIds = value.Keys.ToArray();
			}
		}

		#endregion

		#region events

		public event UnityAction OnReady;

		public event UnityAction OnComplete;

		public event UnityAction<ViewContentLoadedArgs> OnContentLoaded;

		public event UnityAction<RigStage> OnStageChange;

		public event UnityAction<StudyLoadedArgs> OnStudyLoaded;

		public event UnityAction<ResultsForCloud> OnDataReadyForCloud;

		#endregion

	}
}