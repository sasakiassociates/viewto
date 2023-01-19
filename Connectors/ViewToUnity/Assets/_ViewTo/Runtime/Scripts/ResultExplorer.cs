using NaughtyAttributes;
using Pcx;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using ViewObjects;
using ViewObjects.Clouds;
using ViewObjects.Common;
using ViewObjects.Contents;
using ViewObjects.Results;
using ViewObjects.Studies;
using ViewObjects.Unity;
using VU = ViewObjects.Unity;

namespace ViewTo.Connector.Unity
{

  public class ResultExplorer : MonoBehaviour, IExplorer
  {

    [SerializeField] VU.ViewStudy study;
    [SerializeField] VU.ResultCloud result;
    [SerializeField] ExplorerValueType valueType = ExplorerValueType.ExistingOverPotential;
    [SerializeField] Gradient gradient;
    int _activePoint;



    PointCloudData _pcxData;
    PointCloudRenderer _renderer;
    double[] _values;

    public int activePoint
    {
      get => _activePoint;
      set
      {
        _activePoint = value;
        SetResultPoint();
      }
    }

    public UnityAction onStudyLoaded;

    public UnityAction<ResultPoint> onResultPointSet;

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
      Settings.colorRamp = gradient.GetColors();

      Options = Cloud.Data.Where(x => x != null).Select(x => x.Option).Cast<ContentOption>().ToList();
      var opt = Options.FirstOrDefault();

      if(opt != null)
      {
        ActiveContent = new ContentInfo(opt);
        ApplyNewValues();
      }
      else
      {
        Debug.Log("No active options found");
      }

      onStudyLoaded?.Invoke();

    }

    public void Visualize(string targetName)
    {

      if(!Options.Valid())
      {
        Debug.Log("No content options are found in this explorer");
        return;
      }

      foreach(var opt in Options)
      {
        if(opt.Name.Valid() && opt.Name.ToUpper().Equals(targetName.ToUpper()))
        {
          ActiveContent = new ContentInfo(opt);
          ApplyNewValues();
          break;
        }
      }
    }


    void Visualize(ContentOption content)
    { }


    void ApplyNewValues()
    {

      if(!this.TryGetValues(valueType, out var values))
      {
        Debug.Log("Values did not return properly");
        return;
      }

      _values = values as double[] ?? values.ToArray();


      // TODO: this should be swapped out at some point 
      var colors = new List<Color32>();
      var points = new List<Vector3>();

      for(int i = 0; i < Cloud.GetCount(); i++)
      {
        var point = Cloud.Points[i];

        colors.Add(Settings.GetColor(_values[i]).ToUnity());
        points.Add(point.ToUnity());
      }


    #if UNITY_EDITOR
          _pcxData = ScriptableObject.CreateInstance<PointCloudData>();
          _pcxData.Initialize(points, colors);
    #endif

      if(_renderer == null)
      {
        _renderer = gameObject.GetComponent<PointCloudRenderer>();

        if(_renderer == null) _renderer = gameObject.AddComponent<PointCloudRenderer>();
      }

      _renderer.sourceData = _pcxData;

      if(activePoint >= points.Count) activePoint = 0;
      SetResultPoint();
    }

    void SetResultPoint() => onResultPointSet?.Invoke(
      new ResultPoint
      {
        Option = ActiveContent,
        Value = _values[activePoint],
        Index = activePoint,
        X = Cloud.Points[activePoint].x,
        Y = Cloud.Points[activePoint].y,
        Z = Cloud.Points[activePoint].z
      }
    );

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

    public IResultCloud Cloud => result;

    public List<IResultCloudData> Data => Cloud?.Data ?? new List<IResultCloudData>();

    public ExplorerSettings Settings { get; set; }

    public ContentInfo ActiveContent { get; set; }

    /// <inheritdoc />
    public bool IsValid => study != null;

    public List<ContentOption> Options { get; internal set; }
  }

}
