using NUnit.Framework;
using Objects.Geometry;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Speckle.Core.Transports;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViewObjects;
using ViewObjects.Converter;
using VS = ViewObjects.Speckle;

namespace ViewTo.Tests.Objects
{
  [TestFixture]
  [Category(Categories.CMD)]
  public class RandomHelperTests
  {
    private Client _client;
    private ServerTransport _transport;
    private (string id, string branch, string commit) _stream;

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
        new ContentReference(new List<string> { "bf100bfa87" }, ContentType.Target, ObjUtils.InitGuid, "Capitol Dome"),
        new ContentReference(new List<string> { "2c47ef9dd9" }, ContentType.Target, ObjUtils.InitGuid, "Capitol Base"),
        new ContentReference(new List<string> { "4a592aa84e" }, ContentType.Existing, ObjUtils.InitGuid, "Topo"),
        new ContentReference(new List<string> { "3f2c42f4eb" }, ContentType.Existing, ObjUtils.InitGuid, "Context Buildings"),
        new ContentReference(new List<string> { "1b874412bb" }, ContentType.Existing, ObjUtils.InitGuid, "Trees"),
        new ContentReference(new List<string> { "749482d9c9" }, ContentType.Existing, ObjUtils.InitGuid, "CCMP Monuments"),
        new ContentReference(new List<string> { "aeb3f65de0" }, ContentType.Existing, ObjUtils.InitGuid, "CCMP Buildings"),
        new ViewCloudReference(new List<string> { "77a617d330" }, ObjUtils.InitGuid),
        new Viewer(new List<IViewerLayout>() { new LayoutHorizontal() })
      };

      _client = new Client(AccountManager.GetDefaultAccount());

      // test stream for view to 
      _stream.id = "81c40b04df";
      _stream.branch = "studies/site";

      var obj = new ViewStudy(contents, "site");
      var converter = new ViewObjectsConverter();
      var @base = converter.ConvertToSpeckle(obj);

      _transport = new ServerTransport(_client.Account, _stream.id);

      var id = await Operations.Send(@base, new List<ITransport> { _transport });

      Assert.IsNotNull(id);

      var commit = await _client.CommitCreate(new CommitCreateInput
      {
        streamId = _stream.id,
        branchName = _stream.branch,
        objectId = id,
        message = "Draped Topo"
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
      if (cloud["@A"] is IList itemsA)
      {
        values.AddRange(itemsA.Cast<double>());
      }

      cloud = await Operations.Receive("67f4245e919c1861fc4595f227f79639", _transport);
      if (cloud["@A"] is IList itemsB)
      {
        values.AddRange(itemsB.Cast<double>());
      }


      var pointCloud = new Pointcloud
      {
        points = values.ToList()
      };

      var id = await Operations.Send(pointCloud, new List<ITransport> { _transport });

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

      if (obj is not ViewStudy study)
      {
        Assert.Fail($"Object should be converted to View Study but is " + (obj == null ? "null" : obj.GetType()));
        return;

      }

      var cloud = study.Get<ResultCloud>();
      for (var i = 0; i < study.Objects.Count; i++)
      {
        var o = study.Objects[i];
        if (o is ResultCloud)
        {
          study.Objects.RemoveAt(i);
          study.Objects.Insert(i, new ResultCloud(
            cloud.Points,
            cloud.Data,
            "39d20718-e961-47f3-8f26-fb4296c32228"));
        }
      }

      var @base = converter.ConvertToSpeckle(study);

      _transport = new ServerTransport(_client.Account, _stream.id);

      var id = await Operations.Send(@base, new List<ITransport> { _transport });

      var commit = await _client.CommitCreate(new CommitCreateInput
      {
        streamId = _stream.id,
        branchName = "studies/site",
        objectId = id,
        message = "Draped Topo"
      });

      ViewTests.WriteSpeckleURL(StreamWrapperType.Commit, commit, _stream.id, _client.ServerUrl);


    }


  }
}
