#region

using Sasaki.Unity;
using Speckle.ConnectorUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using ViewObjects.Clouds;
using ViewObjects.Contents;
using ViewObjects.Results;
using ViewObjects.Unity;
using VO = ViewObjects;

#endregion

namespace ViewTo.Connector.Unity
{

  public class ViewerSystem : PixelSystem
  {
    [SerializeField] bool _isGlobal;

    (int cloud, int design) _active;

    List<IResultCloudData> _bundleDataForCloud;

    ViewerSetupData _data;

    VO.ContentType _stage;

    List<ViewObjects.Unity.Content> designs
    {
      get => _data.ProposedContent;
    }

    ViewObjects.Unity.ViewCloud activeCloud
    {
      get => clouds[_active.cloud];
    }

    List<ViewObjects.Unity.ViewCloud> clouds
    {
      get => _data.Clouds;
    }

    bool checkIfDesignStage
    {
      get => stage == VO.ContentType.Proposed && hasValidProposedOptions;
    }

    bool hasValidProposedOptions
    {
      get => designs.Valid(_active.design);
    }

    public bool isGlobal
    {
      get => _isGlobal;
      set => _isGlobal = value;
    }

    public VO.ContentType stage
    {
      get => _stage;
      set
      {
        if(!Layouts.Valid())
          return;

        _stage = value;
        Mask = value.GetCullingMask();
        OnStageChange?.Invoke(value);
      }
    }

    bool hasMoreStagesToDo
    {
      get
      {
        if(stage == VO.ContentType.Potential)
          _active.design++;

        stage = GetNextStage(stage);

        if(stage != VO.ContentType.Proposed)
          return true;

        if(stage == VO.ContentType.Proposed && hasValidProposedOptions)
        {
          foreach(var d in designs)
            d.Show = false;

          designs[_active.design].Show = true;

          return true;
        }

        return false;
      }
    }

    public override Dictionary<string, int> GetMaskLayers()
    {
      var values = new Dictionary<string, int>();

      foreach(VO.ContentType v in Enum.GetValues(typeof(VO.ContentType)))
      {
        if(v == VO.ContentType.Proposed && !designs.Valid())
          continue;

        values.Add(v.ToString(), v.GetCullingMask());
      }

      return values;
    }

    public void Init(ViewerSetupData data)
    {
      if(!data.Layouts.Valid() || !data.Colors.Valid() || !data.Clouds.Valid())
      {
        Debug.Log($"Invalid Layouts for {name}");
        return;
      }

      _data = data;

      // TODO: Fix the data ILayout type being passed in 
      var converted = new List<PixelLayout>();
      foreach(var layout in data.Layouts)
        switch(layout.Viewers.Count)
        {
          case 6:
            converted.Add(new GameObject().AddComponent<PixelLayoutCube>());
            break;
          case 4:
            converted.Add(new GameObject().AddComponent<PixelLayoutHorizontal>());
            break;
          case 1:
            converted.Add(new GameObject().AddComponent<PixelLayout>());
            break;
          // case LayoutCube:
          // 	converted.Add(new GameObject().AddComponent<PixelLayoutCube>());
          // 	break;
          // case LayoutHorizontal:
          // 	converted.Add(new GameObject().AddComponent<PixelLayoutHorizontal>());
          // 	break;
          // case LayoutOrtho o:
          // 	var res = new GameObject().AddComponent<PixelLayoutOrtho>();
          // 	res.orthoSize = (float)o.Size;
          // 	converted.Add(res);
          // 	break;
          // case LayoutFocus o:
          // 	Debug.LogWarning($"{o} is not supported yet");
          // 	// converted.Add(new GameObject().AddComponent<>());
          // 	break;
          // case LayoutNormal o:
          // 	// TODO: handle relating the normal cloud type
          // 	Debug.LogWarning($"{o} is not supported yet");
          // 	// converted.Add(new GameObject().AddComponent<>());
          // 	break;
        }

      // OnDataReadyForCloud = onDataForCloud;
      _active.cloud = clouds.Count - 1;
      var systemPoints = clouds[_active.cloud].GetPointsAsVectors();

      Init(systemPoints, data.Colors.ToUnity().ToArray(), converted);

      // Note: important to do this here!
      stage = VO.ContentType.Potential;
    }

    VO.ContentType GetNextStage(VO.ContentType s)
    {
      return s switch
      {
        VO.ContentType.Potential => VO.ContentType.Existing,
        VO.ContentType.Existing => VO.ContentType.Proposed,
        _ => VO.ContentType.Proposed
      };
    }

    protected override void ResetSystem()
    {
      _active.cloud = 0;
      _active.design = 0;
      _stage = VO.ContentType.Potential;
      base.ResetSystem();
    }

    protected override bool ShouldSystemRunAgain()
    {
      // if another stage is available, we store that data with the stage name and reset to 0
      if(hasMoreStagesToDo)
      {
        ResetDataContainer();

        ViewConsole.Log($"{name} is starting next stage "
                        + (checkIfDesignStage ? stage + $" {_data.ProposedContent[_active.design].name}" : stage));
        return true;
      }

      // if we reached the last stage for a cloud, we send that data to morph the view cloud into a result cloud
      OnDataReadyForCloud?.Invoke(new ResultsForCloud(activeCloud.ViewId, _bundleDataForCloud));

      // remove the previous index
      clouds.RemoveAt(_active.cloud);

      // step forward to see if have more clouds
      _active.cloud--;

      // if we are done with our clouds 
      if(_active.cloud >= 0)
      {
        ResetDataContainer();

        // there are more clouds to use, so we store the points and set the run back to 0
        ViewConsole.Log("Loading new cloud");

        // store points
        Points = clouds[_active.cloud].GetPointsAsVectors();

        stage = VO.ContentType.Potential;

        // reset all views  
        foreach(var d in designs)
          d.Show = false;

        return true;
      }

      // if no more clouds are available we finish the call and send off the completed system data
      ViewConsole.Log("All clouds complete, gathering data");
      return false;
    }

    protected override IPixelSystemDataContainer GatherSystemData()
    {
      _bundleDataForCloud ??= new List<IResultCloudData>();

      // gather all data
      var container = new PixelSystemData(this);

      for(var layoutIndex = 0; layoutIndex < container.data.Length; layoutIndex++)
      {
        var layout = container.data[layoutIndex];
        var layoutName = container.layoutNames[layoutIndex];

        // each view color is associated with the second array (double[pointIndex][colorIndex])
        for(var colorIndex = 0; colorIndex < _data.Colors.Count; colorIndex++)
        {
          // go through each finder and compile each point for that color
          var layoutValues = new int[CollectionSize];
          var vc = _data.Colors[colorIndex];

          var raw1d = layout.data.Get1d(colorIndex);

          for(var pIndex = 0; pIndex < raw1d.Length; pIndex++)
          {
            layoutValues[pIndex] += raw1d[pIndex];
          }

          _bundleDataForCloud.Add(
            new ResultCloudData()
            {
              Values = layoutValues.ToList(),
              Layout = layoutName,
              Option = new ContentOption()
              {
                Id = vc.id, Name = vc.name, Stage = stage
              }
            }
          );
        }
      }

      ViewConsole.Log($"{name} is done gathering data for "
                      + $"{(checkIfDesignStage ? stage + $" {_data.ProposedContent[_active.design].name}" : stage)}");

      return container;
    }

    public event UnityAction<VO.ContentType> OnStageChange;

    public event UnityAction<ResultsForCloud> OnDataReadyForCloud;
  }

}
