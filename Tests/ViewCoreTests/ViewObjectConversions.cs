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
using ViewObjects.References;
using ViewObjects.Results;
using ViewObjects.Studies;
using ViewObjects.Systems;
using ViewObjects.Systems.Layouts;
using ViewTo;
using ViewTo.Tests;

namespace ViewCoreTests
{

  [TestFixture]
  [Category(Categories.INT)]
  public class ViewObjectConversions
  {


    [OneTimeTearDown]
    public void BreakDown()
    {
      _transport?.Dispose();
      _client?.Dispose();
    }

    Client _client;
    ServerTransport _transport;

    (string id, string branch, string commit) _stream;

    [Test]
    [Order(00)]
    [Ignore("A hack for modifying a view content type")]
    public async Task CreateStudy_ContentType()
    {
      _stream.id = "628d9282f9";
      _stream.branch = "studies/facade";
      _stream.commit = "8e14951f11";

      _client = new Client(AccountManager.GetDefaultAccount());
      var commit = await _client.CommitGet(_stream.id, _stream.commit);
      Assert.IsNotNull(commit);

      _transport = new ServerTransport(_client.Account, _stream.id);
      var data = await Operations.Receive(commit.referencedObject, _transport);
      Assert.IsNotNull(data);

      var speckleStudy = data.SearchForType<ViewObjects.Speckle.ViewStudy>(true);
      Assert.IsNotNull(speckleStudy);

      var converter = new ViewObjectsConverter();
      var viewStudy = (IViewStudy)converter.ConvertToNative(speckleStudy);
      Assert.IsNotNull(viewStudy);

      var items = new List<IViewObject>();
      var vcs = viewStudy.GetAll<ContentReference>();
      Assert.IsNotNull(vcs);

      foreach(var co in vcs)
      {
        var item = co.ViewName == null ?
          new ContentReference(co.References, ViewContentType.Existing, co.ViewId, co.ViewName) :
          co;

        items.Add(item);
      }


      var cloud = viewStudy.FindObject<ViewCloudReference>();
      Assert.IsNotNull(cloud);
      items.Add(cloud);

      var viewer = viewStudy.FindObject<Viewer>();
      Assert.IsNotNull(viewer);
      items.Add(viewer);

      Assert.IsTrue(items.Count == viewStudy.objects.Count);
      viewStudy.objects = items;

      var updatedStudy = (ViewObjects.Speckle.ViewStudy)converter.ConvertToSpeckle(viewStudy);
      Assert.IsNotNull(updatedStudy);
      Assert.IsTrue(updatedStudy.objects.Count == viewStudy.objects.Count);

      var refObj = new Base()
      {
        ["@Data"] = updatedStudy
      };

      var result = await Operations.Send(refObj, new List<ITransport>() {_transport});
      Assert.IsNotNull(result);
      Console.WriteLine(result);

      var rId = await _client.CommitCreate(new CommitCreateInput()
      {
        branchName = _stream.branch,
        message = "Corrected Types from Test",
        sourceApplication = HostApplications.NET.Name,
        streamId = _stream.id,
        objectId = result
      });

      Assert.IsNotNull(rId);
      Console.WriteLine(rId);


    }


  }

}
