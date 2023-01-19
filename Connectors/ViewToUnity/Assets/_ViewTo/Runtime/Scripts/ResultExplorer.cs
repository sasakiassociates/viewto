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


    public int index
    {
      get => _index;
      set
      {
        _index = value;
        point = Cloud.Points[value].ToUnity();
        onPointSet?.Invoke();
      }
    }
    public Vector3 point { get; private set; }

    public List<ContentOption> Options { get; internal set; }

    public bool isRigged
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
            _rig.Run(index, false);
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

    public ResultPoint GetResultPoint() => new ResultPoint
    {
      Option = ActiveContent,
      Value = _values[index],
      Index = index,
      X = Cloud.Points[index].x,
      Y = Cloud.Points[index].y,
      Z = Cloud.Points[index].z
    };

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
        var pt = Cloud.Points[i];

        colors.Add(Settings.GetColor(_values[i]).ToUnity());
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

      if(index >= points.Count) index = 0;
    }
  }

}
