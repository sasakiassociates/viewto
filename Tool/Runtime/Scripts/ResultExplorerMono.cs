#region

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using ViewObjects;
using ViewObjects.Rig;
using ViewObjects.Unity;
using ViewObjects.Viewer;
using ViewTo.Connector.Unity.Commands;
using ViewTo.Events.Report;

#endregion

namespace ViewTo.Connector.Unity
{

	public class ResultExplorerMono : MonoBehaviour, IResultExplorer
	{

		#region fields

		[Header("|| OBJECTS ||")] [SerializeField] ResultCloudMono _resultCloud;
		[SerializeField] RigSystem _rig;
		[SerializeField] FirstPersonViewer _viewer;
		[SerializeField] SimpleCloud _filteredCloud;

		[Header("|| SETUP ||")] [SerializeField] bool _normalize;
		[SerializeField] string _activeTarget;
		[SerializeField] Gradient _gradient;

		[Header("|| VALUES ||")] [SerializeField] Vector2 _minMax = new(0f, 1f);
		[SerializeField] int _activePoint;
		[SerializeField] double[] _mappedValues;
		[SerializeField] ResultValue _resultValue;
		[SerializeField] ResultDataSample _targetData;

		[Header("|| FILTERS ||")] [SerializeField] float _pow;
		[SerializeField] Vector2 _range = new(0f, 1f);
		[SerializeField] ObstructedFilters _obstructedFilter = ObstructedFilters.Potential;
		[SerializeField] ResultStage _activeMask = ResultStage.Potential;

		#endregion

		public float minRange
		{
			get => _range.x;
			set => _range.x = value;
		}

		public float maxRange
		{
			get => _range.y;
			set => _range.y = value;
		}

		public float min
		{
			get => _minMax.x;
			set => _minMax.x = value;
		}

		public float max
		{
			get => _minMax.y;
			set => _minMax.y = value;
		}

		public float pointSize
		{
			get => _filteredCloud != null ? _filteredCloud.pointSize : 0;
			set
			{
				if (_filteredCloud != null)
					_filteredCloud.pointSize = value;
			}
		}

		public Vector3[] points
		{
			get => _resultCloud != null ? _resultCloud.pointsAsVector : new[] { Vector3.zero };

		}

		public Gradient gradient
		{
			get
			{
				if (_gradient == null)
				{
					var colorKeys = new GradientColorKey[Commander.BasicColorRamp.Length];

					for (var i = 0; i < colorKeys.Length; i++)
					{
						var c = Commander.BasicColorRamp[i];
						colorKeys[i] = new GradientColorKey(new Color32(c.R, c.G, c.B, 255), (float)i / colorKeys.Length);
					}

					_gradient = new Gradient
					{
						mode = GradientMode.Fixed,
						colorKeys = colorKeys
					};
				}

				return _gradient;
			}
			set
			{
				_gradient = value;
				ApplyValues();
			}
		}

		public float pow
		{
			get => _pow;
			set
			{
				_pow = value;
				ApplyValues();
			}
		}

		public int pointCount
		{
			get => _resultCloud != null ? _resultCloud.count : 0;
		}

		public ObstructedFilters filter
		{
			get => _obstructedFilter;
			set
			{
				_obstructedFilter = value;
				ApplyFilter();
			}
		}

		public ResultStage ResultStage
		{
			get => _activeMask;
			set
			{
				_activeMask = value;

				if (_rig != null)
					_rig.TrySetStage(_activeMask);

				if (ViewToHub.Instance != null)
					ViewToHub.Instance.OnResultStageSet?.Invoke(_activeMask);

				ApplyValues();
			}
		}

		public IResultCloud source
		{
			get => _resultCloud;
		}

		public void Attach(ResultCloudMono obj)
		{
			_resultCloud = obj;

			_resultValue = new ResultValue();
			activeTarget = obj.targets.FirstOrDefault();

			ApplyFilter();

			OnNewCloudSet?.Invoke();
		}

		void ApplyFilter()
		{
			ViewConsole.Log($"Filter set to {_obstructedFilter}");

			switch (_obstructedFilter)
			{
				case ObstructedFilters.Potential:
					activeValues = this.Fetch(ResultStage.Potential).ToArray();
					break;
				case ObstructedFilters.Existing:
					activeValues = this.Fetch(ResultStage.Existing).ToArray();
					break;
				case ObstructedFilters.Proposed:
					activeValues = this.Fetch(ResultStage.Proposed).ToArray();
					break;

				case ObstructedFilters.ExistingOverPotential:
					activeValues = this.GetExistingOverPotential().ToArray();
					break;
				case ObstructedFilters.ProposedOverExisting:
					activeValues = this.GetProposedOverExisting().ToArray();
					break;
				case ObstructedFilters.ProposedOverPotential:
					activeValues = this.GetProposedOverPotential().ToArray();
					break;

				default:
					ViewConsole.Warn("Invalid Enum");
					break;
			}

			// NOTE: something is off with this, and max and min + normalizing from the library is returning different results even thou it's the same exact code. 
			// NOTE: it might be something with the float to double? eh not sure
			min = 0;
			max = 0;
			foreach (var value in activeValues)
			{
				if (value <= 0)
					continue;

				if (min > value)
					min = (float)value;
				if (max < value)
					max = (float)value;
			}

			if (_obstructedFilter == ObstructedFilters.Potential
			    || _obstructedFilter == ObstructedFilters.Existing
			    || _obstructedFilter == ObstructedFilters.Proposed)
			{
				var sample = activeValues.ToList();
				activeValues = sample.NormalizeValues(min, max).ToArray();
			}

			// activeValues.GetMaxMin(out var maxD, out var minD);
			//
			// max = (float)maxD;
			// min = (float)minD;

			ViewConsole.Log($"Min={min} Max={max}");

			ApplyValues();
		}

