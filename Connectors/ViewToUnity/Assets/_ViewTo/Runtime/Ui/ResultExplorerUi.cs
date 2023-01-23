using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using ViewObjects.Results;
using ViewObjects.Unity;

namespace ViewTo.Connector.Unity
{

  public class ResultExplorerUi : MonoUiDoc<ResultExplorer>
  {
    Button _captureBut;


    DropdownField _contentOptions;
    SliderInt _pointSlider;
    VisualElement _resultPointContainer;
    Label _targetName, _targetValue;
    Toggle _toggleRig;


    protected override void HookSource()
    {
      source.onStudyLoaded += HandleNewStudy;
      source.onPointSet += () => HandleNewResultPoint(source.GetResultPoint());
      _pointSlider.RegisterValueChangedCallback(e => source.Index = e.newValue);
    }

    protected override void BuildFromRoot(VisualElement root)
    {
      // hacky way of setting the ui doc shit 
      _contentOptions = root.Q<DropdownField>();
      _pointSlider = root.Q<SliderInt>();
      _captureBut = root.Q<Button>();
      _resultPointContainer = root.Q<VisualElement>("result-samples-container");
      _toggleRig = root.Q<Toggle>();
      _targetName = _resultPointContainer.Q<Label>("target-name");
      _targetValue = _resultPointContainer.Q<Label>("target-value");

      _toggleRig.RegisterValueChangedCallback(e => source.IsRigged = e.newValue);
    }

    void HandleNewResultPoint(ResultPoint arg)
    {
      _targetName.text = arg.Option.ViewName;
      _targetValue.text = arg.Value.ToString();
    }

    void HandleNewStudy()
    {
      Debug.Log("New Study Loaded");

      if(_contentOptions != null)
      {
        _contentOptions.choices = source.Options.Select(x => x == null ? "null" : x.Name).ToList();
        _contentOptions.index = 0;
      }

      if(_pointSlider != null)
      {
        _pointSlider.lowValue = 0;
        _pointSlider.highValue = source.Cloud.GetCount();
        _pointSlider.value = 0;
      }

    }
  }

}
