using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using Objects.Geometry;
using Pcx;
using Speckle.ConnectorUnity;
using Speckle.ConnectorUnity.Converter;
using Speckle.ConnectorUnity.Models;
using Speckle.ConnectorUnity.Ops;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Speckle.Core.Kits;
using Speckle.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using ViewObjects;
using ViewObjects.Clouds;
using ViewObjects.Systems;
using ViewObjects.Systems.Layouts;
using ViewTo.Connector.Unity.Commands;
using VU = ViewObjects.Unity;
using VO = ViewObjects;
using VS = ViewObjects.Speckle;

namespace ViewTo.Connector.Unity
{

  public class ViewToHub : MonoBehaviour
  {

    [SerializeField] Material analysisMaterial;
    [SerializeField] Material renderedMat;
    [SerializeField] string tempStreamId = "81c40b04df";
    [SerializeField] string tempCommitId = "bab6b9a0a2";
    [SerializeField, Range(60, 300)] int tempFrameRate = 200;
    [SerializeField] PointCloudRenderer renderer;

    [SerializeField] bool createCommit = true;
    [SerializeField] bool runRepeatCommand;

    [SerializeField] VU.ViewStudy study;

    [SerializeField][HideInInspector] Rig rig;

    [SerializeField] ScriptableConverter converterUnity;

    int _cachedFrameRate;
    VS.ViewStudy _speckleStudy;


    RepeatResultTester _tester;

    Stopwatch _timer;

    [field: SerializeField] public ResultExplorer explorer { get; private set; }

    [field: SerializeField] public SpeckleStreamObject streamObject { get; private set; }

    public SpeckleStream stream { get; set; }

    public bool canRun
    {
      get => study != null && study.IsValid && rig != null && rig.IsReady;
    }

    public bool inProcess { get; private set; }

  #region static methods

    public static ViewToHub Initialize()
    {
      var hub = FindObjectOfType<ViewToHub>();

      if(hub == null)
        hub = new GameObject("ViewToHub").AddComponent<ViewToHub>();

      hub.Init();

      return hub;
    }

  #endregion

    [Button]
    public void LoadResults() => AutoLoadResults().Forget();

    [Button]
    public void LoadStudy() => AutoLoadStudy().Forget();

    public void TryLoadStudy(ViewObjects.Unity.ViewStudy obj)
    {
      if(!IsInit) Init();

      ViewConsole.Log($"{name} is loading View Study {obj.ViewName} ");
      study = obj;

      if(this.rig != null)
        VU.ViewObject.SafeDestroy(this.rig.gameObject);

      this.rig = new GameObject().AddComponent<Rig>();

      // setup events
      this.rig.OnReady += RigReady;
      this.rig.OnComplete += RigComplete;
      this.rig.OnStudyLoaded += OnStudyLoaded;
      this.rig.OnStageChange += OnRigStageChanged;
      this.rig.OnContentLoaded += OnViewContentLoaded;
      this.rig.OnActiveViewerSystem += OnActiveViewerSystem;
      this.rig.OnDataReadyForCloud += (data) =>
      {
        if(study != null)
        {
          var rc = study.TrySetResults(data);
          if(rc != null)
          {
            SendResultsToStream(rc);
          }
        }
      };

      foreach(var c in study.FindObjects<VU.Content>())
      {
        c.PrimeMeshData(analysisMaterial);
      }

      IRig r = this.rig;
      study.LoadStudyToRig(ref r);

      StartRigSystemRun();
    }

    public void StartRigSystemRun(int pointToStart = 0)
    {
      if(!canRun)
      {
        ViewConsole.Warn($"Hub not able to run study\nStudy Valid? {study != null && study.IsValid}\nRig Ready{rig != null && rig.IsReady}");
        return;
      }

      Application.targetFrameRate = 200;

      ViewConsole.Log($"Starting Run for {study.ViewName}");

      inProcess = true;

      _timer ??= new Stopwatch();
      _timer.Start();

      rig.Run(pointToStart);
    }

