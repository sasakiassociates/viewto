using UnityEngine;
using UnityEngine.UIElements;
namespace ViewTo.Connector.Unity
{
  public class ViewToHubUi : MonoBehaviour
  {

    [field: SerializeField] public UIDocument ui { get; private set; }
    [SerializeField] VisualTreeAsset accountElement;
    
  }
}
