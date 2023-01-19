using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using ViewObjects;
using ViewObjects.Clouds;
using ViewObjects.Contents;
using ViewObjects.Results;
using ViewObjects.Studies;
using VU = ViewObjects.Unity;

namespace ViewTo.Connector.Unity
{

  public class ResultExplorer : MonoBehaviour, IExplorer
  {

    [SerializeField] VU.ViewStudy study;
    [SerializeField] VU.ResultCloud result;


    public int activePoint { get; set; }
    public UnityAction onStudyLoaded;

    public void Load(IViewStudy viewObj)
    {

      Debug.Log($"Loading new study into {nameof(ResultExplorer)}-{name}");

      if(viewObj == default(object) || !viewObj.CanExplore())
      {
        Debug.LogWarning("Invalid view study object to explore");
        return;
      }

      if(viewObj is VU.ViewStudy vs) study = vs;

      result = viewObj.FindObject<VU.ResultCloud>();

      if(Cloud == null)
      {
        return;
      }

      Settings ??= new ExplorerSettings();

      Options = Cloud.Data.Where(x => x != null).Select(x => x.Option).Cast<ContentOption>().ToList();
      var opt = Options.FirstOrDefault();

      if(opt != null)
      {
        ActiveContent = new ContentInfo(opt);
      }
      else
      {
        Debug.Log("No active options found");
      }

      onStudyLoaded?.Invoke();
    }

    public IViewStudy Source
    {
      get
      {
        if(study == null)
        {
          Debug.Log($"No study found in {nameof(ResultExplorer)}-{name} ");
          return null;
        }

        return study;
      }
    }

    int p;

    void Update()
    {
      if(p == activePoint) return;
      p = activePoint;
      Debug.Log(activePoint);
    }

    public IResultCloud Cloud => result;

    public List<IResultCloudData> Data => Cloud?.Data ?? new List<IResultCloudData>();

    public ExplorerSettings Settings { get; set; }

    public ContentInfo ActiveContent { get; set; }

    /// <inheritdoc />
    public bool IsValid => study != null;

    public List<ContentOption> Options { get; internal set; }
  }

}
