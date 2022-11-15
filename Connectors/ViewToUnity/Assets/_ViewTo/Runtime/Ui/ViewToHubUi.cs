using Cysharp.Threading.Tasks;
using Speckle.ConnectorUnity;
using Speckle.ConnectorUnity.Elements;
using Speckle.ConnectorUnity.Ops;
using Speckle.Core.Credentials;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using ViewObjects.Unity;
namespace ViewTo.Connector.Unity
{
  public class ViewToHubUi : MonoBehaviour
  {

    [field: SerializeField] public UIDocument ui { get; private set; }

    [SerializeField] VisualTreeAsset accountVisual;

    List<Account> _accounts;
    ListView _accountList;
    ViewToHub _hub;
    SpeckleConnector _connector;
    AccountElement _account;

    DropdownField _streams, _studies;
    ListView _details;

    void Start()
    {

      if (ui == null)
      {
        Debug.Log($"{this} is missing a {typeof(UIDocument)}");
        return;
      }

      _account = ui.rootVisualElement.Q<AccountElement>();
      _streams = ui.rootVisualElement.Q<DropdownField>("streams");
      _studies = ui.rootVisualElement.Q<DropdownField>("studies");
      _details = ui.rootVisualElement.Q<ListView>();


      _hub = ViewToHub.Instance;
      _connector = SpeckleConnector.Instance;

      if (_hub == null)
      {
        Debug.Log($"{this} needs an instance of {nameof(ViewToHub)}");
        return;
      }

      if (_connector == null)
      {
        Debug.Log($"{this} needs an instance of {nameof(SpeckleConnector)}");
        return;
      }

      _hub.OnStudiesFound += SetStudies;
      _connector.OnAccountSet += SetAccount;
      _connector.OnStreamsLoaded += SetStreams;


      SetAccount();
    }

    void SetStudies(List<ViewStudy> args)
    {
    }


    void SetStreams(List<SpeckleStream> args)
    {
      // _streams.Clear();
      var values = new List<string>();

      foreach (var stream in args)
      {
        var v = stream.Name + " | " + stream.Id;
        Debug.Log(v);
        values.Add(v);
      }

      _streams.choices = values;
      _streams.index = 0;

    }

    void SetAccount()
    {

      _account.SetAccount(_hub.account);
    }


    // void SetUpAccountList()
    // {
    //   _accountList = ui.rootVisualElement.Q<ListView>("accounts");
    //
    //   // _accountList.makeItem = () =>
    //   // {
    //   //   var entry = accountVisual.Instantiate();
    //   //   // entry
    //   // };
    //
    //   _accountList.bindItem = (item, index) =>
    //   {
    //     if (item.userData is AccountElement ae)
    //     {
    //       var a = _accounts[index];
    //       ae.SetUserInfo(a.userInfo);
    //       ae.SetServerInfo(a.serverInfo);
    //     }
    //   };
    //   // Set a fixed item height
    //   _accountList.fixedItemHeight = 45;
    //   _accountList.selectionType = SelectionType.Single;
    //   _accountList.reorderable = false;
    //   _accountList.itemsSource = _accounts;
    //
    //   void onAccountSelected(IEnumerable<object> selectedItems)
    //   {
    //     _hub.account = _accounts[_accountList.selectedIndex];
    //   }
    //
    //   _accountList.onSelectionChange += onAccountSelected;
    // }

  }

}
