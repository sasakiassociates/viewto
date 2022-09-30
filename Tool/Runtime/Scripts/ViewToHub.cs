#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cysharp.Threading.Tasks;
using Objects.Geometry;
using Speckle.ConnectorUnity;
using Speckle.ConnectorUnity.Converter;
using Speckle.ConnectorUnity.Ops;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Speckle.Core.Kits;
using Speckle.Core.Models;
using UnityEngine;
using UnityEngine.Events;
using ViewObjects;
using ViewObjects.Speckle;
using ViewObjects.Unity;
using ViewObjects.Viewer;
using ViewTo.Connector.Unity.Commands;
using Debug = UnityEngine.Debug;

#endregion

namespace ViewTo.Connector.Unity
{

	public class ViewToHub : MonoBehaviour
	{

		[SerializeField] Material _analysisMaterial;
		[SerializeField] Material _renderedMat;

		[SerializeField] bool _loadResults = true;
		[SerializeField] bool _createCommit = true;
		[SerializeField] bool _runOnLoad = true;
		[SerializeField] bool _autoRun = false;

		[SerializeField] bool _runRepeatCommand;

		[SerializeField] ViewStudyMono _study;

		[SerializeField] [HideInInspector] RigSystem _rig;

		// [SerializeField] [HideInInspector] ResultExplorerMono _explorer;

		[SerializeField] ScriptableSpeckleConverter _converterUnity;
		RepeatResultTester _tester;

		Stopwatch _timer;

		ViewStudyBase_v2 _studyBase;

		public bool canRun
		{
			get => _study != null && _study.isValid && _rig != null && _rig.isReady;
		}

		public bool inProcess { get; private set; }

		#region static methods

		public static ViewToHub Initialize()
		{
			var hub = FindObjectOfType<ViewToHub>();

			if (hub == null)
				hub = new GameObject("ViewToHub").AddComponent<ViewToHub>();

			hub.Init();

			return hub;
		}

		#endregion

		public void TryLoadStudy(ViewStudyMono obj)
		{
			if (!IsInit)
				Init();

			ViewConsole.Log($"{name} is loading View Study {obj.ViewName} ");
			_study = obj;
			_study.OnResultsSet += SendResultsToStream;

			if (_rig != null)
				ViewObjMonoExt.SafeDestroy(_rig.gameObject);

			_rig = new GameObject().AddComponent<RigSystem>();

			// setup events
			_rig.OnReady += RigReady;
			_rig.OnComplete += RigComplete;
			_rig.OnStudyLoaded += OnStudyLoaded;
			_rig.OnStageChange += OnRigStageChanged;
			_rig.OnContentLoaded += OnViewContentLoaded;
			_rig.OnActiveViewerSystem += OnActiveViewerSystem;

			_rig.Build(_study);

			if (_runOnLoad)
				StartRigSystemRun();
		}

		public void StartRigSystemRun(int pointToStart = 0)
		{
			if (!canRun)
			{
				ViewConsole.Warn($"Hub not able to run study\nStudy Valid? {_study != null && _study.isValid}\nRig Ready{_rig != null && _rig.isReady}");
				return;
			}

			ViewConsole.Log($"Starting Run for {_study.ViewName}");

			inProcess = true;

			_timer ??= new Stopwatch();
			_timer.Start();

			_rig.Run(pointToStart, _autoRun);
		}

		void Init()
		{
			Instance = this;

			if (_analysisMaterial == null)
				_analysisMaterial = new Material(Resources.Load<Shader>("UnlitDoubleSided"));

			if (_renderedMat == null)
				_renderedMat = new Material(Shader.Find(@"Standard"));
		}

		async UniTask GetBrepTest()
		{
			var client = new SpeckleUnityClient(AccountManager.GetDefaultAccount());

			client.token = this.GetCancellationTokenOnDestroy();

			var commit = await client.CommitGet(STREAM, "96e1ed10be");

			var @base = await SpeckleOps.Receive(client, STREAM, commit.referencedObject);

			await SpeckleOps.ConvertToScene(new GameObject("Brep Test").transform, @base, _converterUnity, client.token);
		}