    void Init()
    {
      Instance = this;

      if(analysisMaterial == null)
        analysisMaterial = new Material(Resources.Load<Shader>("UnlitDoubleSided"));

      if(renderedMat == null)
        renderedMat = new Material(Shader.Find(@"Standard"));
    }

    async UniTask AutoLoadResults()
    {
      streamObject = ScriptableObject.CreateInstance<SpeckleStreamObject>();
      await streamObject.Initialize("https://speckle.xyz/streams/" + tempStreamId + "/commits/" + tempCommitId);

      var client = new SpeckleClient(streamObject.baseAccount);
      client.token = this.GetCancellationTokenOnDestroy();

      var commit = await client.CommitGet(streamObject.Id, streamObject.Commit.id);

      if(commit == null) return;

      var @base = await SpeckleOps.Receive(client, streamObject.Id, commit.referencedObject);

      if(@base == null) return;

      _speckleStudy = await @base.SearchForType<VS.ViewStudy>(true, client.token);

      if(_speckleStudy == null) return;

      UnityEngine.Debug.Log($"Found Study {_speckleStudy.ViewName} with {_speckleStudy.Objects.Count} objects\n({_speckleStudy.ViewId})");
      study = new GameObject("Study").AddComponent<VU.ViewStudy>();
      study.transform.position = Vector3.zero;
      study.ViewId = _speckleStudy.ViewId;
      study.ViewName = _speckleStudy.ViewName;
      study.Objects = new List<IViewObject>();

      var mono = new List<IViewObject>();

      foreach(var o in _speckleStudy.Objects)
      {
        if(o is VS.ResultCloud cloud)
        {
          UnityEngine.Debug.Log("Found Cloud");
          var rc = new GameObject("ResultCloud").AddComponent<VU.ResultCloud>();
          rc.transform.position = Vector3.zero;
          rc.Points = cloud.Points;
          cloud.Data.ForEach(x => rc.AddResultData(x));
          rc.transform.SetParent(study.transform);
          mono.Add(rc);
        }
      }

      study.Objects = mono;


      if(explorer != null)
      {
        explorer.Load(study);
      }
    }

    async UniTask AutoLoadStudy()
    {
      UnityEngine.Debug.Log("Starting Hacking Version of View To");

      streamObject = ScriptableObject.CreateInstance<SpeckleStreamObject>();
      await streamObject.Initialize("https://speckle.xyz/streams/" + tempStreamId + "/commits/" + tempCommitId);

      var client = new SpeckleClient(streamObject.baseAccount);
      client.token = this.GetCancellationTokenOnDestroy();

      var commit = await client.CommitGet(streamObject.Id, streamObject.Commit.id);

      if(commit == null)
        return;

      var @base = await SpeckleOps.Receive(client, streamObject.Id, commit.referencedObject);

      if(@base == null)
        return;

      UnityEngine.Debug.Log($"Receive Done {@base.totalChildrenCount}");

      UnityEngine.Debug.Log("Looking for object type");

      _speckleStudy = await @base.SearchForType<VS.ViewStudy>(true, client.token);

      if(_speckleStudy == null)
        return;

      UnityEngine.Debug.Log($"Found Study {_speckleStudy.ViewName} with {_speckleStudy.Objects.Count} objects\n({_speckleStudy.ViewId})");

      var objectsToConvert = new List<IViewObject>();

      foreach(var obj in _speckleStudy.Objects)
      {
        var go = new GameObject();

        UnityEngine.Debug.Log($"Object {obj.speckle_type}");

        switch(obj)
        {
          case VS.ContentReference o:
            var content = go.AddComponent<VU.Content>();
            content.ContentType = o.ContentType;
            await GetContentData(content, o, client, streamObject.Id);
            objectsToConvert.Add(content);
            break;
          case VS.ViewCloudReference o:
            var cloud = go.AddComponent<ViewObjects.Unity.ViewCloud>();
            cloud.ViewId = o.ViewId;
            cloud.Reference = o.References;

            var co = await ReceiveCommitWithData(client, streamObject.Id, cloud.Reference.FirstOrDefault());
            var pc = await co.SearchForType<Pointcloud>(true, this.GetCancellationTokenOnDestroy());

            cloud.Points = ArrayToCloudPoint(pc.points, pc.units).ToArray();
            objectsToConvert.Add(cloud);

            break;
          case VS.Viewer o:
            // TODO: Bypassing linked clouds and different layout types but should be fixed in the near future
            var bundle = go.AddComponent<VU.Viewer>();
            bundle.Layouts = new List<ILayout> {new LayoutHorizontal()};
            objectsToConvert.Add(bundle);
            break;
          default:
            UnityEngine.Debug.LogWarning($"Study object type {obj.speckle_type} was not converted");
            break;
        }
      }

      var studyToBuild = new GameObject().AddComponent<ViewObjects.Unity.ViewStudy>();
      studyToBuild.Objects = objectsToConvert;

      TryLoadStudy(studyToBuild);
    }

