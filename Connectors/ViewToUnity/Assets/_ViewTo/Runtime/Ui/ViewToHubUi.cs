using Speckle.ConnectorUnity;
using Speckle.ConnectorUnity.Elements;
using Speckle.ConnectorUnity.Ops;
using Speckle.Core.Credentials;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using ViewObjects.Unity;

namespace ViewTo.Connector.Unity
{

  public class ViewToHubUi : MonoBehaviour
  {

    [SerializeField] HubStage stage;
    [SerializeField] UIDocument ui;
    [SerializeField] VisualTreeAsset streamListItem;

    List<Account> _accounts;
    ListView _accountList;
    ViewToHub _hub;
    SpeckleConnector _connector;
    AccountElement _account;

    List<SpeckleStream> _streams;
    ListView _streamsList;
    ListView _details;
    DropdownField _branches, _commits, _studies;


    public enum HubStage
    {
      SelectStream,
      SearchStream,
      ShowStudy,
      RunStudy

    }

    void Start()
    {


      stage = HubStage.SearchStream;

      if(ui == null)
      {
        Debug.Log($"{this} is missing a {typeof(UIDocument)}");
        return;
      }

      _account = ui.rootVisualElement.Q<AccountElement>();
      _streamsList = ui.rootVisualElement.Q<ListView>("stream-list");
      _studies = ui.rootVisualElement.Q<DropdownField>("studies");
      _details = ui.rootVisualElement.Q<ListView>();


      _hub = ViewToHub.Instance;
      _connector = SpeckleConnector.instance;

      if(_hub == null)
      {
        Debug.Log($"{this} needs an instance of {nameof(ViewToHub)}");
        return;
      }

      if(_connector == null)
      {
        Debug.Log($"{this} needs an instance of {nameof(SpeckleConnector)}");
        return;
      }

      _hub.OnStudiesFound += SetStudies;
      _connector.OnInitialize += SetAccount;
      // _connector.OnStreamsLoaded += SetStreams;

      SetAccount();
      SetStage();
    }


    void SetStage()
    {
      switch(stage)
      {

        case HubStage.SearchStream:
          SetupStreamUI();
          break;
        case HubStage.SelectStream:
          break;
        case HubStage.ShowStudy:
          break;
        case HubStage.RunStudy:
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }

    }

    void SetupStreamUI()
    {
      SetRootVisible(true, "streams");
      SetRootVisible(false, "stream-selection");
      SetRootVisible(false, "study-selection");
      SetRootVisible(false, "footer");

    }

    void SetStudies(List<ViewStudy> args)
    { }

    void SetRootVisible(bool value, string ueName)
    {
      var root = ui.rootVisualElement.Q<VisualElement>(ueName);
      if(root == null)
      {
        Debug.Log($"no root with name {ueName}");
        return;
      }
      root.visible = value;
    }

    void SetStreams(List<SpeckleStream> args)
    {
      _streams = args;
      _streamsList.makeItem = streamListItem.CloneTree;

      _streamsList.bindItem = (item, index) =>
      {
        var i = item.Q<SpeckleStreamListItem>();
        if(i == null)
        {
          Debug.Log("no item found");
        }
        // i.SetValueWithoutNotify(_streams[index]);
        item.Q<SpeckleStreamListItem>().SetValueWithoutNotify(_streams[index]);
      };


      _streamsList.fixedItemHeight = 45;
      _streamsList.selectionType = SelectionType.Single;
      _streamsList.reorderable = false;
      _streamsList.itemsSource = _streams;

      _streamsList.onSelectionChange += (objects) =>
      {
        _hub.stream = _streams[_streamsList.selectedIndex];
      };
    }

    void SetAccount()
    {
      _account.value = SpeckleConnector.instance.accounts.FirstOrDefault();
    }


  }

}