		async UniTask AutoStart()
		{
			Debug.Log("Starting Hacking Version of View To");
			Debug.Log($"Stream {STREAM}\nCommit {COMMIT}\nBranch {BRANCH}");

			var client = new SpeckleUnityClient(AccountManager.GetDefaultAccount());

			client.token = this.GetCancellationTokenOnDestroy();

			var commit = await client.CommitGet(STREAM, COMMIT);

			if (commit == null) return;

			Debug.Log($"Commit Found {commit.id}");

			var @base = await SpeckleOps.Receive(client, STREAM, commit.referencedObject);

			if (@base == null) return;

			Debug.Log($"Receive Done {@base.totalChildrenCount}");

			Debug.Log("Looking for object type");

			_studyBase = await @base.SearchForType<ViewStudyBase_v2>(true, client.token);

			if (_studyBase == null) return;

			Debug.Log($"Found Study {_studyBase.ViewName} with {_studyBase.Objects.Count} objects\n({_studyBase.ViewId})");

			var objectsToConvert = new List<IViewObj>();

			var contents = new List<IViewContent>();

			foreach (var obj in _studyBase.Objects)
			{
				var go = new GameObject();

				Debug.Log($"Object {obj.speckle_type}");

				switch (obj)
				{
					case ContentBase_v2 o:
						ContentMono content;

						// TODO: Remove different object types when ViewCore commands have been updated
						switch (o.ContentType)
						{
							case ContentType.Target:

								content = go.AddComponent<TargetContentMono>();
								await GetContentData(content, o, client, STREAM);
								contents.Add((IViewContent)content);

								break;
							case ContentType.Existing:

								content = go.AddComponent<BlockerContentMono>();
								await GetContentData(content, o, client, STREAM);
								contents.Add((IViewContent)content);

								break;
							case ContentType.Proposed:

								content = go.AddComponent<DesignContentMono>();
								await GetContentData(content, o, client, STREAM);
								contents.Add((IViewContent)content);

								break;
							default:
								Debug.Log("Not supported");
								return;
						}

						break;
					case ViewCloudBase_v2 o:
						var cloud = go.AddComponent<ViewCloudMono>();
						cloud.ViewId = Guid.NewGuid().ToString();

						var baseCloud = await ReceiveCommitWithData(client, STREAM, o.References.FirstOrDefault());

						var pc = await baseCloud.SearchForType<Pointcloud>(true, client.token);

						if (pc != null)
						{
							cloud.points = ArrayToCloudPoint(pc.points, pc.units).ToArray();
							objectsToConvert.Add(cloud);
						}
						else
						{
							Debug.Log("Did not Find cloud");
						}

						break;
					case ViewerSystemBase_v2 o:
						// TODO: Bypassing linked clouds and different layout types but should be fixed in the near future
						var bundle = go.AddComponent<ViewerBundleMono>();
						bundle.layouts = new List<IViewerLayout>
							{ new ViewerLayoutHorizontal() };
						objectsToConvert.Add(bundle);
						break;
					default:
						Debug.LogWarning($"Study object type {obj.speckle_type} was not converted");
						break;
				}
			}

			var contentBundle = new GameObject("Content Bundle").AddComponent<ContentBundleMono>();
			contentBundle.contents = contents;
			objectsToConvert.Add(contentBundle);

			var studyToBuild = new GameObject().AddComponent<ViewStudyMono>();
			studyToBuild.objs = objectsToConvert;

			TryLoadStudy(studyToBuild);
		}

		public static IEnumerable<CloudPoint> ArrayToCloudPoint(IReadOnlyCollection<double> arr, string units)
		{
			if (arr == null)
				throw new Exception("point array is not valid ");

			if (arr.Count % 3 != 0)
				throw new Exception("Array malformed: length%3 != 0.");

			var points = new CloudPoint[arr.Count / 3];
			var asArray = arr.ToArray();

			for (int i = 2, k = 0; i < arr.Count; i += 3)
				points[k++] = CloudByCoordinates(asArray[i - 2], asArray[i - 1], asArray[i], units);

			return points;
		}

		public static List<double> ToSpeckle(IEnumerable<CloudPoint> points)
		{
			var res = new List<double>();

			if (points == null)
			{
				Debug.LogException(new Exception("point array is not valid "));
				return res;
			}

			foreach (var point in points)
			{
				res.Add(point.x);
				res.Add(point.y);
				res.Add(point.z);
			}

			return res;
		}

		public static CloudPoint CloudByCoordinates(double x, double y, double z, string units) =>
			new((float)ScaleToNative(x, units), (float)ScaleToNative(y, units), (float)ScaleToNative(z, units));

		public static double ScaleToNative(double value, string units) => value * Units.GetConversionFactor(units, Units.Meters);

		static async UniTask<Base> ReceiveCommitWithData(SpeckleUnityClient client, string stream, string refId)
		{
			var refCommit = await client.CommitGet(stream, refId);
			return await SpeckleOps.Receive(client, stream, refCommit.referencedObject);
		}

