using UnityEngine;
using UnityEngine.UIElements;

namespace ViewTo.Connector.Unity
{

  public abstract class MonoUiDoc<TMono> : MonoBehaviour where TMono : MonoBehaviour
  {
    [SerializeField] protected TMono source;
    [SerializeField] protected UIDocument uiDoc;


    void Awake()
    {
      if(source == null)
      {
        source = GetComponent<TMono>();
      }
      if(uiDoc == null)
      {
        Debug.Log($"No UI Doc is found for {name}({nameof(TMono)}.\nSet a UI Doc in order to use the explorer ui");
        return;
      }

      BuildFromRoot(uiDoc.rootVisualElement);
    }

    void Start()
    {
      if(source == null)
      {
        Debug.Log($"No source of {nameof(TMono)} was found. Please make sure to add this component so the ui can connect with it");
        return;
      }

      HookSource();
    }

    protected abstract void HookSource();

    protected abstract void BuildFromRoot(VisualElement root);
  }

}
