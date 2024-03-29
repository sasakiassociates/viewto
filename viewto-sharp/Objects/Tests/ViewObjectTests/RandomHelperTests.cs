﻿using NUnit.Framework;
using Objects.Geometry;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Speckle.Core.Transports;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ViewObjects;
using ViewObjects.Clouds;
using ViewObjects.Common;
using ViewObjects.Converter;
using ViewObjects.References;
using ViewObjects.Studies;
using ViewObjects.Systems;
using ViewObjects.Systems.Layouts;
using VS = ViewObjects.Speckle;

namespace ViewTo.Tests.Objects
{

  [TestFixture]
  [Category(Categories.CMD)]
  public class RandomHelperTests
  {
    private Client _client;
    private ServerTransport _transport;
    private(string id, string branch, string commit) _stream;

    [TearDown]
    public void BreakDown()
    {
      _transport?.Dispose();
      _client?.Dispose();
    }

    [Test]
    public async Task Create_ViewStudy_FromScript()
    {
      var contents = new List<IViewObject>
      {
        new ContentReference(new List<string> {"2e14d7936c"}, ViewContentType.Potential, ObjUtils.InitGuid, "Park"),
        new ContentReference(new List<string> {"a7af04b287"}, ViewContentType.Potential, ObjUtils.InitGuid, "Water"),
        new ContentReference(new List<string> {"afc4a766e5"}, ViewContentType.Potential, ObjUtils.InitGuid, "Landmark"),
        new ContentReference(new List<string> {"e91a49c540"}, ViewContentType.Potential, ObjUtils.InitGuid, "Mountain"),
        new ContentReference(new List<string> {"bc49a5e0b0"}, ViewContentType.Potential, ObjUtils.InitGuid, "Mountain-Top-View-Deck"),
        new ContentReference(new List<string> {"813d4062e8"}, ViewContentType.Existing, ObjUtils.InitGuid, "Topo"),
        new ContentReference(new List<string> {"b2fe722cb8"}, ViewContentType.Existing, ObjUtils.InitGuid, "Buildings-Adjacent"),
        new ContentReference(new List<string> {"bfe46788b8"}, ViewContentType.Existing, ObjUtils.InitGuid, "Buildings-Context"),
        new ViewCloudReference(new List<string> {"e01e2058b2"}, ObjUtils.InitGuid),
        new Viewer(new List<ILayout> {new LayoutHorizontal()})
      };

      _client = new Client(AccountManager.GetDefaultAccount());

      // test stream for view to 
      _stream.id = "9a45304b95";
      _stream.branch = "studies/towers";

      var obj = new ViewStudy(contents, "towers");
      var converter = new ViewObjectsConverter();
      var @base = converter.ConvertToSpeckle(obj);

      _transport = new ServerTransport(_client.Account, _stream.id);

      var id = await Operations.Send(@base, new List<ITransport> {_transport});

      Assert.IsNotNull(id);

      var commit = await _client.CommitCreate(new CommitCreateInput
      {
        streamId = _stream.id,
        branchName = _stream.branch,
        objectId = id,
        message = "First study creations"
      });

      Assert.IsNotNull(commit);
      Console.WriteLine($"Commit {commit}\n", _client.ServerUrl + "/streams/" + _stream.id + "/commits/" + commit);
    }


    [Test]
    public async Task Create_ViewCloud_FromGrasshopperObjectNode()
    {
      _client = new Client(AccountManager.GetDefaultAccount());

      // test stream for view to 
      _stream.id = "81c40b04df";
      _stream.branch = "clouds/site";

      _transport = new ServerTransport(_client.Account, "81c40b04df");

      var values = new List<double>();
      var cloud = await Operations.Receive("e3da2a2017b94e5c8fe8ee6a25ac194c", _transport);
      if(cloud["@A"] is IList itemsA)
      {
        values.AddRange(itemsA.Cast<double>());
      }

      cloud = await Operations.Receive("67f4245e919c1861fc4595f227f79639", _transport);
      if(cloud["@A"] is IList itemsB)
      {
        values.AddRange(itemsB.Cast<double>());
      }


      var pointCloud = new Pointcloud
      {
        points = values.ToList()
      };

      var id = await Operations.Send(pointCloud, new List<ITransport> {_transport});

      var commit = await _client.CommitCreate(new CommitCreateInput
      {
        streamId = _stream.id,
        branchName = _stream.branch,
        objectId = id,
        message = "Updated to meters"
      });

      Console.WriteLine($"Commit {commit}\n", _client.ServerUrl + "/streams/" + _stream.id + "/commits/" + commit);


    }


    [Test]
    public async Task Modify_ViewId_FromScript()
    {

      _client = new Client(AccountManager.GetDefaultAccount());

      // test stream for view to 
      _stream.id = "81c40b04df";
      _stream.branch = "main";

      _transport = new ServerTransport(_client.Account, _stream.id);

      var converter = new ViewObjectsConverter();

      var item = await Operations.Receive("82be3de3a781e6ea0652edf9000850e3", _transport);
      var obj = converter.ConvertToNative(item);

      if(obj is not ViewStudy study)
      {
        Assert.Fail($"Object should be converted to View Study but is " + (obj == null ? "null" : obj.GetType()));
        return;

      }

      var cloud = study.Get<ResultCloud>();
      for(var i = 0; i < study.objects.Count; i++)
      {
        var o = study.objects[i];
        if(o is ResultCloud)
        {
          study.objects.RemoveAt(i);
          study.objects.Insert(i, new ResultCloud(
            cloud.Points,
            cloud.Data,
            "39d20718-e961-47f3-8f26-fb4296c32228"));
        }
      }

      var @base = converter.ConvertToSpeckle(study);

      _transport = new ServerTransport(_client.Account, _stream.id);

      var id = await Operations.Send(@base, new List<ITransport> {_transport});

      var commit = await _client.CommitCreate(new CommitCreateInput
      {
        streamId = _stream.id,
        branchName = "studies/site",
        objectId = id,
        message = "Draped Topo"
      });

      ViewTests.WriteSpeckleURL(StreamWrapperType.Commit, commit, _stream.id, _client.ServerUrl);


    }

    [Test]
    public async Task Scan_Stream_Objects()
    {
      var timer = new Stopwatch();
      timer.Start();

      _client = new Client(AccountManager.GetDefaultAccount());

      // test stream for view to 
      _stream.id = "81c40b04df";

      var commits = await _client.StreamGetCommits(_stream.id, 10);

      foreach(var commitRef in commits)
      {
        var data = await _client.CommitGet(_stream.id, commitRef.id);

      }

    }

  }

}