		async UniTask GetContentData(ContentMono content, ContentBase_v2 contentBase, SpeckleUnityClient client, string stream)
		{
			content.ViewId = contentBase.ViewId;
			content.ViewName = contentBase.ViewName;
			content.ContentType = contentBase.ContentType;
			content.References = contentBase.References;

			foreach (var refId in content.References)
			{
				var referenceBase = await ReceiveCommitWithData(client, stream, refId);
				await UniTask.Yield();
				var hierarchy = await SpeckleOps.ConvertToScene(content.transform, referenceBase, _converterUnity, client.token);
				hierarchy.ParentAllObjects();
			}

			// gather objects for content to keep track
			content.objects = GetKids(content.transform).Cast<object>().ToList();
			UniTask.Yield();
		}

		static List<GameObject> GetKids(Transform parent)
		{
			var currentList = new List<GameObject>();

			foreach (Transform child in parent)
			{
				currentList.Add(child.gameObject);
				if (child.childCount > 0)
					currentList.AddRange(GetKids(child));
			}

			return currentList;
		}

		async UniTask AutoStartViewStudy()
		{
			var client = new SpeckleUnityClient(AccountManager.GetDefaultAccount());
			Debug.Log("Starting");
			client.token = this.GetCancellationTokenOnDestroy();
			var commit = await client.CommitGet(STREAM, COMMIT);
			Debug.Log("Commit Found");
			// var @base = await Operations.Receive(commit.referencedObject, this.GetCancellationTokenOnDestroy(), transport);
			var @base = await SpeckleOps.Receive(client, STREAM, commit.referencedObject);

			var node = new GameObject().AddComponent<SpeckleObjectBehaviour>();
			_converterUnity.SetConverterSettings(new ScriptableConverterSettings { style = ConverterStyle.Queue });

			await node.ConvertToScene(@base, _converterUnity, this.GetCancellationTokenOnDestroy());

			WhatIsThis(node);
		}

		const string STREAM = Inglewood_Stream;
		const string BRANCH = Inglewood_Branch;
		const string COMMIT = Inglewood_Commit;

		const string BPY_Stream = "96855cab4a";
		const string BPY_Branch_1 = "viewstudy/largewaterfront";
		const string BPY_Commit_1 = "deb54ce87c";
		const string BPY_Branch_2 = "viewstudy/gridofparks";
		const string BPY_Commit_2 = "dafc49783b";
		const string BPY_Branch_3 = "viewstudy/eastwestparks";
		const string BPY_Commit_3 = "f5b36c93d2";

		const string BCHP_Stream = "9b692137ca";
		const string BCHP_Commit = "031307d6d5";
		const string BCHP_Branch = "viewstudy/all-targets";

		const string Inglewood_Stream = "4777dea055";
		const string Inglewood_Commit = "e86f9ecc1f";
		const string Inglewood_Branch = "viewstudy/massing-from-road";

		const string UPH_Stream = "a823053e07";
		const string UPH_Commit = "3b5406b590";
		const string UPH_Branch = "viewstudies/road-way";

		const string TEST_Stream = "1da7b18b31";
		const string TEST_Commit = "1518e1cc4c";
		const string TEST_Branch = "viewstudy/sphere";

		void SendResultsToStream(ResultCloudMono mono)
		{
			Debug.Log("Auto Send");

			// var layer = new GameObject("Result Layer").AddComponent<SpeckleLayer>();
			// layer.Add(mono.gameObject);
			// var node = new GameObject("Node").AddComponent<SpeckleNode>();
			// node.AddLayer(layer);

			var data = mono.data
				.Select(x => new ResultPixelBase_v2(x.values, x.content, (ResultStage)Enum.Parse(typeof(ResultStage), x.stage), x.layout))
				.Cast<IResultCloudData>()
				.ToList();

			var resultCloud = new ResultCloudBase_v2 { Data = data, Points = ToSpeckle(mono.points) };
			_studyBase.Objects.Add(resultCloud);

			if (_createCommit)
			{
				var client = new SpeckleUnityClient(AccountManager.GetDefaultAccount());
				try
				{
					UniTask.Create(async () =>
					{
						await UniTask.Yield(PlayerLoopTiming.LastUpdate);

						// // TODO: fix this so no converter is needed
						// var @base = node.SceneToData(_converterUnity, this.GetCancellationTokenOnDestroy());

						var res = await SpeckleOps.Send(client, new Base { ["Data"] = resultCloud }, STREAM);

						await client.CommitCreate(new CommitCreateInput()
						{
							objectId = res,
							message = $"{_study.ViewName} is complete! {mono.count}",
							branchName = BRANCH,
							sourceApplication = SpeckleUnity.APP,
							streamId = STREAM
						});
					});
				}
				catch (Exception e)
				{
					Debug.Log(e.Message);
				}
			}
		}

