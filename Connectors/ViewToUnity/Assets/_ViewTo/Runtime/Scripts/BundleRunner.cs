//
// 	public class BundleRunner : MonoBehaviour
// 	{
// 		[SerializeField] private bool isRunning = false;
//
// 		[SerializeField] protected List<ViewerBundleSystem> layouts;
// 		[SerializeField] protected RigStage stage = RigStage.Target;
//
// 		[SerializeField] List<PixelFinderLayout> _systems;
//
// 		[SerializeField, HideInInspector]
// 		private SoRunnerData _data;
// 		[SerializeField, HideInInspector]
// 		protected List<ContentMono> designContentInScene;
//
// 		// NOTE: fields for protecting data collection
// 		[SerializeField, HideInInspector]
// 		private bool trashedFirstPoint;
// 		private int currentRepeat;
// 		private const int RepeatCount = 2;
//
// 		public event Action<ViewerBundleSystem> OnLayoutAdded;
//
// 		protected SoRunnerData data
// 		{
// 			get
// 			{
// 				if (_data == null)
// 					_data = ScriptableObject.CreateInstance<SoRunnerData>();
// 				return _data;
// 			}
// 		}
//
// 		public event Action<RigStage> OnStageSet;
// 		public event Action<bool> OnRunState;
// 		public event Action OnStartRun;
//
// 		public IEnumerable<ViewerBundleSystem> GetLayouts
// 		{
// 			get => layouts;
// 		}
//
// 		public bool IsComplete { get; protected set; }
//
// 		public bool Running
// 		{
// 			get => isRunning;
// 		}
//
// 		public int PointIndex { get; protected set; }
//
// 		public int PointCount
// 		{
// 			get => _data != null ? _data.count : 0;
// 		}
//
// 		protected Vector3[] points
// 		{
// 			get => data.points;
// 			set => data.points = value;
// 		}
//
// 		protected Color32[] colors
// 		{
// 			get => data.colors;
// 		}
//
// 		protected ViewColor[] viewColors
// 		{
// 			get => data.viewColors;
// 			set => data.viewColors = value;
// 		}
//
// 		protected List<ViewColorWithName> nameAndColor
// 		{
// 			get => data.nameAndColor;
// 			set => data.nameAndColor = value;
// 		}
//
// 		protected RigStage SetNewStage
// 		{
// 			set
// 			{
// 				stage = value;
// 				OnStageSet?.Invoke(stage);
// 			}
// 		}
//
// 		protected void SetToPoint()
// 		{
// 			if (points.Valid(PointIndex))
// 			{
// 				transform.position = points[PointIndex];
//
// 				if (!isRunning)
// 					IsRunning(true);
//
// 				foreach (var l in layouts)
// 					l.PreRender();
// 			}
// 		}
//
// 		public void TrySetToIndex(int index)
// 		{
// 			if (points.Valid(index))
// 			{
// 				PointIndex = index;
// 				SetToPoint();
// 			}
// 		}
//
// 		protected void IsRunning(bool running)
// 		{
// 			// Debug.Log(running ? "Running" : "Paused");
// 			isRunning = running;
// 			OnRunState?.Invoke(running);
// 		}
//
// 		public void Init(
// 			List<IViewerLayout> viewerLayouts, ViewerSetupData setup, List<ViewColorWithName> nameWithColor, List<ContentMono> designsInScene = null
// 		)
// 		{
//
// 			_systems = new List<PixelFinderLayout>();
// 			
//
// 			_data = ScriptableObject.CreateInstance<SoRunnerData>();
// 			points = setup.points;
//
// 			viewColors = setup.viewColors;
// 			nameAndColor = nameWithColor;
// 			designContentInScene = designsInScene;
//
// 			layouts = new List<ViewerBundleSystem>();
//
// 			// NOTE: since this is a collection of IViewerlayouts we need to check if it's an active game obj yet 
// 			foreach (var layout in viewerLayouts)
// 			{
// 				switch (layout)
// 				{
// 					// case 1: not a unity object yet, so we need to push that bad pup into the scene 
// 					case ViewerLayout obj:
// 						var viewerMono = new GameObject().AddComponent<ViewerLayoutMono>();
// 						viewerMono.SetData(obj);
// 						AttachAndSetViewerSystem(viewerMono);
// 						break;
// 					// case 2: it's a unity object so we are good to go
// 					case ViewerLayoutMono mono:
// 						AttachAndSetViewerSystem(mono);
// 						break;
// 				}
// 			}
// 		}
//
// 		public virtual void StartRunner(int startPoint)
// 		{
// 			PreRenderSystems();
// 			OnStageSet?.Invoke(stage);
//
// 			if (points.Valid(startPoint))
// 				PointIndex = startPoint;
// 			else
// 			{
// 				ViewConsole.Warn($"Trying to start runner at invalid index {startPoint}, setting index to 0");
// 				PointIndex = 0;
// 			}
//
// 			currentRepeat = 0;
// 			IsRunning(true);
//
// 			OnStartRun?.Invoke();
// 		}
//
// 		public virtual void StartRunner()
// 		{
// 			PreRenderSystems();
// 			OnStageSet?.Invoke(stage);
//
// 			PointIndex = 0;
// 			currentRepeat = 0;
// 			IsRunning(true);
//
// 			OnStartRun?.Invoke();
// 		}
//
// 		/// <summary>
// 		/// Method for checking if first point is trashed and repeat counter threshold is passed
// 		/// </summary>
// 		/// <returns></returns>
// 		protected bool CheckRepeatThreshold()
// 		{
// 			// throw the first point out since the camera is pre-warming
// 			if (PointIndex == 0 && trashedFirstPoint)
// 			{
// 				trashedFirstPoint = true;
// 				return false;
// 			}
//
// 			// data does not seem to gather the data properly on the first cycle
// 			if (currentRepeat < RepeatCount - 1)
// 				currentRepeat++;
// 			else
// 				currentRepeat = 0;
//
// 			return currentRepeat == 0;
// 		}
//
// 		protected void PreRenderSystems()
// 		{
// 			foreach (var l in layouts)
// 				l.PreRender();
// 		}
//
// 		protected void Completed()
// 		{
// 			IsRunning(false);
// 			IsComplete = true;
// 		}
//
// 		protected bool LayoutsComplete
// 		{
// 			get
// 			{
// 				foreach (var l in layouts)
// 					if (!l.jobDone)
// 					{
// 						Debug.Log(l.name + "-" + "Is Not Complete");
// 						return false;
// 					}
//
// 				return true;
// 			}
// 		}
//
// 		protected virtual ViewerBundleSystem CreateLayoutSystem(ViewerLayoutMono layoutMono)
// 		{
// 			layoutMono.transform.SetParent(transform);
//
// 			var layoutSys = layoutMono.gameObject.AddComponent<ViewerBundleSystem>();
//
// 			layoutSys.Init(PointCount, colors);
// 			// TODO: address break
// 			// layoutMono.Build(mono => layoutSys.AddAndSetViewer(mono));
//
// 			OnStartRun += layoutSys.StartRun;
// 			OnRunState += layoutSys.ChangeRunState;
// 			OnStageSet += value => layoutSys.stage = value;
//
// 			layoutSys.OnLayoutComplete += CheckSystemAtPoint;
//
// 			return layoutSys;
// 		}
//
// 		protected virtual void CheckSystemAtPoint()
// 		{
// 			if (!LayoutsComplete)
// 				return;
//
// 			Debug.Log("All Systems Done");
// 		}
//
// 		private void AttachAndSetViewerSystem(ViewerLayoutMono mono)
// 		{
// 			var layoutSys = CreateLayoutSystem(mono);
// 			if (layoutSys == null)
// 				return;
//
// 			layouts.Add(layoutSys);
// 			OnLayoutAdded?.Invoke(layoutSys);
// 		}
//
// 		public static BundleRunner Create(string name, List<IViewerLayout> layouts, List<Vector3> points, List<ViewColor> viewColors, bool debug)
// 		{
// 			if (!layouts.Valid() || !points.Valid() || !viewColors.Valid())
// 			{
// 				ViewConsole.Error("Viewer Setup Data could not be created, points or colors are not setup correctly");
// 				return null;
// 			}
//
// 			var nameWithColor = new List<ViewColorWithName>();
// 			var designsInScene = new List<ContentMono>();
// 			var content = FindObjectsOfType<ContentMono>();
//
// 			// Associate name of target to view color
// 			foreach (var vc in content)
// 			{
// 				if (vc.transform.hideFlags != HideFlags.None)
// 					continue;
//
// 				if (vc is ITargetContent)
// 				{
// 					if (nameWithColor.Count == 0)
// 						nameWithColor.Add(new ViewColorWithName(vc.viewColor, vc.viewName));
//
// 					else if (!nameWithColor.Any(x => x.content.Equals(vc.viewName)))
// 					{
// 						nameWithColor.Add(new ViewColorWithName(vc.viewColor, vc.viewName));
// 					}
// 				}
// 				else if (vc is IDesignContent)
// 				{
// 					designsInScene.Add(vc);
// 				}
// 			}
//
// 			BundleRunner runner;
//
// 			if (debug)
// 				runner = new GameObject($"DebugRunner-{name} ").AddComponent<BundleRunnerDebugger>();
// 			else
// 				runner = new GameObject($"Runner-{name} ").AddComponent<BundleRunnerRuntime>();
//
// 			var setup = new ViewerSetupData(points.ToArray(), viewColors.ToArray());
//
// 			runner.Init(layouts, setup, nameWithColor, designsInScene);
// 			return runner;
// 		}
// 	}
// }

