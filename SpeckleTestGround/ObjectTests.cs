using NUnit.Framework;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Speckle.Core.Kits;
using Speckle.Core.Models;
using Speckle.Core.Models.Extensions;
using Speckle.Core.Transports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SpeckleTestGround
{

  public class Tests
  {
    [SetUp]
    public void Init()
    {
      Speckle.Core.Logging.SpeckleLog.Initialize(HostApplications.Other.Name, "1");
    }

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
    public async Task BlobItUp()
    {
      var id = "6f3acfb477";
      var branch = "dev/dm";

      _client = new Client(AccountManager.GetDefaultAccount());
      _transport = new ServerTransport(_client.Account, id);

      var blobOut = new Blob(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "blobie.png"));
      var objId = await Operations.Send(new Base() {["Data"] = blobOut}, new List<ITransport>() {_transport});
      var commit = await _client.CommitCreate(new CommitCreateInput() {branchName = branch, objectId = objId, streamId = id, message = "The blob is alive!"});

      Assert.IsNotNull(commit);

      var objIn = await Operations.Receive(objId, _transport);
      var blobIn = (Blob)objIn["Data"];
      Assert.IsNotNull(blobIn);

      Console.WriteLine(blobIn.filePath);
    }

    [TestCase("6f3acfb477", "87578ef1bb")] // magpie
    [TestCase("a823053e07", "0755c1db43")] // point cloud
    [TestCase("a823053e07", "7ba456cb3f")] // view study
    [TestCase("a823053e07", "cb31f00107")] // result cloud data
    public async Task BaseFlattenObjects(string streamId, string commitId)
    {
      Console.WriteLine($"Grabbing data for {streamId}-{commitId}");
      _client = new Client(AccountManager.GetDefaultAccount());
      _transport = new ServerTransport(_client.Account, streamId);

      var obj = await _client.CommitGet(streamId, commitId);
      Assert.That(obj, Is.Not.Null);
      Assert.That(obj.referencedObject, Is.Not.Empty);
      var data = await Operations.Receive(obj.referencedObject, _transport);
      Assert.That(data, Is.Not.Null);

      var flatten = data.Flatten().ToList();
      var range = Math.Min(flatten.Count, 20);

      for(int i = 0; i<range; i++) Console.WriteLine(flatten[i].speckle_type);
    }




  }

}
