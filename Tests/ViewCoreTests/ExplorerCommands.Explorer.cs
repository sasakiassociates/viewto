﻿using NUnit.Framework;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
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

  [OneTimeTearDown]
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
  public void GetAccountInfo ()
  {

    var account = AccountManager.GetDefaultAccount();
    Console.WriteLine(account.ToString());
    Console.WriteLine(account.serverInfo.url);
    _client = new Client(account);

    Console.WriteLine($"Base Uri{ _client.GQLClient.HttpClient.BaseAddress}");
    var sqLite = new SQLiteTransport();
    var server = new ServerTransport(account, "628d9282f9");
    Console.WriteLine("SQLite");
    Console.WriteLine($"Root Path={sqLite.RootPath}");
    Console.WriteLine($"Transport Name={sqLite.TransportName}");
    Console.WriteLine("Server");
    Console.WriteLine($"Base Uri={server.BaseUri}");
    Console.WriteLine($"Api={server.Api}");

  }
  
  [Test]
  [Ignore("Used to change the values of a content item")]
  public async Task ExplorerCommand_GetValues_WithOptions()
  {

    _stream.id = "628d9282f9";
    _stream.branch = "main";
    _stream.commit = "57f2b967ef";

    _client = new Client(AccountManager.GetDefaultAccount());
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

    Assert.IsNotNull(_explorer.cloud, $"{nameof(Explorer)} should have a {nameof(IResultCloud)} attached");
    Assert.IsTrue(_explorer.cloud.ViewId.Equals(rc.ViewId), $"{nameof(Explorer)} should have the same ids");


    Assert.IsTrue(_explorer.TryGetValues(ExplorerValueType.ExistingOverPotential, new List<IContentOption>()
      {
        new ContentOption(
          new ContentInfo("551b4ded-5bb6-4398-955f-76f8063acffa", "ends"),
          new ContentInfo("551b4ded-5bb6-4398-955f-76f8063acffa", "ends-Existing"),
          ViewContentType.Existing
        ),
        new ContentOption(
          new ContentInfo("c625bf76-4d35-4a80-a0ba-97c6f41f95b9", "third row"),
          new ContentInfo("c625bf76-4d35-4a80-a0ba-97c6f41f95b9", "third row-Existing"),
          ViewContentType.Existing
        )
      },
      out var values));
    
    Assert.IsNotNull(values);
  }

  [Test]
  public async Task FetchStudy_LoadResult_ToExplorer()
  {

    _stream.id = "140f5691d0";
    _stream.branch = "main";
    _stream.commit = "d5845812a3";

    _client = new Client(AccountManager.GetDefaultAccount());
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

    Assert.IsNotNull(_explorer.cloud, $"{nameof(Explorer)} should have a {nameof(IResultCloud)} attached");
    Assert.IsTrue(_explorer.cloud.ViewId.Equals(rc.ViewId), $"{nameof(Explorer)} should have the same ids");

    // Check all meta data 

    // all of the basic data for loading
    Assert.IsNotNull(_explorer.data, $"{nameof(Explorer)} should have a {nameof(IResultCloudData)} attached");
    Assert.IsNotNull(_explorer.settings, $"{nameof(Explorer)} should have a {nameof(ExplorerSettings)} attached");
    Assert.IsNotNull(_explorer.meta, $"{nameof(Explorer)} should have a {nameof(ExplorerMetaData)} attached");

    // tests for for checking values match up 
    Assert.IsNotNull(_explorer.meta.activeStage, $"{nameof(ExplorerMetaData)} should have {nameof(ViewContentType)} attached");
    Assert.IsNotNull(_explorer.meta.activeTarget, $"{nameof(ExplorerMetaData)} should have a target {nameof(IContentInfo)} attached");
    Assert.IsNotEmpty(_explorer.meta.options, $"{nameof(ExplorerMetaData)} should contain {nameof(IContentOption)}");
    Assert.IsNotEmpty(_explorer.meta.activeOptions, $"{nameof(ExplorerMetaData)} should should contain the first {nameof(IContentOption)}");

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

}