using Sasaki.Unity;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace ViewTo.Connector.Unity
{

  public class DebugExplorerUi : MonoUiDoc<DebugExplorer>
  {

    Button _captureBut;
    SliderInt _pointSlider;

    protected override void BuildFromRoot(VisualElement root)
    {
      _pointSlider = root.Q<SliderInt>();
      _captureBut = root.Q<Button>();
    }

    protected override void HookSource()
    {
      source.OnCapture += ProcessCapture;
      source.onPointSizeSet += size =>
      {
        _pointSlider.lowValue = 0;
        _pointSlider.highValue = size - 1;
        _pointSlider.SetValueWithoutNotify(0);
      };

      _pointSlider.RegisterValueChangedCallback(e => source.Index = e.newValue);
      _captureBut.clickable.clicked += HandleCapture;

    }

    void ProcessCapture(SystemCaptureArgs capture)
    {
      Debug.Log($"Processing capture for {capture.name}");

      foreach(var arg in capture.args)
      {
        Debug.Log($"{arg.name}-Finder Args{arg.finderArgs.Count}");
        foreach(var finderArgs in arg.finderArgs)
        {
          Debug.Log($"Finder-{finderArgs.name} Point-{finderArgs.point} Value-{finderArgs.values.FirstOrDefault()}");
        }
      }

    }

    void HandleCapture() => source.Capture();
  }

}
