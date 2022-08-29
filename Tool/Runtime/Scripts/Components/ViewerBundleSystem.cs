#region

using System;
using System.Collections.Generic;
using System.Linq;
using Sasaki.Unity;
using UnityEngine;
using UnityEngine.Events;
using ViewObjects;
using ViewObjects.Content;
using ViewObjects.Unity;
using ViewObjects.Viewer;

#endregion

namespace ViewTo.Connector.Unity
{
	public class ViewerBundleSystem : PixelFinderSystem
	{
		[SerializeField]
		bool _isGlobal;

		// public bool _saveScreenShot;

		(int cloud, int design) _active;

		List<IResultData> _bundleDataForCloud;

		ViewerSetupData _data;

		RigStage _stage;

		List<DesignContentMono> designs
		{
			get => _data.designContent;
		}

		ViewCloudMono activeCloud
		{
			get => clouds[_active.cloud];
		}

		List<ViewCloudMono> clouds
		{
			get => _data.clouds;
		}

		bool checkIfDesignStage
		{
			get => stage == RigStage.Design && hasValidDesignOption;
		}

		bool hasValidDesignOption
		{
			get => designs.Valid(_active.design);
		}

		public bool isGlobal
		{
			get => _isGlobal;
			set => _isGlobal = value;
		}

		public RigStage stage
		{
			get => _stage;
			set
			{
				if (!layouts.Valid())
					return;

				_stage = value;
				mask = value.GetCullingMask();

				OnStageChange?.Invoke(value);
			}
		}

		public void Init(ViewerSetupData data)
		{
			if (!data.layouts.Valid() || !data.viewColors.Valid() || !data.clouds.Valid())
			{
				Debug.Log($"Invalid Layouts for {name}");
				return;
			}

			// copy over 
			_data = data;

			var converted = new List<PixelFinderLayout>();

			foreach (var layout in _data.layouts)
				switch (layout)
				{
					case ViewerLayoutCube:
						converted.Add(new GameObject().AddComponent<PixelFinderCube>());
						break;
					case ViewerLayoutHorizontal:
						converted.Add(new GameObject().AddComponent<PixelFinderHorizontal>());
						break;
					case ViewerLayoutOrtho o:
						var res = new GameObject().AddComponent<PixelFinderOrtho>();
						res.orthoSize = (float)o.size;
						converted.Add(res);
						break;
					case ViewerLayoutFocus o:
						Debug.LogWarning($"{o} is not supported yet");
						// converted.Add(new GameObject().AddComponent<>());
						break;
					case ViewerLayoutNormal o:
						// TODO: handle relating the normal cloud type
						Debug.LogWarning($"{o} is not supported yet");
						// converted.Add(new GameObject().AddComponent<>());
						break;
				}

			// OnDataReadyForCloud = onDataForCloud;
			_active.cloud = clouds.Count - 1;
			var systemPoints = clouds[_active.cloud].GetPointsAsVectors();

			Init(systemPoints, _data.viewColors.ToUnity().ToArray(), converted);

			// Note: important to do this here!
			stage = RigStage.Target;
		}

		public RigStage GetNextStage(RigStage s)
		{
			return s switch
			{
				RigStage.Target => RigStage.Blocker,
				RigStage.Blocker => hasValidDesignOption ? RigStage.Design : RigStage.Complete,
				RigStage.Design => hasValidDesignOption ? RigStage.Design : RigStage.Complete,
				RigStage.Complete => RigStage.Complete,
				_ => RigStage.Complete
			};
		}

		void StoreStageData()
		{
			// store the data!
			var meta = "meta";
			var type = ResultType.Proposed;

			if (stage == RigStage.Target)
				type = ResultType.Potential;
			else if (stage == RigStage.Blocker)
				type = ResultType.Existing;
			else if (stage == RigStage.Design)
				type = ResultType.Proposed;

			_bundleDataForCloud ??= new List<IResultData>();

			// gather all data
			var container = new FinderSystemDataContainer(layouts, name);

			for (var layoutIndex = 0; layoutIndex < container.data.Length; layoutIndex++)
			{
				var layout = container.data[layoutIndex];
				// each view color is associated with the second array (double[pointIndex][colorIndex])
				for (var colorIndex = 0; colorIndex < _data.viewColors.Count; colorIndex++)
				{
					var layoutValues = new List<double>();

					// go through each finder and compile each point for that color
					foreach (var finder in layout.data)
						// raw data from finder
						for (var i = 0; i < finder.data.Length; i++)
						{
							var value = finder.data[i][colorIndex];
							if (layoutValues.Count <= i)
								layoutValues.Add(value);
							else
								layoutValues[i] += value;
						}

					_bundleDataForCloud.Add(
						new ContentResultData(
							layoutValues,
							type.ToString(),
							_data.viewColors[colorIndex].content,
							colorIndex,
							layout: container.layoutNames[layoutIndex],
							meta: meta
						));
				}
			}
		}

