using Speckle.ConnectorUnity.Elements;
using Speckle.Core.Credentials;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace ViewTo.Connector.Unity
{
  public class ViewToHubUi : MonoBehaviour
  {

    [field: SerializeField] public UIDocument ui { get; private set; }

    [SerializeField] VisualTreeAsset accountVisual;

    List<Account> _accounts;
    ListView _accountList;
    ViewToHub _hub;


    void Start()
    {

      _hub = ViewToHub.Instance;
      if (_hub == null)
      {
        Debug.Log($"{this} needs an instance of {nameof(ViewToHub)}");
        return;
      }

      if (ui == null)
      {
        Debug.Log($"{this} is missing a {typeof(UIDocument)}");
        return;
      }

      if (accountVisual == null)
      {
        Debug.Log($"{this} is missing a {typeof(VisualTreeAsset)} for accounts");
        return;
      }

      _accounts = _hub.GetSpeckleAccounts().ToList();

      SetUpAccountList();

    }

    void SetUpAccountList()
    {
      _accountList = ui.rootVisualElement.Q<ListView>("accounts");

      _accountList.makeItem = () =>
      {
        var entry = accountVisual.Instantiate();
        entry
      };

      _accountList.bindItem = (item, index) =>
      {
        if (item.userData is AccountElement ae)
        {
          var a = _accounts[index];
          ae.SetUserInfo(a.userInfo);
          ae.SetServerInfo(a.serverInfo);
        }
      };
      // Set a fixed item height
      _accountList.fixedItemHeight = 45;
      _accountList.selectionType = SelectionType.Single;
      _accountList.reorderable = false;
      _accountList.itemsSource = _accounts;

      void onAccountSelected(IEnumerable<object> selectedItems)
      {
        _hub.account = _accounts[_accountList.selectedIndex];
      }

      _accountList.onSelectionChange += onAccountSelected;
    }

  }

}
