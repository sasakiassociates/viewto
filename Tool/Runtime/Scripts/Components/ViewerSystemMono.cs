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
	public class ViewerSystemMono : PixelSystem
	{
		[SerializeField] bool _isGlobal;

		(int cloud, int design) _active;

		List<IResultData> _bundleDataForCloud;

		ViewerSetupData _data;

		ResultStage _stage;

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
			get => stage == ResultStage.Proposed && hasValidProposedOptions;
		}

		bool hasValidProposedOptions
		{
			get => designs.Valid(_active.design);
		}

		public bool isGlobal
		{
			get => _isGlobal;
			set => _isGlobal = value;
		}

		public ResultStage stage
		{
			get => _stage;
			set
			{
				if (!Layouts.Valid())
					return;

				_stage = value;
				Mask = value.GetCullingMask();
				OnStageChange?.Invoke(value);
			}
		}

		public override Dictionary<string, int> GetMaskLayers()
		{
			var values = new Dictionary<string, int>();

			foreach (ResultStage v in Enum.GetValues(typeof(ResultStage)))
			{
				if (v == ResultStage.Proposed && !designs.Valid())
					continue;

				values.Add(v.ToString(), v.GetCullingMask());
			}

			return values;
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

			var converted = new List<PixelLayout>();

			foreach (var layout in _data.layouts)
				switch (layout)
				{
					case ViewerLayoutCube:
						converted.Add(new GameObject().AddComponent<PixelLayoutCube>());
						break;
					case ViewerLayoutHorizontal:
						converted.Add(new GameObject().AddComponent<PixelLayoutHorizontal>());
						break;
					case ViewerLayoutOrtho o:
						var res = new GameObject().AddComponent<PixelLayoutOrtho>();
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
			stage = ResultStage.Potential;
		}

		bool hasMoreStagesToDo
		{
			get
			{
				if (stage == ResultStage.Potential)
					_active.design++;

				stage = GetNextStage(stage);

				if (stage != ResultStage.Proposed)
					return true;

				if (stage == ResultStage.Proposed && hasValidProposedOptions)
				{
					foreach (var d in designs)
						d.show = false;

					designs[_active.design].show = true;

					return true;
				}

				return false;
			}
		}

		ResultStage GetNextStage(ResultStage s)
		{
			return s switch
			{
				ResultStage.Potential => ResultStage.Existing,
				ResultStage.Existing => ResultStage.Proposed,
				_ => ResultStage.Proposed
			};
		}

		protected override void ResetSystem()
		{
			_active.cloud = 0;
			_active.design = 0;
			_stage = ResultStage.Potential;
			
			base.ResetSystem();
		}

		protected override bool ShouldSystemRunAgain()
		{
			// if another stage is available, we store that data with the stage name and reset to 0
			if (hasMoreStagesToDo)
			{
				ResetDataContainer();

				ViewConsole.Log($"{name} is starting next stage "
				                + (checkIfDesignStage ? stage + $" {_data.designContent[_active.design].name}" : stage));
				return true;
			}

			// if we reached the last stage for a cloud, we send that data to morph the view cloud into a result cloud
			OnDataReadyForCloud?.Invoke(new ResultsForCloud(activeCloud.ViewId, _bundleDataForCloud));

			// remove the previous index
			clouds.RemoveAt(_active.cloud);

			// step forward to see if have more clouds
			_active.cloud--;

			// if we are done with our clouds 
			if (_active.cloud >= 0)
			{
				ResetDataContainer();

				// there are more clouds to use, so we store the points and set the run back to 0
				ViewConsole.Log("Loading new cloud");

				// store points
				Points = clouds[_active.cloud].GetPointsAsVectors();

				stage = ResultStage.Potential;

				// reset all views  
				foreach (var d in designs)
					d.show = false;

				return true;
			}

			// if no more clouds are available we finish the call and send off the completed system data
			ViewConsole.Log("All clouds complete, gathering data");
			return false;
		}

		protected override IPixelSystemDataContainer GatherSystemData()
		{
			_bundleDataForCloud ??= new List<IResultData>();

			// gather all data
			var container = new PixelSystemData(this);

			for (var layoutIndex = 0; layoutIndex < container.data.Length; layoutIndex++)
			{
				var layout = container.data[layoutIndex];
				var layoutName = container.layoutNames[layoutIndex];

				// each view color is associated with the second array (double[pointIndex][colorIndex])
				for (var colorIndex = 0; colorIndex < _data.viewColors.Count; colorIndex++)
				{
					// go through each finder and compile each point for that color
					var layoutValues = new int[CollectionSize];
					var content = _data.viewColors[colorIndex].content;

					var raw1d = layout.data.Get1d(colorIndex);
					
					for (var pIndex = 0; pIndex < raw1d.Length; pIndex++)
					{
						layoutValues[pIndex] += raw1d[pIndex];
					}

					_bundleDataForCloud.Add(
						new ContentResultData(
							layoutValues.ToList(),
							stage.ToString(),
							content,
							colorIndex,
							layout: layoutName
						));
				}
			}

			ViewConsole.Log($"{name} is done gathering data for "
			                + $"{(checkIfDesignStage ? stage + $" {_data.designContent[_active.design].name}" : stage)}");

			return container;
		}

		public static ViewerSystemMono CreateGlobal(
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

		public static ViewerSystemMono Create(
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

			var system = new GameObject().AddComponent<ViewerSystemMono>();
			system.name = name;
			system.Init(new ViewerSetupData(clouds, layouts, viewColors, designContent));

			return system;
		}

		public event UnityAction<ResultStage> OnStageChange;

		public event UnityAction<ResultsForCloud> OnDataReadyForCloud;

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
	}

}