    void SendResultsToStream(VU.ResultCloud mono)
    {
      UnityEngine.Debug.Log("Auto Send");

      // var layer = new GameObject("Result Layer").AddComponent<SpeckleLayer>();
      // layer.Add(mono.gameObject);
      // var node = new GameObject("Node").AddComponent<SpeckleNode>();
      // node.AddLayer(layer);

      var data = mono.Data
        .Select(x => new VS.ResultCloudData(x.Values, x.Option, x.Layout))
        .ToList();

      var resultCloud = new VS.ResultCloud() {Data = data, Points = mono.Points};
      _speckleStudy.Objects.Add(resultCloud);

      if(createCommit)
      {
        var client = new SpeckleClient(AccountManager.GetDefaultAccount());
        try
        {
          UniTask.Create(async () =>
          {
            await UniTask.Yield(PlayerLoopTiming.LastUpdate);

            // // TODO: fix this so no converter is needed
            // var @base = node.SceneToData(_converterUnity, this.GetCancellationTokenOnDestroy());

            var res = await SpeckleOps.Send(client, new Base {["Data"] = _speckleStudy}, streamObject.Id);

            await client.CommitCreate(new CommitCreateInput
            {
              objectId = res,
              message = $"{study.ViewName} is complete! {mono.count}",
              branchName = streamObject.IsValid() && streamObject.Branch.Valid() ? streamObject.Branch.name : "main",
              sourceApplication = SpeckleUnity.APP,
              streamId = streamObject.Id
            });
          });
        }
        catch(Exception e)
        {
          UnityEngine.Debug.Log(e.Message);
        }
      }
    }

    void RigReady()
    {
      OnRigBuilt?.Invoke(rig);
    }

    void RigComplete()
    {
      ViewConsole.Log("Rig Complete");

      _timer.Stop();
      var ts = _timer.Elapsed;
      ViewConsole.Log("Runtime-" + $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}");

      if(runRepeatCommand)
      {
        ViewConsole.Log("Running Second Pass");

        runRepeatCommand = false;

        TryLoadStudy(study);
        rig.Run();
        return;
      }

      inProcess = false;

      OnStudyComplete?.Invoke(study);
    }


    void TryLoadResultCloud(ViewObjects.Unity.ResultCloud cloud)
    {
      if(cloud == null)
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

      foreach(var obj in data)
      {
        if(obj == null)
          continue;

        if(obj.GetComponent<ViewObjects.Unity.ViewStudy>() != null)
          TryLoadStudy(obj.GetComponent<ViewObjects.Unity.ViewStudy>());

        else if(obj.GetComponent<ViewObjects.Unity.ResultCloud>() != null)
          TryLoadResultCloud(obj.GetComponent<ViewObjects.Unity.ResultCloud>());
      }
    }

    void ProcessSender(Sender s)
    {
      ViewConsole.Log($"Nothing set for {nameof(ProcessSender)} function");
    }


  #region Converter shit that should be moved away

