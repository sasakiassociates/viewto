using Pcx;
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
using ViewObjects.Systems;
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

    int _index;
    PointCloudData _pcxData;
    PointCloudRenderer _renderer;
    Rig _rig;
    double[] _values;

    public UnityAction onPointSet;

    public UnityAction onStudyLoaded;

    public int Index
    {
      get => _index;
      set
      {
        _index = value;
        Point = cloud.Points[value].ToUnity();
        onPointSet?.Invoke();
      }
    }

    public Vector3 Point { get; private set; }


    public bool IsRigged
    {
      get => _rig != null;
      set
      {
        switch(value)
        {
          case false when _rig != null:
            Destroy(_rig.gameObject);
            break;
          case true when _rig == null:
            _rig = new GameObject("Rig").AddComponent<Rig>();
            IRig iRig = _rig;
            Debug.Log("Loading study to new rig");
            var args = study.LoadStudyToRig(ref iRig);

            if(args.Valid()) args.ForEach(Debug.Log);

            Debug.Log("Study Loaded to rig - Setting Rig to point");
            _rig.Activate(false);
            break;
        }

      }
    }

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

      if(cloud == null)
      {
        return;
      }
      
      Load(result);


    }
    
    
    public void Load(IResultCloud obj)
    {
      settings ??= new ExplorerSettings();
      settings.colorRamp = gradient.GetColors();
      meta = new ExplorerMetaData(obj);
      ApplyNewValues();
      onStudyLoaded?.Invoke();
    }

    public List<IResultCloudData> data => cloud?.Data ?? new List<IResultCloudData>();
    public IResultCloud cloud { get; internal set; }
    public ExplorerMetaData meta { get; internal set; }
    public ExplorerSettings settings { get; set; }


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

      for(int i = 0; i < cloud.GetCount(); i++)
      {
        var pt = cloud.Points[i];

        colors.Add(settings.GetColor(_values[i]).ToUnity());
        points.Add(pt.ToUnity());
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

      if(Index >= points.Count) Index = 0;
    }

  }

}
