using NUnit.Framework;
using Objects.Organization;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Speckle.Core.Models;
using Speckle.Core.Transports;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ViewObjects;
using ViewObjects.Clouds;
using ViewObjects.Converter;
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

    private Client _client;
    private ServerTransport _transport;

    private(string id, string branch, string commit) _stream;

    [Test]
    public async Task Convert_ReferenceObject()
    {
      const string commitObjectReferenceId = "fdfacc1fd63b1e633e06d31a2997267b";
      // NOTE: using a commit id seems to upset the converter and ends up hashing the ID into a new reference ID 
      // const string commitId = "9a2048729";

      var reference = new ObjectReference()
      {
        referencedId = commitObjectReferenceId
      };

      Console.WriteLine("type=" + reference.speckle_type);
      Console.WriteLine("id=" + reference.referencedId);

      _client = new Client(AccountManager.GetDefaultAccount());

      _stream.id = "6f3acfb477";
      _stream.branch = "dev/dm";

      _transport = new ServerTransport(_client.Account, _stream.id);

      var wrapper = new Base
      {
        ["reference object"] = reference
      };

      var container = new Container("some container", new List<Base>() {wrapper});
      // var container = new Base();
      var id = await Operations.Send(wrapper, new List<ITransport> {_transport});

      Assert.IsNotNull(id);

      var commit = await _client.CommitCreate(new CommitCreateInput
      {
        streamId = _stream.id,
        branchName = _stream.branch,
        objectId = id,
        message = "Test commit from Script"
      });

      Assert.IsNotNull(commit);
      Console.WriteLine(commit);

      var res = await Operations.Receive(id, _transport);

      Assert.IsNotNull(res);
    }

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
    public async Task CheckMaxMinValues()
    {
      _client = new Client(AccountManager.GetDefaultAccount());

      // test stream for view to 
      _stream.id = "a823053e07";
      _stream.branch = "main";
      _stream.commit = "325b010fda";

      var commit = await _client.CommitGet(_stream.id, _stream.commit);
      _transport = new ServerTransport(_client.Account, _stream.id);

      var studyBase = await Operations.Receive(commit.referencedObject, _transport);

      var converter = new ViewObjectsConverter();
      var study = converter.ConvertToNativeViewObject(studyBase.SearchForType<VS.ViewStudy>(true)) as ViewStudy;

      var cloud = study.FindObject<ResultCloud>();

      foreach(var data in cloud.Data)
      {
        var min = 1;
        var max = 1;

        foreach(var v in data.values)
        {
          if(v < min) min = v;
          else if(v > max) max = v;
        }
        Console.WriteLine($"min={min} : max={max}");
      }

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
