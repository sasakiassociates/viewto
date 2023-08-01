using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using Speckle.ConnectorUnity;
using Speckle.ConnectorUnity.Converter;
using Speckle.ConnectorUnity.Models;
using Speckle.ConnectorUnity.Ops;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Speckle.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using ViewObjects;
using ViewObjects.Systems;
using ViewTo.Connector.Unity.Commands;
using VU = ViewObjects.Unity;
using VO = ViewObjects;
using VS = ViewObjects.Speckle;

namespace ViewTo.Connector.Unity
{


  public class ViewToHub_v1 : MonoBehaviour
  {

    [Header("Will Trash Soon")]
    public bool autoRun = true;
    public string tempStreamId = "81c40b04df";
    public string tempCommitId = "bab6b9a0a2";

    [Header("Setup")]
    [SerializeField] Material analysisMaterial;
    [SerializeField] Material renderedMat;
    [SerializeField] bool createCommit = true;
    [SerializeField] bool runRepeatCommand;

    [Header("Objects")]
    [SerializeField] VU.ViewStudy study;
    [SerializeField] Rig rig;
    [SerializeField, HideInInspector] ResultExplorer explorer;
    [SerializeField, HideInInspector] DebugExplorer debugExplorer;
    [SerializeField] SpeckleStreamObject streamObject;
    [SerializeField] ScriptableConverter converterUnity;

    // TODO: remove this at some point so the conversions are handled properly
    VS.ViewStudy _speckleStudy;
    RepeatResultTester _tester;


    public SpeckleStream Stream {get;set;}

    public bool CanRun
    {
      get => study != null && study.IsValid && rig != null && rig.IsReady;
    }

    public bool CanExplore
    {
      get => study != null && study.IsValid && study.CanExplore();
    }



    public void LoadStudy(string newStreamId, string newCommitId)
    {
      tempStreamId = newStreamId;
      tempCommitId = newCommitId;
      ReceiveStream().Forget();
    }

    [Button]
    public void LoadStudy() => ReceiveStream().Forget();

    [Button]
    public void ExploreAnalysis() => explorer.Load(study);

    [Button]
    public void RunAnalysis() => rig.Run();

    [Button]
    public void DebugStudy() => debugExplorer.Load(rig, study);

    void Init()
    {
      Instance = this;

      if(analysisMaterial == null)
        analysisMaterial = new Material(Resources.Load<Shader>("UnlitDoubleSided"));

      if(renderedMat == null)
        renderedMat = new Material(Shader.Find(@"Standard"));
    }

    SpeckleConnector GetConnector()
    {
      var connector = SpeckleConnector.instance;
      if(connector == null)
      {
        connector = new GameObject("Speckle Connector").AddComponent<SpeckleConnector>();
      }

      return connector;
    }

    void HandleDataReady(VU.ResultsForCloud args)
    {
      if(study == null) return;

      var rc = study.TrySetResults(args);
      if(rc != null)
      {
        SendResultsToStream(rc);
      }
    }

    void SetRigToStudy()
    {
      if(!IsInit) Init();

      ViewConsole.Log($"{name} is loading View Study {study.ViewName} ");

      if(this.rig != null) VU.ViewObject.SafeDestroy(this.rig.gameObject);

      this.rig = new GameObject().AddComponent<Rig>();

      // setup events
      this.rig.OnReady += RigReady;
      this.rig.OnComplete += RigComplete;
      this.rig.OnDataReadyForCloud += HandleDataReady;
      this.rig.OnStudyLoaded += OnStudyLoaded;
      this.rig.OnStageChange += OnRigStageChanged;
      this.rig.OnContentLoaded += OnViewContentLoaded;
      this.rig.OnActiveViewerSystem += OnActiveViewerSystem;

      foreach(var c in study.FindObjects<VU.Content>())
      {
        c.PrimeMeshData(analysisMaterial);
      }

      IRig r = this.rig;
      study.LoadStudyToRig(ref r);

      rig.Activate(autoRun);
      if(autoRun)
      {
        RunAnalysis();
      }
    }

    // TODO: move this into speckle object



    async UniTask ReceiveStream()
    {
      // TODO: Fix the scriptable object so it stores that date properly in the editor
      streamObject = ScriptableObject.CreateInstance<SpeckleStreamObject>();
      await streamObject.Initialize("https://speckle.xyz/streams/"+tempStreamId+"/commits/"+tempCommitId);

      // TODO: This should be completely handled by the receiver object and not controlled here
      var client = new SpeckleClient(streamObject.BaseAccount);
      client.token = this.GetCancellationTokenOnDestroy();

      var commit = await client.CommitGet(streamObject.Id, streamObject.Commit.id);

      if(commit == null) return;

      var @base = await SpeckleOps.Receive(client, streamObject.Id, commit.referencedObject);

      if(@base == null) return;

      // TODO: this should be data saved in the object and converted back after the study is complete 
      _speckleStudy = await @base.SearchForType<VS.ViewStudy>(true, client.token);

      // this should be a study 
      var settings = new ConverterSettings() {style = ConverterSettings.ConversionStyle.Sync};

      converterUnity.SetConverterSettings(settings);

      var hierarchy = await SpeckleOps.ConvertToScene(transform, @base, converterUnity, client.token);

      foreach(var item in hierarchy.GetObjects())
      {
        var vs = item.GetComponent<VU.ViewStudy>();
        if(vs == null) continue;

        study = vs;
      }

      if(study == null)
      {
        UnityEngine.Debug.Log("Did not load study correctly");
        return;
      }

      settings.style = ConverterSettings.ConversionStyle.Queue;
      converterUnity.SetConverterSettings(settings);

      var loader = gameObject.AddComponent<StudyLoader>();
      loader.onLoadComplete += SetRigToStudy;

      loader.Run(study, client, streamObject, converterUnity).Forget();
    }

    void SendResultsToStream(VU.ResultCloud mono)
    {
      UnityEngine.Debug.Log("Auto Send");

      var data = mono.Data
        .Select(x => new VS.ResultCloudData(x.values, x.info, x.count))
        .ToList();

      var resultCloud = new VS.ResultCloud()
      {
        ViewId = Guid.NewGuid().ToString(),
        Data = data, 
        Points = mono.Points
      };
      _speckleStudy.objects.Add(resultCloud);

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

      // ViewConsole.Log("Runtime-" + $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}");

      if(runRepeatCommand)
      {
        ViewConsole.Log("Running Second Pass");

        runRepeatCommand = false;

        SetRigToStudy();
        rig.Activate();
        return;
      }

      OnStudyComplete?.Invoke(study);
    }

  #region static methods

    public static ViewToHub_v1 Initialize()
    {
      var hub = FindObjectOfType<ViewToHub_v1>();

      if(hub == null)
        hub = new GameObject("ViewToHub").AddComponent<ViewToHub_v1>();

      hub.Init();

      return hub;
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

    public static ViewToHub_v1 Instance {get;set;}

    public static bool IsInit
    {
      get => Instance != null && AnalysisMat != null && RenderedMat != null;
    }

  #endregion

  #region unity methods

    void Awake()
    {
      if(!IsInit) Init();
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

    public event UnityAction<ViewContentType> OnRigStageChanged;

    public event UnityAction<StudyLoadedArgs> OnStudyLoaded;

    public event UnityAction<ViewContentLoadedArgs> OnViewContentLoaded;

    // public UnityEvent<Bounds> OnContentBoundsSet;

    //

    // public UnityEvent<ViewContentType> OnResultStageSet;


    // public event EventHandler<RenderCameraEventArgs> OnMapCameraSet;

  #endregion

  }

}
