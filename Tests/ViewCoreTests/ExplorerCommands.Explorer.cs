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
using ViewTo.Cmd;
using ViewTo.Tests;
using ViewTo.Values;

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

    Assert.IsTrue(_explorer.TryGetSols(optionA: options[0], optionB: options[2], results: out var values));
    Assert.IsNotEmpty(values);

  }


  [Test]
  public async Task FetchStudy_LoadResult_ToExplorer()
  {
    _stream.id = "140f5691d0";
    _stream.branch = "main";
    _stream.commit = "d5845812a3";

    var commit = await _client.CommitGet(streamId: _stream.id, commitId: _stream.commit);
    Assert.IsNotNull(commit);

    _transport = new ServerTransport(account: _client.Account, streamId: _stream.id);
    var data = await Operations.Receive(objectId: commit.referencedObject, remoteTransport: _transport);
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
    var targetToSelect = new ContentInfo(viewId: "904caeed-54e7-4a76-a811-0187d33d1c43", viewName: "water");
    var contentToSelect = new ContentInfo(viewId: "27288995-a549-4979-a0ab-8d973e0c9260", viewName: "ae-1");
    var option = new ContentOption(target: targetToSelect, value: contentToSelect, stage: ViewContentType.Proposed);

    // you only need to pass in a specific content if using the proposed type 
    Assert.IsTrue(_explorer.cloud.HasTarget(targetToSelect.ViewId));
    Assert.IsTrue(_explorer.cloud.HasOpt(targetId: targetToSelect.ViewId, contentId: contentToSelect.ViewId, stage: option.stage));

    _explorer.SetOption(targetId: targetToSelect.ViewId, stage: ViewContentType.Existing);
    Assert.IsTrue(condition: _explorer.meta.activeOptions.Count == 1, message: "This command should create a new list and add the option to it");
    Assert.IsTrue(_explorer.meta.activeOptions.First().stage == ViewContentType.Existing);
    Assert.IsTrue(_explorer.meta.activeOptions.First().target.ViewId.Equals(targetToSelect.ViewId));
    Assert.IsTrue(_explorer.meta.activeOptions.First().content.ViewId.Equals(targetToSelect.ViewId));

    _explorer.SetOption(targetId: targetToSelect.ViewId, contentId: contentToSelect.ViewId, stage: option.stage);
    Assert.IsTrue(condition: _explorer.meta.activeOptions.Count == 1, message: "This command should create a new list and add the option to it");
    Assert.IsTrue(_explorer.meta.activeOptions.First().stage == option.stage);
    Assert.IsTrue(_explorer.meta.activeOptions.First().target.ViewId.Equals(targetToSelect.ViewId));
    Assert.IsTrue(_explorer.meta.activeOptions.First().content.ViewId.Equals(contentToSelect.ViewId));
  }


  [Test]
  [Ignore("Only used for fixing a id problem from unity")]
  public async Task SetNewId()
  {
    _transport = new ServerTransport(account: _client.Account, streamId: "a823053e07");

    var commit = await _client.CommitGet(streamId: _transport.StreamId, commitId: "e10c55d81e");
    var obj = await Operations.Receive(objectId: commit.referencedObject, remoteTransport: _transport);

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
    var sent = await Operations.Send(@object: wrapper, transports: new List<ITransport>() {_transport});
    var newCommit = await _client.CommitCreate(new CommitCreateInput()
    {
      branchName = "viewstudies/magpie-models",
      message = "Updated Result Id",
      objectId = sent,
      streamId = _transport.StreamId,
    });

    Console.WriteLine(newCommit);
  }

  /// <summary>
  /// This test is meant to validate the command for retrieving values from the explorer using the <see cref="ExplorerFilterInput"/> to modify the final result
  /// </summary>
  [Test]
  public async Task FilterRawViews()
  {
    _transport = new ServerTransport(account: _client.Account, streamId: "a823053e07");

    var commit = await _client.CommitGet(streamId: _transport.StreamId, commitId: "f75cb44d5f");
    var obj = await Operations.Receive(objectId: commit.referencedObject, remoteTransport: _transport);

    var data = new ViewObjectsConverter().ConvertToNative(obj.SearchForType<ViewObjects.Speckle.ViewStudy>(true));
    Assert.IsAssignableFrom(expected: typeof(ViewObjects.Studies.ViewStudy), actual: data);

    var study = data as ViewStudy;
    var cloud = study.Get<ResultCloud>();

    _explorer = new Explorer();
    _explorer.Load(cloud);

    // grab a set of points to remove 
    var indexes = new List<int>();
    for(int i = 10; i<_explorer.cloud.Points.Length-30; i++) indexes.Add(i);

    var rnd = new Random();

    for(int i = indexes.Count-1; i>=0; i--)
    {
      if(rnd.NextDouble()<0.5) indexes.Remove(i);
    }

    var options = cloud.GetAllOpts();

    var valueOption = options[2];
    var maxOption = options[0];

    Console.WriteLine($"Max Option\nTarget: {maxOption.target.ViewName}\nContent: {maxOption.content.ViewName}\nType: {maxOption.target.type}\n"+
                      "-------------------------\n"+
                      $"Value Option\nTarget: {valueOption.target.ViewName}\nContent: {valueOption.content.ViewName}\nType: {valueOption.target.type}");

    const double modifier = 0.001;
    var max = (int)Math.Floor(2147483647*modifier);
    var min = 0;

    Console.WriteLine($"Max value is {max}(modifier={modifier}");

    var filter = new ExplorerFilterInput(minValue: 0, maxValue: 1, pixelMin: min, pixelMax: max, filteredIndexes: indexes);
    Console.WriteLine(filter.ToString());

    var results = _explorer.GetSols(valueOption: valueOption, maxOption: maxOption, filter: filter).ToArray();

    Assert.IsNotEmpty(results);
    Assert.IsTrue(results.Max()<=filter.valueRange.max);
    Assert.IsTrue(results.Min()>=filter.valueRange.min);
    Assert.IsTrue(results.Length == indexes.Count);

    results = _explorer.GetSols(valueOption: valueOption, maxOption: maxOption, filter: filter, normalizeByFilter: true).ToArray();

    var maxCheck = results.Max();
    var minCheck = results.Min();
    Assert.IsNotEmpty(results);
    Assert.IsTrue(maxCheck<=filter.valueRange.max, $"Max:{maxCheck} - Filter:{filter.valueRange.max}");
    Assert.IsTrue(minCheck>=filter.valueRange.min, $"Min:{minCheck} - Filter:{filter.valueRange.min}");
    Assert.IsTrue(results.Length == indexes.Count);
  }

  void CheckLoadingExplorer(string rc)
  {
    Assert.IsNotNull(anObject: _explorer.cloud, message: $"{nameof(Explorer)} should have a {nameof(IResultCloud)} attached");
    Assert.IsTrue(condition: _explorer.cloud.ViewId.Equals(rc), message: $"{nameof(Explorer)} should have the same ids");

    // all of the basic data for loading
    Assert.IsNotNull(anObject: _explorer.data, message: $"{nameof(Explorer)} should have a {nameof(IResultCloudData)} attached");
    Assert.IsNotNull(anObject: _explorer.settings, message: $"{nameof(Explorer)} should have a {nameof(ExplorerSettings)} attached");
    Assert.IsNotNull(anObject: _explorer.meta, message: $"{nameof(Explorer)} should have a {nameof(ExplorerMetaData)} attached");

    // tests for for checking values match up 
    Assert.IsNotNull(anObject: _explorer.meta.activeStage, message: $"{nameof(ExplorerMetaData)} should have {nameof(ViewContentType)} attached");
    Assert.IsNotNull(anObject: _explorer.meta.activeTarget, message: $"{nameof(ExplorerMetaData)} should have a target {nameof(IContentInfo)} attached");
    Assert.IsNotEmpty(collection: _explorer.meta.options, message: $"{nameof(ExplorerMetaData)} should contain {nameof(IContentOption)}");
    Assert.IsNotEmpty(collection: _explorer.meta.activeOptions, message: $"{nameof(ExplorerMetaData)} should should contain the first {nameof(IContentOption)}");
  }

  async Task<IViewStudy> CheckStudyReceive()
  {
    var commit = await _client.CommitGet(streamId: _stream.id, commitId: _stream.commit);
    Assert.IsNotNull(commit);

    _transport = new ServerTransport(account: _client.Account, streamId: _stream.id);
    var data = await Operations.Receive(objectId: commit.referencedObject, remoteTransport: _transport);
    Assert.IsNotNull(data);

    var speckleStudy = data.SearchForType<ViewObjects.Speckle.ViewStudy>(true);
    Assert.IsNotNull(speckleStudy);

    var viewStudy = (IViewStudy)new ViewObjectsConverter().ConvertToNative(speckleStudy);
    Assert.IsNotNull(viewStudy);

    return viewStudy;
  }

}
