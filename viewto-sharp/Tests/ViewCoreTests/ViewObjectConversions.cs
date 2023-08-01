using NUnit.Framework;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Speckle.Core.Transports;
using System;
using System.IO;
using System.Threading.Tasks;
using ViewObjects;
using ViewObjects.Clouds;
using ViewObjects.Converter;
using ViewObjects.Studies;
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
    public async Task CreateCsv()
    {
      _stream.id = "3ec95efa2d";
      _stream.branch = "main";
      _stream.commit = "bdc5c1fd6e";
      // bdc5c1fd6e
      // e900657494

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
      var cloud = viewStudy.FindObject<ResultCloud>();
      Assert.IsNotNull(cloud);


      var lines = new string[cloud.Points.Length + 1];

      lines[0] = "x,y,z,";
      foreach(var d in cloud.Data)
      {
        lines[0] += $"{d.info.target}-{d.info.content}-{d.info.stage},";
      }

      lines[0] = lines[0].TrimEnd(',');


      for(var i = 0; i < cloud.Points.Length; i++)
      {
        var ptn = cloud.Points[i];
        var line = $"{ptn.x},{ptn.y},{ptn.z}";

        foreach(var d in cloud.Data)
        {
          line += $",{d.values[i]}";
        }

        lines[i + 1] = line;

      }

      File.WriteAllLines(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{viewStudy.ViewName}.csv"), lines);




    }


  }

}
