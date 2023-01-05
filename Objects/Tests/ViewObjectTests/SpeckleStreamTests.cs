using NUnit.Framework;
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
using ViewObjects.References;
using ViewObjects.Studies;
using VS = ViewObjects.Speckle;
using VO = ViewObjects;

namespace ViewTo.Tests.Objects
{

  [TestFixture]
  [Category(Categories.SPECKLE)]
  public class SpeckleStreamTests
  {

    [TearDown]
    public void BreakDown()
    {
      _transport?.Dispose();
      _client?.Dispose();
    }

    Client _client;
    ServerTransport _transport;

    (string id, string branch, string commit) _stream;


    [Test]
    public async Task SendTestResultCloud()
    {
      _client = new Client(AccountManager.GetDefaultAccount());

      // test stream for view to 
      _stream.id = "1da7b18b31";
      _stream.branch = "conversions";

      var cloud = ViewObjectFaker.ResultCloud<VS.ResultCloud, VS.ResultCloudData>(1000, 2);

      _transport = new ServerTransport(_client.Account, _stream.id);

      var id = await Operations.Send(cloud, new List<ITransport>
        {_transport});

      Assert.IsNotNull(id);

      var commit = await _client.CommitCreate(new CommitCreateInput
      {
        streamId = _stream.id,
        branchName = _stream.branch,
        objectId = id,
        message = "Test commit from Script"
      });

      Assert.IsNotNull(commit);
    }

    public static string TerminalURL(string caption, string url)
    {
      return$"\u001B]8;;{url}\a{caption}\u001B]8;;\a";
    }





    [Test]
    public async Task Send_ViewStudy()
    {
      _client = new Client(AccountManager.GetDefaultAccount());

      // test stream for view to
      _stream.id = "1da7b18b31";
      _stream.branch = "conversions";

      var obj = ViewObjectFaker.Study();
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
        message = "Test commit from Script"
      });

      Assert.IsNotNull(commit);
    }

    [Test]
    [Ignore("Test for merging data")]
    public async Task AddCloudToStudy()
    {
      _client = new Client(AccountManager.GetDefaultAccount());

      // test stream for view to 
      _stream.id = "a823053e07";
      _stream.branch = "viewstudies/site";

      _transport = new ServerTransport(_client.Account, _stream.id);

      var cloudCommit = await _client.CommitGet(_stream.id, "c65fb6fd85");
      var studyCommit = await _client.CommitGet(_stream.id, "34a98434f1");

      var studyBase = await Operations.Receive(studyCommit.referencedObject, _transport);
      var cloudBase = await Operations.Receive(cloudCommit.referencedObject, _transport);

      var converter = new ViewObjectsConverter();
      var study = converter.ConvertToNativeViewObject(studyBase.SearchForType<VS.ViewStudy>(true)) as ViewStudy;
      var cloud = converter.ConvertToNativeViewObject(cloudBase.SearchForType<VS.ResultCloud>(true)) as ResultCloud;

      var targets = study.GetAll<ContentReference>().Where(x => x.ContentType == ContentType.Potential).ToList();

      Assert.IsTrue(targets.Count * 2 == cloud.Data.Count);
      for(var i = 0; i < targets.Count; i++)
      {
        var t = targets[i];
        cloud.Data[i].Option = new ContentOption
        {
          Stage = ContentType.Potential, Id = t.ViewId, Name = t.ViewName
        };
        cloud.Data[i + targets.Count].Option = new ContentOption
        {
          Stage = ContentType.Existing, Id = t.ViewId, Name = t.ViewName
        };
      }

      study.Objects.Add(cloud);

      var res = converter.ConvertToSpeckle(study);
      var id = await Operations.Send(res, new List<ITransport> {_transport});

      var commit = await _client.CommitCreate(new CommitCreateInput
      {
        streamId = _stream.id,
        branchName = _stream.branch,
        objectId = id,
        message = "Test for combinding objects"
      });

      Assert.IsNotNull(commit);
    }

    [Test]
    public async Task ReceiveResultCloudFromCommit()
    {
      _client = new Client(AccountManager.GetDefaultAccount());

      // example from uph stream
      _stream.id = "4777dea055";
      _stream.branch = "viewstudy/massing-from-road";
      _stream.commit = "78dc82b648";

      var commit = await _client.CommitGet(_stream.id, _stream.commit);

      Assert.NotNull(commit);
      _transport = new ServerTransport(_client.Account, _stream.id);

      var @base = await Operations.Receive(commit.referencedObject, _transport);

      Assert.IsNotNull(@base);

      Console.WriteLine($"{@base.speckle_type} Total Child Cout={@base.totalChildrenCount}");

      var obj = @base.SearchForType<VS.ViewStudy>(true);

      Assert.IsNotNull(obj);

      var id = await Operations.Send(obj, new List<ITransport> {_transport});

      var commitCreated = await _client.CommitCreate(new CommitCreateInput
      {
        streamId = _stream.id,
        branchName = _stream.branch,
        objectId = id,
        message = "Test for combinding objects"
      });

      Assert.IsNotNull(obj);
    }
  }

}
