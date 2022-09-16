#region

using System;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using Speckle.ConnectorUnity;
using Speckle.ConnectorUnity.Converter;
using Speckle.ConnectorUnity.Models;
using Speckle.ConnectorUnity.Ops;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using UnityEngine;
using UnityEngine.Events;
using ViewObjects;
using ViewObjects.Unity;
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

		[SerializeField] bool _runRepeatCommand;

		[SerializeField] ViewStudyMono _study;

		[SerializeField] [HideInInspector] RigSystem _rig;

		// [SerializeField] [HideInInspector] ResultExplorerMono _explorer;

		[SerializeField, HideInInspector] StreamAdapter _stream;

		[SerializeField] ScriptableSpeckleConverter _converterUnity;
		RepeatResultTester _tester;

		Stopwatch _timer;

		public bool canRun
		{
			get => _study != null && _study.isValid && _rig != null && _rig.isReady;
		}

		public bool inProcess { get; private set; }

		// public ResultExplorerMono explorer
		// {
		// 	get
		// 	{
		// 		if (_explorer != null)
		// 			return _explorer;
		//
		// 		var res = FindObjectOfType<ResultExplorerMono>();
		//
		// 		if (res == null)
		// 			res = new GameObject("ResultExplorer").AddComponent<ResultExplorerMono>();
		//
		// 		return res;
		// 	}
		// }

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

			ViewConsole.Log($"{name} is loading View Study {obj.viewName} ");
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

			_rig.Build(_study);

			if (_runOnLoad)
				Run();
		}

		public void Run()
		{
			if (!canRun)
			{
				ViewConsole.Warn($"Hub not able to run study\nStudy Valid? {_study != null && _study.isValid}\nRig Ready{_rig != null && _rig.isReady}");
				return;
			}

			ViewConsole.Log($"Starting Run for {_study.viewName}");

			inProcess = true;

			_timer ??= new Stopwatch();
			_timer.Start();

			_rig.Run();
		}

		void Init()
		{
			Instance = this;

			if (_analysisMaterial == null)
				_analysisMaterial = new Material(Resources.Load<Shader>("UnlitDoubleSided"));

			if (_renderedMat == null)
				_renderedMat = new Material(Shader.Find(@"Standard"));
		}

		async UniTask AutoStart()
		{
			var client = new SpeckleUnityClient(AccountManager.GetDefaultAccount());
			Debug.Log("Starting");
			client.token = this.GetCancellationTokenOnDestroy();
			var commit = await client.CommitGet(STREAM, COMMIT);
			Debug.Log("Commit Found");
			// var @base = await Operations.Receive(commit.referencedObject, this.GetCancellationTokenOnDestroy(), transport);
			var @base = await SpeckleOps.Receive(client, STREAM, commit.referencedObject);
			Debug.Log("Receive Done");

			var node = new GameObject().AddComponent<SpeckleObjectBehaviour>();
			_converterUnity.SetConverterSettings(new ScriptableConverterSettings { style = ConverterStyle.Queue });

			await node.DataToScene(@base, _converterUnity, this.GetCancellationTokenOnDestroy());

			WhatIsThis(node);
		}

		const string STREAM = Inglewood_Stream;
		const string BRANCH = Inglewood_Branch;
		const string COMMIT = Inglewood_Commit;

		const string BPY_Stream = "96855cab4a";
		const string BPY_Branch_1 = "viewstudy/largewaterfront";
		const string BPY_Commit_1 =  "deb54ce87c";
		const string BPY_Branch_2 = "viewstudy/gridofparks";
		const string BPY_Commit_2 =  "dafc49783b";
		const string BPY_Branch_3 = "viewstudy/eastwestparks";
		const string BPY_Commit_3 =  "f5b36c93d2";

		const string BCHP_Stream = "9b692137ca";
		const string BCHP_Commit = "031307d6d5";
		const string BCHP_Branch = "viewstudy/all-targets";

		const string Inglewood_Stream = "4777dea055";
		const string Inglewood_Commit = "c3258e3979";
		const string Inglewood_Branch = "viewstudy/massing-from-road";
		
		const string TEST_Stream = "1da7b18b31";
		const string TEST_Commit = "1518e1cc4c";
		const string TEST_Branch = "viewstudy/sphere";

		void SendResultsToStream(ResultCloudMono mono)
		{
			Debug.Log("Auto Send");

			var layer = new GameObject("Result Layer").AddComponent<SpeckleLayer>();
			layer.Add(mono.gameObject);
			var node = new GameObject("Node").AddComponent<SpeckleNode>();
			node.AddLayer(layer);

			var client = new SpeckleUnityClient(AccountManager.GetDefaultAccount());
			try
			{
				UniTask.Create(async () =>
				{
					ViewConsole.Log($"Before: Has view cloud ? {mono.GetComponent<ViewCloudMono>() != null}");

					await UniTask.Yield(PlayerLoopTiming.LastUpdate);

					ViewConsole.Log($"After: Has view cloud ? {mono.GetComponent<ViewCloudMono>() != null}");

					var @base = node.SceneToData(_converterUnity, this.GetCancellationTokenOnDestroy());

					var res = await SpeckleOps.Send(client, @base, STREAM);

					await client.CommitCreate(new CommitCreateInput()
					{
						objectId = res,
						message = $"{_study.viewName} is complete! {mono.count}",
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

		public event UnityAction<ViewStudyMono> OnStudyComplete;

		public event UnityAction<RigStage> OnRigStageChanged;

		public event UnityAction<StudyLoadedArgs> OnStudyLoaded;

		public event UnityAction<ViewContentLoadedArgs> OnViewContentLoaded;

		public UnityEvent<Bounds> OnContentBoundsSet;

		public UnityEvent<ResultStage> OnResultStageSet;

		// public event EventHandler<RenderCameraEventArgs> OnMapCameraSet;

		#endregion

	}

}