		void RigReady()
		{
			OnRigBuilt?.Invoke(_rig);
		}

		void RigComplete()
		{
			ViewConsole.Log("Rig Complete");

			_timer.Stop();
			var ts = _timer.Elapsed;
			ViewConsole.Log("Runtime-" + $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}");

			if (_runRepeatCommand)
			{
				ViewConsole.Log("Running Second Pass");

				_runRepeatCommand = false;
				_runOnLoad = false;

				TryLoadStudy(_study);
				_rig.Run();
				return;
			}

			inProcess = false;

			OnStudyComplete?.Invoke(_study);
		}

		void TryLoadResultCloud(ResultCloudMono cloud)
		{
			if (cloud == null)
			{
				ViewConsole.Warn("Trying to load a null result cloud");
				return;
			}

			ViewConsole.Log($"Loading Result Cloud to explorer {cloud.name}");

			OnContentBoundsSet?.Invoke(cloud.GetBounds());
			// explorer.Attach(cloud);
		}

		void ProcessReceiver(Receiver r)
		{
			// r.OnStreamUpdated += s =>
			// {
			// 	ViewConsole.Log($"Stream Updated {s}");
			// 	// _stream = s;
			// };
			r.OnNodeComplete += WhatIsThis;
		}

		void WhatIsThis(SpeckleObjectBehaviour node)
		{
			var data = node.hierarchy.GetObjects();

			ViewConsole.Log($"{data.Count}");

			foreach (var obj in data)
			{
				if (obj == null)
					continue;

				if (obj.GetComponent<ViewStudyMono>() != null)
					TryLoadStudy(obj.GetComponent<ViewStudyMono>());

				else if (obj.GetComponent<ResultCloudMono>() != null)
					TryLoadResultCloud(obj.GetComponent<ResultCloudMono>());
			}
		}

		void ProcessSender(Sender s)
		{
			ViewConsole.Log($"Nothing set for {nameof(ProcessSender)} function");
		}

		#region static props

		public static Material AnalysisMat
		{
			get => Instance._analysisMaterial;
		}

		public static Material RenderedMat
		{
			get => Instance._renderedMat;
		}

		public static ViewToHub Instance { get; set; }

		public static bool IsInit
		{
			get => Instance != null && AnalysisMat != null && RenderedMat != null;
		}

		#endregion

		#region unity methods

		void OnEnable()
		{
			Instance = this;

			if (!IsInit)
				Init();

			// OnStudyComplete += mono => ViewConsole.Log($"{mono} is complete!");
			//
			// if (_speckleConnector == null)
			// 	return;
			//
			// // hacky way of getting connector data
			// _speckleConnector.OnReceiverCreated += ProcessReceiver;
			// _speckleConnector.OnSenderCreated += ProcessSender;
		}

		void Start()
		{
			// AutoStartViewStudy().Forget();

			AutoStart().Forget();
		}

		void OnDisable()
		{
			if (_rig != null)
			{
				// setup events
				_rig.OnReady -= RigReady;
				_rig.OnComplete -= RigComplete;
				_rig.OnStudyLoaded -= OnStudyLoaded;
				_rig.OnStageChange -= OnRigStageChanged;
				_rig.OnContentLoaded -= OnViewContentLoaded;
			}

			// if (_speckleConnector != null)
			// {
			// 	_speckleConnector.OnReceiverCreated -= ProcessReceiver;
			// 	_speckleConnector.OnSenderCreated -= ProcessSender;
			// }
		}

		#endregion

		#region Events

		public event UnityAction OnRigReady;

		public event UnityAction OnRigComplete;

		public event UnityAction<IRigSystem> OnRigBuilt;

		public event UnityAction<ViewerSystemMono> OnActiveViewerSystem;

		public event UnityAction<ViewStudyMono> OnStudyComplete;

		public event UnityAction<ResultStage> OnRigStageChanged;

		public event UnityAction<StudyLoadedArgs> OnStudyLoaded;

		public event UnityAction<ViewContentLoadedArgs> OnViewContentLoaded;

		public UnityEvent<Bounds> OnContentBoundsSet;

		public UnityEvent<ResultStage> OnResultStageSet;

		// public event EventHandler<RenderCameraEventArgs> OnMapCameraSet;

		#endregion

	}

}