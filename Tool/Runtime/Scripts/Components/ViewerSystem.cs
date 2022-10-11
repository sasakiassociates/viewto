#region

using System;
using System.Collections.Generic;
using System.Linq;
using Sasaki.Unity;
using Speckle.ConnectorUnity;
using UnityEngine;
using UnityEngine.Events;
using VO = ViewObjects;
using ViewObjects.Unity;

#endregion

namespace ViewTo.Connector.Unity
{
	public class ViewerSystem : PixelSystem
	{
		[SerializeField] bool _isGlobal;

		(int cloud, int design) _active;

		List<VO.IResultCloudData> _bundleDataForCloud;

		ViewerSetupData _data;

		VO.ResultStage _stage;

		List<Content> designs
		{
			get => _data.ProposedContent;
		}

		ViewCloud activeCloud
		{
			get => clouds[_active.cloud];
		}

		List<ViewCloud> clouds
		{
			get => _data.Clouds;
		}

		bool checkIfDesignStage
		{
			get => stage == VO.ResultStage.Proposed && hasValidProposedOptions;
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

		public VO.ResultStage stage
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

			foreach (VO.ResultStage v in Enum.GetValues(typeof(VO.ResultStage)))
			{
				if (v == VO.ResultStage.Proposed && !designs.Valid())
					continue;

				values.Add(v.ToString(), v.GetCullingMask());
			}

			return values;
		}

		public void Init(ViewerSetupData data)
		{
			if (!data.Layouts.Valid() || !data.Colors.Valid() || !data.Clouds.Valid())
			{
				Debug.Log($"Invalid Layouts for {name}");
				return;
			}

			_data = data;

			var converted = new List<PixelLayout>();
			foreach (var layout in data.Layouts)
				switch (layout)
				{
					case VO.LayoutCube:
						converted.Add(new GameObject().AddComponent<PixelLayoutCube>());
						break;
					case VO.LayoutHorizontal:
						converted.Add(new GameObject().AddComponent<PixelLayoutHorizontal>());
						break;
					case VO.LayoutOrtho o:
						var res = new GameObject().AddComponent<PixelLayoutOrtho>();
						res.orthoSize = (float)o.Size;
						converted.Add(res);
						break;
					case VO.LayoutFocus o:
						Debug.LogWarning($"{o} is not supported yet");
						// converted.Add(new GameObject().AddComponent<>());
						break;
					case VO.LayoutNormal o:
						// TODO: handle relating the normal cloud type
						Debug.LogWarning($"{o} is not supported yet");
						// converted.Add(new GameObject().AddComponent<>());
						break;
				}

			// OnDataReadyForCloud = onDataForCloud;
			_active.cloud = clouds.Count - 1;
			var systemPoints = clouds[_active.cloud].GetPointsAsVectors();

			Init(systemPoints, data.Colors.ToUnity().ToArray(), converted);

			// Note: important to do this here!
			stage = VO.ResultStage.Potential;
		}

		bool hasMoreStagesToDo
		{
			get
			{
				if (stage == VO.ResultStage.Potential)
					_active.design++;

				stage = GetNextStage(stage);

				if (stage != VO.ResultStage.Proposed)
					return true;

				if (stage == VO.ResultStage.Proposed && hasValidProposedOptions)
				{
					foreach (var d in designs)
						d.Show = false;

					designs[_active.design].Show = true;

					return true;
				}

				return false;
			}
		}

		VO.ResultStage GetNextStage(VO.ResultStage s)
		{
			return s switch
			{
				VO.ResultStage.Potential => VO.ResultStage.Existing,
				VO.ResultStage.Existing => VO.ResultStage.Proposed,
				_ => VO.ResultStage.Proposed
			};
		}

		protected override void ResetSystem()
		{
			_active.cloud = 0;
			_active.design = 0;
			_stage = VO.ResultStage.Potential;
			base.ResetSystem();
		}

		protected override bool ShouldSystemRunAgain()
		{
			// if another stage is available, we store that data with the stage name and reset to 0
			if (hasMoreStagesToDo)
			{
				ResetDataContainer();

				ViewConsole.Log($"{name} is starting next stage "
				                + (checkIfDesignStage ? stage + $" {_data.ProposedContent[_active.design].name}" : stage));
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

				stage = VO.ResultStage.Potential;

				// reset all views  
				foreach (var d in designs)
					d.Show = false;

				return true;
			}

			// if no more clouds are available we finish the call and send off the completed system data
			ViewConsole.Log("All clouds complete, gathering data");
			return false;
		}

		protected override IPixelSystemDataContainer GatherSystemData()
		{
			_bundleDataForCloud ??= new List<VO.IResultCloudData>();

			// gather all data
			var container = new PixelSystemData(this);

			for (var layoutIndex = 0; layoutIndex < container.data.Length; layoutIndex++)
			{
				var layout = container.data[layoutIndex];
				var layoutName = container.layoutNames[layoutIndex];

				// each view color is associated with the second array (double[pointIndex][colorIndex])
				for (var colorIndex = 0; colorIndex < _data.Colors.Count; colorIndex++)
				{
					// go through each finder and compile each point for that color
					var layoutValues = new int[CollectionSize];
					var vc = _data.Colors[colorIndex];

					var raw1d = layout.data.Get1d(colorIndex);

					for (var pIndex = 0; pIndex < raw1d.Length; pIndex++)
					{
						layoutValues[pIndex] += raw1d[pIndex];
					}

					_bundleDataForCloud.Add(
						new VO.ResultCloudData()
						{
							Values = layoutValues.ToList(),
							Layout = layoutName,
							Option = new VO.ContentOption()
							{
								Id = vc.id, Name = vc.name, Stage = stage
							}
						}
					);
				}
			}

			ViewConsole.Log($"{name} is done gathering data for "
			                + $"{(checkIfDesignStage ? stage + $" {_data.ProposedContent[_active.design].name}" : stage)}");

			return container;
		}

		public event UnityAction<VO.ResultStage> OnStageChange;

		public event UnityAction<ResultsForCloud> OnDataReadyForCloud;

	}

}