		public void ApplyValues()
		{
			var index = activePoint;

			_resultValue.potential = (float)_targetData.Get(ResultStage.Potential, index);
			_resultValue.existing = (float)_targetData.Get(ResultStage.Existing, index);
			_resultValue.proposed = (float)_targetData.Get(ResultStage.Proposed, index);

			ApplyValuesToCloud();

			OnActiveValueSet?.Invoke((float)activeValues[index]);
			OnResultValueSet?.Invoke(_resultValue);
		}

		/// <summary>
		///   Force the cloud to be repainted
		/// </summary>
		public void ApplyValuesToCloud()
		{
			if (_resultCloud == null || !_resultCloud.points.Valid())
			{
				ViewConsole.Error($"No result cloud or points are available to update. Connect the result cloud with a valid set of poiints to {name}");
				return;
			}

			// grab the current points
			var pts = points;

			if (pts.Valid())
			{
				var filteredPoints = new List<Vector3>();
				var filteredColors = new List<Color32>();

				for (var i = 0; i < pts.Length; i++)
				{
					var value = (float)activeValues[i];

					// check if value is within range
					if (value < minRange || value > maxRange)
						continue;

					var color = value < 0 ? Color.gray : gradient.Evaluate(value);

					filteredPoints.Add(pts[i]);
					filteredColors.Add(color);
				}

				if (!filteredPoints.Valid() || !filteredColors.Valid())
				{
					ViewConsole.Warn("Point or Colors is not valid to render to pointCloud");
					return;
				}

				if (_filteredCloud == null)
					_filteredCloud = new GameObject("Simple Cloud").AddComponent<SimpleCloud>();

				_filteredCloud.Render(filteredPoints, filteredColors);
			}
		}

		void SetRunner()
		{
			var contentBundle = FindObjectOfType<ContentBundleMono>();

			var vc = new List<ViewColor>();
			if (contentBundle != null)
				foreach (var content in contentBundle.GetContents<TargetContentMono>())
					vc.Add(content.viewColor);
			else
				// TODO: update this build process so result cloud is pulled with a view study
				// NOTE: if not content is loaded into the scene add a empty view color so the rig can build
				vc.Add(new ViewColor(byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue));

			var rigParams = new RigParameters();
			// NOTE: temporary way of setting viewer layout.
			// NOTE: this should probably be related to the view analysis setup
			rigParams.Set(new ViewerLayoutCube());

			// NOTE: This is a little hacky at the moment, we shouldn't need to create these objects just to get a rig up and running
			var globalParams = new List<IRigParam>
				{ rigParams };
			var cloudPoints = new Dictionary<string, CloudPoint[]>
			{
				{
					_resultCloud.viewID, _resultCloud.points
				}
			};

			var args = new PrimedRigArgs(globalParams, vc, cloudPoints);

			if (_rig != null)
				ViewObjMonoExt.SafeDestroy(_rig.gameObject);

			// if (args.Valid())
			// ViewToHub.DebugBundle = true;
			// _rig = ViewToHub.CreateAndBuildRig(args, "ExplorerRig");
			// else
			// ViewConsole.Log("Primed Rig Args is not valid, stopping rig process");
		}

		#region UI Commands

		public void TryFindPoint(float value)
		{
			// var index = this.FindPointWithValue(value);
			var index = ViewToCoreExtensions.CheckCollection(activeValues.ToList(), value);

			if (index >= 0)
				activePoint = index;
			else
				ViewConsole.Warn("No point found with that vlaue");
		}

		#endregion

		#region events

		public event Action OnNewCloudSet;

		public UnityEvent<float> OnActiveValueSet;

		public UnityEvent<ActivePointArgs> OnActivePointSet;

		public UnityAction<ResultValue> OnResultValueSet;

		#endregion

		#region inherited

		public ResultStage activeType
		{
			get => _activeMask;
			set => _activeMask = value;
		}

		public int activePoint
		{
			get => _resultCloud != null ? _resultCloud.activeIndex : 0;
			set
			{
				if (_resultCloud == null)
				{
					ViewConsole.Warn("No Result cloud attached to set active point");
					return;
				}

				_resultCloud.activeIndex = value;
				_activePoint = value;

				OnActivePointSet?.Invoke(new ActivePointArgs(_resultCloud.activePoint, _resultCloud.center));

				if (_rig != null)
					_rig.TrySetPoint(activePoint);

				ApplyValues();
			}

		}

		public string activeTarget
		{
			get => _activeTarget;
			set
			{
				if (!value.Valid() || !targets.Valid() || !targets.Contains(value))
				{
					ViewConsole.Warn($"Trying to set invalid target type {value}. Check what targets are available");
					return;
				}

				_activeTarget = value;
				_targetData = new ResultDataSample(this);
			}
		}

		public double[] activeValues
		{
			get => _mappedValues;
			set
			{
				ViewConsole.Log($"Setting new values with {value.Length}");
				_mappedValues = value;
			}
		}

		public List<string> targets
		{
			get => _resultCloud != null && _resultCloud.targets.Valid() ? _resultCloud.targets : new List<string> { "no targets" };
		}

		public ResultStage ActiveStage { get; set; }

		public List<IResultData> storedData
		{
			get
			{
				var res = new List<IResultData>();

				if (_resultCloud == null)
				{
					ViewConsole.Warn("No Result cloud attached");
					return res;
				}

				foreach (var data in _resultCloud.data)
					res.Add(data);

				return res;
			}
		}

		#endregion

	}

}