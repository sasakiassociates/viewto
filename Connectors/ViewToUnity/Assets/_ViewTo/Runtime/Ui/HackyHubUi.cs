using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ViewTo.Connector.Unity;

namespace ViewTo
{

  public class HackyHubUi : MonoBehaviour
  {

    [SerializeField] ViewToHub_v1 parent;

    [SerializeField] TMP_InputField streamId;
    [SerializeField] TMP_InputField commitId;
    [SerializeField] Button loadButton;

    [SerializeField] Button runButton;
    [SerializeField] Slider frameRateBar;
    [SerializeField] Slider progressBar;
    [SerializeField] TextMeshProUGUI stateText;


    void Start()
    {
      if(parent == null)
      {
        Debug.LogWarning($"No Hub attached for {nameof(HackyHubUi)}");
        return;
      }

      streamId.text = parent.tempStreamId;
      commitId.text = parent.tempCommitId;

      runButton.interactable = parent.CanRun;

      parent.OnRigBuilt += HandleNewRigAction;
      parent.OnRigReady += HandleRigReadyAction;
      parent.OnActiveViewerSystem += HandleNewViewerAction;
      frameRateBar.onValueChanged.AddListener(arg => Application.targetFrameRate = (int)arg);
      loadButton.onClick.AddListener(HandleLoadAction);
      runButton.onClick.AddListener(HandleRunAction);
    }

    void HandleRigReadyAction()
    {
      Debug.Log("Rig Ready Action");
      runButton.interactable = parent.CanRun;
    }

    void HandleNewViewerAction(ViewerSystem arg)
    {
      Debug.Log("New Viewer Action");
      Debug.Log($"Can Parent Run? {parent.CanRun}");
      Debug.Log($"Is Button Interactable?? {runButton.interactable}");

      runButton.interactable = parent.CanRun;

      stateText.text = arg.stage.ToString();
    }


    void HandleNewRigAction(IRigSystem arg)
    {
      Debug.Log("New Rig Action");

      progressBar.interactable = false;
      progressBar.wholeNumbers = true;
      progressBar.maxValue = arg.ActiveViewer.Points.Length;
      progressBar.minValue = 0;

      arg.ActiveViewer.OnStageChange += stage => stateText.text = stage.ToString();
      arg.ActiveViewer.OnPointComplete += index => progressBar.value = index;
    }

    void HandleRunAction()
    {
      Debug.Log("On run call");
      parent.RunAnalysis();
    }

    void HandleLoadAction()
    {
      Debug.Log("On Load call");
      parent.LoadStudy(streamId.text, commitId.text);
    }
  }

}
