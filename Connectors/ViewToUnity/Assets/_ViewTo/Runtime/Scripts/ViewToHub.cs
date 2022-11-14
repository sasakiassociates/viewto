#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cysharp.Threading.Tasks;
using Objects.Geometry;
using Speckle.ConnectorUnity;
using Speckle.ConnectorUnity.Converter;
using Speckle.ConnectorUnity.Models;
using Speckle.ConnectorUnity.Ops;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Speckle.Core.Kits;
using Speckle.Core.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using ViewObjects;
using ViewTo.Connector.Unity.Commands;
using Debug = UnityEngine.Debug;
using VU = ViewObjects.Unity;
using VO = ViewObjects;
using VS = ViewObjects.Speckle;

#endregion

namespace ViewTo.Connector.Unity
{

  public class ViewToHub : MonoBehaviour
  {

    [SerializeField] Material _analysisMaterial;
    [SerializeField] Material _renderedMat;

    [field: SerializeField] public SpeckleStreamObject stream { get; private set; }

    [SerializeField] bool _loadResults = true;
    [SerializeField] bool _createCommit = true;
    [SerializeField] bool _runOnLoad = true;
    [SerializeField] bool _autoRun = false;

    [SerializeField] bool _runRepeatCommand;

    [SerializeField] VU.ViewStudy _study;
    VS.ViewStudy _speckleStudy;

    [SerializeField][HideInInspector] Rig _rig;

    [SerializeField] ScriptableSpeckleConverter _converterUnity;


    RepeatResultTester _tester;

    Stopwatch _timer;

    public Account account { get; set; }

