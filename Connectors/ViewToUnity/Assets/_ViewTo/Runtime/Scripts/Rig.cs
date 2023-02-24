#region

using Sasaki.Unity;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using ViewObjects.Common;
using ViewObjects.Systems;
using ViewTo.Connector.Unity.Commands;
using VU = ViewObjects.Unity;
using VO = ViewObjects;

#endregion

namespace ViewTo.Connector.Unity
{

  public interface IRigSystem
  {
    public ViewerSystem ActiveViewer { get; }

    public bool IsReady { get; }
  }

  public class Rig : MonoBehaviour, IRig, IRigSystem
  {

    Stopwatch _timer;

    [field: SerializeField] public List<RigParameters> RigParams
    {
      get;
      protected set;
    }

    [field: SerializeField] public VO.ViewContentType Stage
    {
      get;
      private set;
    }

    /// <inheritdoc />
    public void Initialize(List<RigParameters> parameters)
    {
      name = "Rig";
      RigParams = parameters;
    }

    /// <inheritdoc />
    public void Build()
    {
      if(TryCreateNewViewer()) OnReady?.Invoke();
    }

    public ViewerSystem ActiveViewer { get; protected set; }

    public bool IsReady => Application.isPlaying && ActiveViewer != null;

    bool TryCreateNewViewer()
    {
      if(!RigParams.Valid())
      {
        return false;
      }

      if(ActiveViewer == null)
      {
        ActiveViewer = new GameObject().AddComponent<ViewerSystem>();
        ActiveViewer.transform.SetParent(transform);
      }

      ActiveViewer.Init(new ViewerSetupData(RigParams[0]));
      ActiveViewerEvents(true);

      ViewConsole.Log($"Active Viewer: {ActiveViewer.name}");
      OnActiveViewerSystem?.Invoke(ActiveViewer);

      if(RigParams.Count <= 1)
      {
        RigParams.Clear();
      }
      else
      {
        var rigParams = new RigParameters[RigParams.Count - 1];
        RigParams.CopyTo(rigParams, 1);
        RigParams = rigParams.ToList();
      }

      return true;
    }

    public void Activate(bool autoRun = true)
    {
      if(!IsReady)
      {
        ViewConsole.Log("Not Ready");
        return;
      }



      if(!autoRun) return;

      _timer ??= new Stopwatch();
      _timer.Start();
      ActiveViewer.Run();
    }

    public void MoveTo(int index)
    {
      if(ActiveViewer != null) ActiveViewer.MoveToPoint(index);
    }

    public void ManualCapture(int index)
    {
      if(ActiveViewer != null) ActiveViewer.Capture(index);
    }

    void HandleViewerComplete()
    {
      _timer.Stop();
      ViewConsole.Log($"Total Time for {ActiveViewer.name}-{_timer.Elapsed}");

      ActiveViewerEvents(false);

      if(TryCreateNewViewer())
      {
        Activate();
        return;
      }

      ViewConsole.Log("Rig Complete");
      OnComplete?.Invoke();
    }

    void SetStageChange(VO.ViewContentType arg)
    {
      Stage = arg;
      OnStageChange?.Invoke(Stage);
    }

    void ActiveViewerEvents(bool listen)
    {
      if(ActiveViewer == null) return;

      if(listen)
      {
        ActiveViewer.OnStageChange += SetStageChange;
        ActiveViewer.OnComplete += HandleViewerComplete;
        ActiveViewer.OnDataReadyForCloud += OnDataReadyForCloud;
        ActiveViewer.OnCapture += CheckCapture;
      }
      else
      {
        ActiveViewer.OnStageChange -= SetStageChange;
        ActiveViewer.OnComplete -= HandleViewerComplete;
        ActiveViewer.OnDataReadyForCloud -= OnDataReadyForCloud;
        ActiveViewer.OnCapture -= CheckCapture;

      }
    }


    void CheckCapture(SystemCaptureArgs args)
    {
      UnityEngine.Debug.Log("On Capture Called from Rig");
      OnCapture?.Invoke(args);
    }

  #region events

    public event UnityAction OnReady;

    public event UnityAction OnComplete;

    public event UnityAction<SystemCaptureArgs> OnCapture;

    public event UnityAction<ViewContentLoadedArgs> OnContentLoaded;

    public event UnityAction<VO.ViewContentType> OnStageChange;

    public event UnityAction<StudyLoadedArgs> OnStudyLoaded;

    public event UnityAction<VU.ResultsForCloud> OnDataReadyForCloud;

    public event UnityAction<ViewerSystem> OnActiveViewerSystem;

  #endregion

  }

}
