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

namespace ViewTo.Connector.Unity
{

  public class ViewerSystem : PixelSystem
  {

    (int cloud, int design) _active;

    List<IResultCloudData> _bundleDataForCloud;

    ViewerSetupData _data;

    VO.ViewContentType _stage;


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
      get => stage == VO.ViewContentType.Proposed && hasValidProposedOptions;
    }

    bool hasValidProposedOptions
    {
      get => designs.Valid(_active.design);
    }

    public bool isGlobal { get; set; }

    public VO.ViewContentType stage
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
        if(stage == VO.ViewContentType.Proposed)
        {
          _active.design++;
        }

        stage = GetNextStage(stage);

        if(stage != VO.ViewContentType.Proposed) return true;

        if(stage == VO.ViewContentType.Proposed && hasValidProposedOptions)
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

      foreach(VO.ViewContentType v in Enum.GetValues(typeof(VO.ViewContentType)))
      {
        if(v == VO.ViewContentType.Proposed && !designs.Valid())
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

      // TODO: this should be set from bounds of the study 
      // TODO: converters should report an overall bounding box of completed object
      Layouts.ForEach(x => x.Finders.ForEach(finder => finder.MaxClipping = 30000));
      // Note: important to do this here!
      stage = VO.ViewContentType.Potential;
    }

    VO.ViewContentType GetNextStage(VO.ViewContentType s)
    {
      return s switch
      {
        VO.ViewContentType.Potential => VO.ViewContentType.Existing,
        VO.ViewContentType.Existing => VO.ViewContentType.Proposed,
        _ => VO.ViewContentType.Proposed
      };
    }

    protected override void ResetSystem()
    {
      _active.cloud = 0;
      _active.design = 0;
      _stage = VO.ViewContentType.Potential;
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

        stage = VO.ViewContentType.Potential;

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
      // TODO: get current content info related to this 


      for(var layoutIndex = 0; layoutIndex < container.Data.Length; layoutIndex++)
      {
        var layout = container.Data[layoutIndex];
        var layoutName = container.ItemName[layoutIndex];

        // each view color is associated with the second array (double[pointIndex][colorIndex])
        for(var colorIndex = 0; colorIndex < _data.Colors.Count; colorIndex++)
        {
          // go through each finder and compile each point for that color
          var valuesFromTarget = new int[CollectionSize];
          var vc = _data.Colors[colorIndex];

          var raw1d = layout.Data.Get1d(colorIndex);

          for(var pIndex = 0; pIndex < raw1d.Length; pIndex++)
          {
            valuesFromTarget[pIndex] += raw1d[pIndex];
          }


          for(var index = 0; index < valuesFromTarget.Length; index++)
          {
            var v = valuesFromTarget[index];
            if(v >= int.MaxValue)
            {
              Debug.LogWarning($"({index}) is too big for int: {v}");
            }
          }


          /*
           * If in Potential we set the content info to be the same as target
           * If in Existing we set the content info to be the same as target (we could use the content info object but there are multiple of them so that might be an issue)
           * If in Proposed we set the content info to be proposed object info 
           */

          var targetInfo = new ContentInfo(vc.id, vc.name);
          var contentInfo = _stage switch
          {

            VO.ViewContentType.Potential => new ContentInfo(vc.id, vc.name + "-" + nameof(VO.ViewContentType.Potential)),
            VO.ViewContentType.Existing => new ContentInfo(vc.id, vc.name + "-" + nameof(VO.ViewContentType.Existing)),
            VO.ViewContentType.Proposed => new ContentInfo(_data.ProposedContent[_active.design]),
            _ => throw new ArgumentOutOfRangeException()
          };

          _bundleDataForCloud.Add(new ResultCloudData(valuesFromTarget.ToList(), new ContentOption(targetInfo, contentInfo, _stage), layout.FinderNames.Length));
        }
      }

      ViewConsole.Log($"{name} is done gathering data for "
                      + $"{(checkIfDesignStage ? stage + $" {_data.ProposedContent[_active.design].name}" : stage)}");

      return container;
    }

    public event UnityAction<VO.ViewContentType> OnStageChange;

    public event UnityAction<ResultsForCloud> OnDataReadyForCloud;
  }

}