    public bool canRun
    {
      get => _study != null && _study.IsValid && _rig != null && _rig.IsReady;
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

    public void TryLoadStudy(ViewObjects.Unity.ViewStudy obj)
    {
      if (!IsInit)
        Init();

      ViewConsole.Log($"{name} is loading View Study {obj.ViewName} ");
      _study = obj;

      if (_rig != null)
        VU.ViewObject.SafeDestroy(_rig.gameObject);

      _rig = new GameObject().AddComponent<Rig>();

      // setup events
      _rig.OnReady += RigReady;
      _rig.OnComplete += RigComplete;
      _rig.OnStudyLoaded += OnStudyLoaded;
      _rig.OnStageChange += OnRigStageChanged;
      _rig.OnContentLoaded += OnViewContentLoaded;
      _rig.OnActiveViewerSystem += OnActiveViewerSystem;
      _rig.OnDataReadyForCloud += (data) =>
      {
        if (_study != null)
        {
          var rc = _study.TrySetResults(data);
          if (rc != null)
          {
            SendResultsToStream(rc);
          }
        }
      };

      foreach (var c in _study.FindObjects<VU.Content>())
      {
        c.PrimeMeshData(_analysisMaterial);
      }

      var rig = (IRig)_rig;
      _study.LoadStudyToRig(ref rig);

      if (_runOnLoad)
      {
        StartRigSystemRun();
      }
    }

    public void StartRigSystemRun(int pointToStart = 0)
    {
      if (!canRun)
      {
        ViewConsole.Warn($"Hub not able to run study\nStudy Valid? {_study != null && _study.IsValid}\nRig Ready{_rig != null && _rig.IsReady}");
        return;
      }

      Application.targetFrameRate = 200;

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

    async UniTask AutoStart()
    {
      Debug.Log("Starting Hacking Version of View To");

      var client = new SpeckleUnityClient(stream.Account);
      client.token = this.GetCancellationTokenOnDestroy();

      var commit = await client.CommitGet(stream.Id, stream.Commit.id);

      if (commit == null)
        return;

      var @base = await SpeckleOps.Receive(client, stream.Id, commit.referencedObject);

      if (@base == null)
        return;

      Debug.Log($"Receive Done {@base.totalChildrenCount}");

      Debug.Log("Looking for object type");

      _speckleStudy = await @base.SearchForType<VS.ViewStudy>(true, client.token);

      if (_speckleStudy == null)
        return;

      Debug.Log($"Found Study {_speckleStudy.ViewName} with {_speckleStudy.Objects.Count} objects\n({_speckleStudy.ViewId})");

      var objectsToConvert = new List<IViewObject>();

      foreach (var obj in _speckleStudy.Objects)
      {
        var go = new GameObject();

        Debug.Log($"Object {obj.speckle_type}");

        switch (obj)
        {
          case VS.ContentReference o:
            var content = go.AddComponent<VU.Content>();
            content.ContentType = o.ContentType;
            await GetContentData(content, o, client, stream.Id);
            objectsToConvert.Add(content);
            break;
          case VS.ViewCloudReference o:
            var cloud = go.AddComponent<ViewObjects.Unity.ViewCloud>();
            cloud.ViewId = o.ViewId;
            cloud.Reference = o.References;

            var co = await ReceiveCommitWithData(client, stream.Id, cloud.Reference.FirstOrDefault());
            var pc = await co.SearchForType<Pointcloud>(true, this.GetCancellationTokenOnDestroy());

            cloud.Points = ArrayToCloudPoint(pc.points, pc.units).ToArray();
            objectsToConvert.Add(cloud);

            break;
          case VS.Viewer o:
            // TODO: Bypassing linked clouds and different layout types but should be fixed in the near future
            var bundle = go.AddComponent<VU.Viewer>();
            bundle.Layouts = new List<IViewerLayout> { new LayoutHorizontal() };
            objectsToConvert.Add(bundle);
            break;
          default:
            Debug.LogWarning($"Study object type {obj.speckle_type} was not converted");
            break;
        }
      }

      var studyToBuild = new GameObject().AddComponent<ViewObjects.Unity.ViewStudy>();
      studyToBuild.Objects = objectsToConvert;

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

    public static CloudPoint CloudByCoordinates(double x, double y, double z, string units) =>
      new((float)ScaleToNative(x, units), (float)ScaleToNative(y, units), (float)ScaleToNative(z, units));

    public static double ScaleToNative(double value, string units) => value * Units.GetConversionFactor(units, Units.Meters);

    static async UniTask<Base> ReceiveCommitWithData(SpeckleUnityClient client, string stream, string refId)
    {
      var refCommit = await client.CommitGet(stream, refId);
      return await SpeckleOps.Receive(client, stream, refCommit.referencedObject);
    }

    async UniTask GetContentData(ViewObjects.Unity.Content content, VO.Speckle.ContentReference contentBase, SpeckleUnityClient client, string stream)
    {
      content.ViewId = contentBase.ViewId;
      content.ViewName = contentBase.ViewName;
      content.ContentType = contentBase.ContentType;
      content.References = contentBase.References;

      foreach (var refId in content.References)
      {
        var referenceBase = await ReceiveCommitWithData(client, stream, refId);
        var hierarchy = await SpeckleOps.ConvertToScene(content.transform, referenceBase, _converterUnity, client.token);
        await UniTask.SwitchToMainThread();
        hierarchy.ParentAllObjects();
      }

      // gather objects for content to keep track
      content.Objects = GetKids(content.transform).ToList();
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


    void SendResultsToStream(VU.ResultCloud mono)
    {
      Debug.Log("Auto Send");

      // var layer = new GameObject("Result Layer").AddComponent<SpeckleLayer>();
      // layer.Add(mono.gameObject);
      // var node = new GameObject("Node").AddComponent<SpeckleNode>();
      // node.AddLayer(layer);

      var data = mono.Data
        .Select(x => new VS.ResultCloudData(x.Values, x.Option, x.Layout))
        .ToList();

      var resultCloud = new VS.ResultCloud() { Data = data, Points = mono.Points };
      _speckleStudy.Objects.Add(resultCloud);

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

            var res = await SpeckleOps.Send(client, new Base { ["Data"] = _speckleStudy }, stream.Id);

            await client.CommitCreate(new CommitCreateInput
            {
              objectId = res,
              message = $"{_study.ViewName} is complete! {mono.count}",
              branchName = stream.IsValid() && stream.Branch.Valid() ? stream.Branch.name : "main",
              sourceApplication = SpeckleUnity.APP,
              streamId = stream.Id
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

    void TryLoadResultCloud(ViewObjects.Unity.ResultCloud cloud)
    {
      if (cloud == null)
      {
        ViewConsole.Warn("Trying to load a null result cloud");
        return;
      }

      ViewConsole.Log($"Loading Result Cloud to explorer {cloud.name}");

      // OnContentBoundsSet?.Invoke(cloud.GetBounds());
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

        if (obj.GetComponent<ViewObjects.Unity.ViewStudy>() != null)
          TryLoadStudy(obj.GetComponent<ViewObjects.Unity.ViewStudy>());

        else if (obj.GetComponent<ViewObjects.Unity.ResultCloud>() != null)
          TryLoadResultCloud(obj.GetComponent<ViewObjects.Unity.ResultCloud>());
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

    void Awake()
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

      LoadStreams();

      if (_autoRun)
        AutoStart().Forget();
    }


    void LoadStreams()
    {

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

    public event UnityAction<ViewerSystem> OnActiveViewerSystem;

    public event UnityAction<ViewObjects.Unity.ViewStudy> OnStudyComplete;

    public event UnityAction<ResultStage> OnRigStageChanged;

    public event UnityAction<StudyLoadedArgs> OnStudyLoaded;

    public event UnityAction<ViewContentLoadedArgs> OnViewContentLoaded;

    public UnityEvent<Bounds> OnContentBoundsSet;

    public UnityEvent<ResultStage> OnResultStageSet;

    // public event EventHandler<RenderCameraEventArgs> OnMapCameraSet;

		#endregion

    #region macros

    public IEnumerable<Account> GetSpeckleAccounts() => AccountManager.GetAccounts();

 #endregion
  }

}
