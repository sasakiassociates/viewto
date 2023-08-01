using Sasaki.Unity;
using UnityEngine;
using UnityEngine.Events;
using VU = ViewObjects.Unity;

namespace ViewTo.Connector.Unity
{

  public class DebugExplorer : MonoBehaviour
  {
    int _index;
    Rig _rig;

    VU.ViewStudy _study;
    public UnityAction onLoad;

    public UnityAction onPointSet;

    public UnityAction<int> onPointSizeSet;


    public int Index
    {
      get => _index;
      set
      {
        _index = value;
        _rig.MoveTo(value);
        // _rig.ManualCapture(value);
        onPointSet?.Invoke();
      }
    }

    public event UnityAction<SystemCaptureArgs> OnCapture;


    public void Capture()
    {
      Debug.Log("Capturing");
      _rig.ManualCapture(_index);
    }

    public void Load(Rig r, VU.ViewStudy study)
    {
      _rig = r;
      _rig.OnCapture += ProcessCapture;

      _study = study;

      onLoad?.Invoke();
      onPointSizeSet?.Invoke(_rig.ActiveViewer.CollectionSize);

    }

    void ProcessCapture(SystemCaptureArgs args)
    {
      Debug.Log("Processing Capture");
      OnCapture?.Invoke(args);
    }
  }

}
