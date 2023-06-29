using NUnit.Framework;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Speckle.Core.Kits;
using Speckle.Core.Models;
using Speckle.Core.Transports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViewObjects;
using ViewObjects.Clouds;
using ViewObjects.Contents;
using ViewObjects.Converter;
using ViewObjects.Results;
using ViewObjects.Studies;
using ViewTo;
using ViewTo.Tests;

[TestFixture]
[Category(Categories.INT)]
public class ExplorerCommands
{

  [SetUp]
  public void Init()
  {
    _client = new Client(AccountManager.GetDefaultAccount());

  }

  [TearDown]
  public void BreakDown()
  {
    _transport?.Dispose();
    _client?.Dispose();
  }


  Client _client;
  IExplorer _explorer;
  ServerTransport _transport;
  (string id, string branch, string commit) _stream;

  [Test]
  public async Task SetNewId()
  {

    _client = new Client(AccountManager.GetDefaultAccount());
    _transport = new ServerTransport(_client.Account, "a823053e07");

    var commit = await _client.CommitGet(_transport.StreamId, "e10c55d81e");
    var obj = await Operations.Receive(commit.referencedObject, _transport);

    var study = obj.SearchForType<ViewObjects.Speckle.ViewStudy>(true);
    var isValid = false;
    for(int i = 0; i<study.objects.Count; i++)
    {
      var item = study.objects[i];
      if(item is ViewObjects.Speckle.ResultCloud rc)
      {
        rc.ViewId = Guid.NewGuid().ToString();
        study.objects[i] = rc;
        isValid = true;
        break;
      }
    }

    if(!isValid) return;

    var wrapper = new Base() {["@Data"] = study};
    var sent = await Operations.Send(wrapper, new List<ITransport>() {_transport});
    var newCommit = await _client.CommitCreate(new CommitCreateInput()
    {
      branchName = "viewstudies/magpie-models",
      message = "Updated Result Id",
      objectId = sent,
      streamId = _transport.StreamId,
    });

    Console.WriteLine(newCommit);




  }

  [Test]
  public void GetAccountInfo()
  {

    var account = AccountManager.GetDefaultAccount();
    Console.WriteLine(account.ToString());
    Console.WriteLine(account.serverInfo.url);
    _client = new Client(account);

    Console.WriteLine($"Base Uri{_client.GQLClient.HttpClient.BaseAddress}");
    var sqLite = new SQLiteTransport();
    var server = new ServerTransport(account, "628d9282f9");
    Console.WriteLine("SQLite");
    Console.WriteLine($"Root Path={sqLite.RootPath}");
    Console.WriteLine($"Transport Name={sqLite.TransportName}");
    Console.WriteLine("Server");
    Console.WriteLine($"Base Uri={server.BaseUri}");
    // Console.WriteLine($"Api={server.}");

  }

  [Test]
  public async Task FetchStudy_LoadResults_GetSols()
  {
    // UPH Stream
    _stream.id = "a823053e07";
    _stream.branch = "main";
    _stream.commit = "325b010fda";

    var viewStudy = await CheckStudyReceive();
    var rc = viewStudy.Get<IResultCloud>();
    Assert.IsNotNull(rc);

    _explorer = new Explorer();
    _explorer.Load(rc);

    CheckLoadingExplorer(rc.ViewId);

    var options = rc.GetAllOpts();
    Assert.IsNotEmpty(options);

    Assert.IsTrue(_explorer.TryGetSols(options[0], options[2], out var values));
    Assert.IsNotEmpty(values);

  }