		protected override void GatherSystemData()
		{
			ViewConsole.Log($"{name} is done gathering data for "
			                + $"{(checkIfDesignStage ? stage + $" {_data.designContent[_active.design].name}" : stage)}");

			StoreStageData();

			if (stage == RigStage.Design)
				_active.design++;

			stage = GetNextStage(stage);

			// if another stage is available, we store that data with the stage name and reset to 0
			if (stage != RigStage.Complete)
			{
				// if we are at the design stage and 
				if (checkIfDesignStage)
				{
					foreach (var d in designs)
						d.show = false;

					designs[_active.design].show = true;
				}

				// isComplete = false;
				ResetDataContainer();

				ViewConsole.Log($"{name} is starting next stage "
				                + (checkIfDesignStage ? stage + $" {_data.designContent[_active.design].name}" : stage));
				Run();
				return;
			}

			// if we reached the last stage for a cloud, we send that data to morph the view cloud into a result cloud
			OnDataReadyForCloud?.Invoke(new ResultsForCloud(activeCloud.viewID, _bundleDataForCloud));

			// remove the previous index
			clouds.RemoveAt(_active.cloud);

			// step forward to see if have more clouds
			_active.cloud--;

			// if we are done with our clouds 
			if (_active.cloud < 0)
			{
				ViewConsole.Log("All clouds complete, gathering data");
				// if no more clouds are available we finish the call and send off the completed system data
				base.GatherSystemData();

				return;
			}

			// there are more clouds to use, so we store the points and set the run back to 0
			ViewConsole.Log("Loading new cloud");

			// store points
			points = clouds[_active.cloud].GetPointsAsVectors();

			stage = RigStage.Target;

			autoRun = true;

			// reset all views  
			foreach (var d in designs)
				d.show = false;

			// reset to 0 and run
			Run();
		}

		// protected override void MoveAndRender()
		// {
		// 	// if (_saveScreenShot && pointIndex != 0)
		// 	// 	foreach (var layout in layouts)
		// 	// 	foreach (var finder in layout.finders)
		// 	// 		SaveTextureAsPNG(finder.toTexture2D(), @"D:\Projects\ViewTo\captures\tests\premove\",
		// 	// 		                 $"{stage}({pointIndex})_{finder.data.data[0].FirstOrDefault()}_{finder.name}_"
		// 	// 		                 + DateTime.Now.ToString("yyyyMMdd_HHmmss"));
		//
		// 	base.MoveAndRender();
		//
		// 	// if (_saveScreenShot && pointIndex != 0)
		// 	// 	foreach (var layout in layouts)
		// 	// 	foreach (var finder in layout.finders)
		// 	// 		SaveTextureAsPNG(finder.toTexture2D(), @"D:\Projects\ViewTo\captures\tests\postmove\",
		// 	// 		                 $"{stage}({pointIndex})_{finder.data.data[0].FirstOrDefault()}_{finder.name}_"
		// 	// 		                 + DateTime.Now.ToString("yyyyMMdd_HHmmss"));
		// }

		/// <summary>
		///   Manually sets the viewer to a specific point in the collection
		///   This will automatically toggle autorun off
		/// </summary>
		/// <param name="index">the point to use</param>
		public void SetToPoint(int index)
		{
			autoRun = false;
			if (points.Valid(index))
			{
				Debug.Log($"setting {index} as point {points[index]}  ");
				Run(index);
			}
		}

		public static ViewerBundleSystem CreateGlobal(
			List<IViewerLayout> layouts,
			List<ViewCloudMono> clouds,
			List<ViewColorWithName> viewColors,
			List<DesignContentMono> designContent
		)
		{
			var system = Create("Global", layouts, clouds, viewColors, designContent);

			if (system != null)
				system.isGlobal = true;

			return system;
		}

		public static ViewerBundleSystem Create(
			string name,
			List<IViewerLayout> layouts,
			List<ViewCloudMono> clouds,
			List<ViewColorWithName> viewColors,
			List<DesignContentMono> designContent
		)
		{
			if (!layouts.Valid() || !clouds.Valid() || !viewColors.Valid())
			{
				ViewConsole.Error("Viewer Setup Data could not be created, points or colors are not setup correctly");
				return null;
			}

			var system = new GameObject().AddComponent<ViewerBundleSystem>();
			system.name = name;
			system.Init(new ViewerSetupData(clouds, layouts, viewColors, designContent));

			return system;
		}

		public event UnityAction<RigStage> OnStageChange;

		public event UnityAction<ResultsForCloud> OnDataReadyForCloud;
	}

}