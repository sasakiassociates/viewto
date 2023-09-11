using NUnit.Framework;
using System;
using System.Collections.Generic;
using ViewObjects;
using ViewObjects.Clouds;
using Sasaki.Common;
using ViewObjects.Contents;
using ViewObjects.Converter;
using ViewObjects.References;
using ViewObjects.Studies;
using ViewObjects.Systems;
using VS = ViewObjects.Speckle;

namespace ViewTo.Tests.Objects
{

  [TestFixture]
  [Category(Categories.UNITS)]
  public class ObjectsTest
  {

    private ViewObjectsConverter _converter;

    [OneTimeSetUp]
    public void Setup()
    {
      _converter = new ViewObjectsConverter();
    }

    [OneTimeTearDown]
    public void BreakDown()
    { }

    [Test]
    public void Convert_Content()
    {
      var values = Enum.GetValues<ViewContentType>();

      foreach(var contentType in values)
      {
        var obj = new ContentReference(new Content(contentType), new List<string>() {"123443q312"});
        var res = _converter.ConvertToSpeckle(obj) as VS.ContentReference;
        Assert.IsTrue(res.appId.Equals(obj.ViewId));
        Assert.IsTrue(res.contentType == obj.contentType);
      }

    }

    [Test]
    public void Convert_Study()
    {
      var objs = new List<IViewObject>
      {
        new ContentReference(new Content(ViewContentType.Potential), new List<string>() {"123443q312"}),
        new ViewCloudReference(new List<string> {"256ff84cf7"}, SasakiTools.InitGuid),
        new Viewer()
      };

      var obj = new ViewStudy(objs, "Test View Study");

      var converter = new ViewObjectsConverter();
      var res = converter.ConvertToSpeckle(obj);

      Assert.IsNotNull(res);
      Assert.IsTrue(res is ViewObjects.Speckle.ViewStudy);

      var casted = res as ViewObjects.Speckle.ViewStudy;
      Assert.IsTrue(obj.ViewId.Equals(casted.guid));
      Assert.IsTrue(obj.name.Equals(casted.name));
      Assert.IsTrue(obj.objects.Count.Equals(casted.objects.Count));
    }
    
    
    [Test]
    public void Convert_ResultCloud()
    {
      var obj = ViewObjectFaker.ResultCloud<ResultCloud>(100, 2);

      var res = _converter.ConvertToSpeckle(obj);
      Assert.IsNotNull(res);
      Assert.IsTrue(res is ViewObjects.Speckle.ResultCloud);

      var casted = res as ViewObjects.Speckle.ResultCloud;
      Assert.IsTrue(obj.guid.Equals(casted.guid));
      Assert.IsTrue(obj.Points.Length == casted.Points.Length);
      Assert.IsTrue(obj.layers.Count == casted.layers.Count);

      for(var i = 0; i<obj.layers.Count; i++)
      {
        var dataOg = obj.layers[i];
        var dataCasted = casted.layers[i];

        dataOg.Similar(dataCasted);

        for(int y = 0; y<dataOg.values.Count; y++)
        {
          Assert.IsTrue(dataOg.values[y] == dataCasted.values[y]);
        }
      }
    }
  }

}