  [Test]
  public async Task FetchStudy_LoadResult_ToExplorer()
  {
    _stream.id = "140f5691d0";
    _stream.branch = "main";
    _stream.commit = "d5845812a3";

    var commit = await _client.CommitGet(_stream.id, _stream.commit);
    Assert.IsNotNull(commit);

    _transport = new ServerTransport(_client.Account, _stream.id);
    var data = await Operations.Receive(commit.referencedObject, _transport);
    Assert.IsNotNull(data);

    var speckleStudy = data.SearchForType<ViewObjects.Speckle.ViewStudy>(true);
    Assert.IsNotNull(speckleStudy);

    var viewStudy = (IViewStudy)new ViewObjectsConverter().ConvertToNative(speckleStudy);
    Assert.IsNotNull(viewStudy);

    var rc = viewStudy.Get<IResultCloud>();
    Assert.IsNotNull(rc);

    _explorer = new Explorer();
    _explorer.Load(rc);

    CheckLoadingExplorer(rc.ViewId);

    // Set commands 
    // NOTE: direct reference from the commit in stream 
    var targetToSelect = new ContentInfo("904caeed-54e7-4a76-a811-0187d33d1c43", "water");
    var contentToSelect = new ContentInfo("27288995-a549-4979-a0ab-8d973e0c9260", "ae-1");
    var option = new ContentOption(targetToSelect, contentToSelect, ViewContentType.Proposed);

    // you only need to pass in a specific content if using the proposed type 
    Assert.IsTrue(_explorer.cloud.HasTarget(targetToSelect.ViewId));
    Assert.IsTrue(_explorer.cloud.HasOpt(targetToSelect.ViewId, contentToSelect.ViewId, option.stage));

    _explorer.SetOption(targetToSelect.ViewId, ViewContentType.Existing);
    Assert.IsTrue(_explorer.meta.activeOptions.Count == 1, "This command should create a new list and add the option to it");
    Assert.IsTrue(_explorer.meta.activeOptions.First().stage == ViewContentType.Existing);
    Assert.IsTrue(_explorer.meta.activeOptions.First().target.ViewId.Equals(targetToSelect.ViewId));
    Assert.IsTrue(_explorer.meta.activeOptions.First().content.ViewId.Equals(targetToSelect.ViewId));

    _explorer.SetOption(targetToSelect.ViewId, contentToSelect.ViewId, option.stage);
    Assert.IsTrue(_explorer.meta.activeOptions.Count == 1, "This command should create a new list and add the option to it");
    Assert.IsTrue(_explorer.meta.activeOptions.First().stage == option.stage);
    Assert.IsTrue(_explorer.meta.activeOptions.First().target.ViewId.Equals(targetToSelect.ViewId));
    Assert.IsTrue(_explorer.meta.activeOptions.First().content.ViewId.Equals(contentToSelect.ViewId));
  }


  void CheckLoadingExplorer(string rc)
  {
    Assert.IsNotNull(_explorer.cloud, $"{nameof(Explorer)} should have a {nameof(IResultCloud)} attached");
    Assert.IsTrue(_explorer.cloud.ViewId.Equals(rc), $"{nameof(Explorer)} should have the same ids");

    // all of the basic data for loading
    Assert.IsNotNull(_explorer.data, $"{nameof(Explorer)} should have a {nameof(IResultCloudData)} attached");
    Assert.IsNotNull(_explorer.settings, $"{nameof(Explorer)} should have a {nameof(ExplorerSettings)} attached");
    Assert.IsNotNull(_explorer.meta, $"{nameof(Explorer)} should have a {nameof(ExplorerMetaData)} attached");

    // tests for for checking values match up 
    Assert.IsNotNull(_explorer.meta.activeStage, $"{nameof(ExplorerMetaData)} should have {nameof(ViewContentType)} attached");
    Assert.IsNotNull(_explorer.meta.activeTarget, $"{nameof(ExplorerMetaData)} should have a target {nameof(IContentInfo)} attached");
    Assert.IsNotEmpty(_explorer.meta.options, $"{nameof(ExplorerMetaData)} should contain {nameof(IContentOption)}");
    Assert.IsNotEmpty(_explorer.meta.activeOptions, $"{nameof(ExplorerMetaData)} should should contain the first {nameof(IContentOption)}");
  }

  async Task<IViewStudy> CheckStudyReceive()
  {
    var commit = await _client.CommitGet(_stream.id, _stream.commit);
    Assert.IsNotNull(commit);

    _transport = new ServerTransport(_client.Account, _stream.id);
    var data = await Operations.Receive(commit.referencedObject, _transport);
    Assert.IsNotNull(data);

    var speckleStudy = data.SearchForType<ViewObjects.Speckle.ViewStudy>(true);
    Assert.IsNotNull(speckleStudy);

    var viewStudy = (IViewStudy)new ViewObjectsConverter().ConvertToNative(speckleStudy);
    Assert.IsNotNull(viewStudy);

    return viewStudy;
  }

}