    public static IEnumerable<CloudPoint> ArrayToCloudPoint(IReadOnlyCollection<double> arr, string units)
    {
      if(arr == null)
        throw new Exception("point array is not valid ");

      if(arr.Count % 3 != 0)
        throw new Exception("Array malformed: length%3 != 0.");

      var points = new CloudPoint[arr.Count / 3];
      var asArray = arr.ToArray();

      for(int i = 2, k = 0; i < arr.Count; i += 3)
        points[k++] = CloudByCoordinates(asArray[i - 2], asArray[i - 1], asArray[i], units);

      return points;
    }


    public static CloudPoint CloudByCoordinates(double x, double y, double z, string units) =>
      new((float)ScaleToNative(x, units), (float)ScaleToNative(y, units), (float)ScaleToNative(z, units));


    public static double ScaleToNative(double value, string units) => value * Units.GetConversionFactor(units, Units.Meters);


    static async UniTask<Base> ReceiveCommitWithData(SpeckleClient client, string stream, string refId)
    {
      var refCommit = await client.CommitGet(stream, refId);
      return await SpeckleOps.Receive(client, stream, refCommit.referencedObject);
    }


    async UniTask GetContentData(ViewObjects.Unity.Content content, VO.Speckle.ContentReference contentBase, SpeckleClient client, string stream)
    {
      content.ViewId = contentBase.ViewId;
      content.ViewName = contentBase.ViewName;
      content.ContentType = contentBase.ContentType;
      content.References = contentBase.References;

      foreach(var refId in content.References)
      {
        var referenceBase = await ReceiveCommitWithData(client, stream, refId);
        var hierarchy = await SpeckleOps.ConvertToScene(content.transform, referenceBase, converterUnity, client.token);
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

      foreach(Transform child in parent)
      {
        currentList.Add(child.gameObject);
        if(child.childCount > 0)
          currentList.AddRange(GetKids(child));
      }

      return currentList;
    }

  #endregion

  #region static props

    public static Material AnalysisMat
    {
      get => Instance.analysisMaterial;
    }

    public static Material RenderedMat
    {
      get => Instance.renderedMat;
    }

    public static ViewToHub Instance { get; set; }

    public static bool IsInit
    {
      get => Instance != null && AnalysisMat != null && RenderedMat != null;
    }

  #endregion

  #region unity methods

    SpeckleConnector GetConnector()
    {
      var connector = SpeckleConnector.instance;
      if(connector == null)
      {
        connector = new GameObject("Speckle Connector").AddComponent<SpeckleConnector>();
      }

      return connector;
    }

    void Awake()
    {
      Instance = this;

      _cachedFrameRate = tempFrameRate;
      Application.targetFrameRate = tempFrameRate;
      if(!IsInit)
        Init();
    }

    void Update()
    {
      if(_cachedFrameRate == tempFrameRate) return;

      _cachedFrameRate = tempFrameRate;
      Application.targetFrameRate = _cachedFrameRate;
    }

    void OnDisable()
    {
      if(rig != null)
      {
        // setup events
        rig.OnReady -= RigReady;
        rig.OnComplete -= RigComplete;
        rig.OnStudyLoaded -= OnStudyLoaded;
        rig.OnStageChange -= OnRigStageChanged;
        rig.OnContentLoaded -= OnViewContentLoaded;
      }
    }

  #endregion

  #region Events

    public event UnityAction<List<VU.ViewStudy>> OnStudiesFound;
    public event UnityAction OnRigReady;

    public event UnityAction OnRigComplete;

    public event UnityAction<IRigSystem> OnRigBuilt;

    public event UnityAction<ViewerSystem> OnActiveViewerSystem;

    public event UnityAction<ViewObjects.Unity.ViewStudy> OnStudyComplete;

    public event UnityAction<ContentType> OnRigStageChanged;

    public event UnityAction<StudyLoadedArgs> OnStudyLoaded;

    public event UnityAction<ViewContentLoadedArgs> OnViewContentLoaded;

    // public UnityEvent<Bounds> OnContentBoundsSet;
    //
    // public UnityEvent<ContentType> OnResultStageSet;

    // public event EventHandler<RenderCameraEventArgs> OnMapCameraSet;

  #endregion

  }

}
