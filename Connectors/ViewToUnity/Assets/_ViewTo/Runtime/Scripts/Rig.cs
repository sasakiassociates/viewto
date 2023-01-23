#region

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

    [field: SerializeField] public VO.ContentType Stage
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
      }

      ActiveViewer.Init(new ViewerSetupData(RigParams[0]));

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

    public void Activate(int startPoint = 0, bool autoRun = true)
    {
      if(!IsReady)
      {
        ViewConsole.Log("Not Ready");
        return;
      }

      ActiveViewer.OnStageChange += SetStageChange;
      ActiveViewer.OnComplete += CompileViewerSystem;
      ActiveViewer.OnDataReadyForCloud += ResultDataCompleted;

      if(autoRun)
      {
        _timer ??= new Stopwatch();
        _timer.Start();
        ActiveViewer.Run();
      }
      else
      {
        ActiveViewer.Capture(startPoint);
      }
    }

    public void TrySetPoint(int index)
    {
      if(ActiveViewer != null)
        ActiveViewer.Capture(index);
    }

    void CompileViewerSystem()
    {
      _timer.Stop();
      ViewConsole.Log($"Total Time for {ActiveViewer.name}-{_timer.Elapsed}");

      ActiveViewer.OnStageChange -= SetStageChange;
      ActiveViewer.OnComplete -= CompileViewerSystem;
      ActiveViewer.OnDataReadyForCloud -= ResultDataCompleted;

      if(TryCreateNewViewer())
      {
        Activate();
        return;
      }

      ViewConsole.Log("Rig Complete");
      OnComplete?.Invoke();
    }

    void SetStageChange(VO.ContentType arg)
    {
      Stage = arg;
      OnStageChange?.Invoke(Stage);
    }

    void ResultDataCompleted(VU.ResultsForCloud data)
    {
      // let any other subscriptions know of the data being completed 
      OnDataReadyForCloud?.Invoke(data);
    }

  #region events

    public event UnityAction OnReady;

    public event UnityAction OnComplete;

    public event UnityAction<ViewContentLoadedArgs> OnContentLoaded;

    public event UnityAction<VO.ContentType> OnStageChange;

    public event UnityAction<StudyLoadedArgs> OnStudyLoaded;

    public event UnityAction<VU.ResultsForCloud> OnDataReadyForCloud;

    public event UnityAction<ViewerSystem> OnActiveViewerSystem;

  #endregion

  }

}
