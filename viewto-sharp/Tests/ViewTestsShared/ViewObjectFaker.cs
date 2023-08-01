using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using ViewObjects;
using ViewObjects.Clouds;
using ViewObjects.Common;
using ViewObjects.Contents;
using ViewObjects.References;
using ViewObjects.Results;
using ViewObjects.Studies;
using ViewObjects.Systems;
using ViewObjects.Systems.Layouts;

namespace ViewTo.Tests
{

  public static class ViewObjectFaker
  {

    public static IViewStudy ViewStudy(string name = "test")
    {
      List<IViewObject> objects = new();
      List<Content> content = new();

      foreach(ViewContentType type in Enum.GetValues(typeof(ViewContentType)))
      {
        content.Add(Content(type, $"test-{type}"));
      }

      var cloud = Cloud<ViewCloud>(100);

      objects.Add(new ViewCloudReference(cloud, new List<string>()));
      objects.AddRange(content.Select(x => new ContentReference(x, new List<string>() { })).ToList());
      objects.Add(new Viewer(new List<ILayout>
        {new LayoutCube()}));
      return new ViewStudy(objects, name);
    }

    [Obsolete("Use ViewStudy() instead")]
    public static IViewStudy Study(string name = "test", bool hasResults = true)
    {
      List<IViewObject> objects = new();
      List<Content> content = new();

      foreach(ViewContentType type in Enum.GetValues(typeof(ViewContentType)))
      {
        content.Add(Content(type, $"test-{type}"));
      }

      var cloud = Cloud<ViewCloud>(100);

      objects.Add(new ViewCloudReference(cloud, new List<string>()));
      objects.AddRange(content.Select(x => new ContentReference(x, new List<string>() { })).ToList());
      objects.Add(new Viewer(new List<ILayout>
        {new LayoutCube()}));
      return new ViewStudy(objects, name);
    }

    public static Content Content(ViewContentType type, string name = "test")
    {
      return new(type, ObjUtils.InitGuid, name);
    }

    public static TCloud Cloud<TCloud>(int pointCount)
      where TCloud : IViewCloud
    {
      var obj = Activator.CreateInstance<TCloud>();
      obj.Points = CloudPoints(pointCount);
      return obj;
    }

    public static TCloud ResultCloud<TCloud>(int pointCount, int colorCount)
      where TCloud : IResultCloud<IResultCloudData>
    {
      var obj = Activator.CreateInstance<TCloud>();
      obj.Points = CloudPoints(pointCount);
      obj.Data = Results(pointCount, colorCount);
      return obj;
    }

    public static List<IResultCloudData> Results(int pointCount, int colorCount)
    {
      var values = new List<IResultCloudData>();
      var random = new Random();
      var targets = new List<IContentInfo>();

      // setup the targets 
      for(var c = 0; c<colorCount; c++) targets.Add(new ContentInfo(ObjUtils.InitGuid, c.ToString()));

      // setup other data items
      var existing = new Content(ViewContentType.Existing, ObjUtils.InitGuid);
      var proposed = new Content(ViewContentType.Existing, ObjUtils.InitGuid);

      // setup data for each target
      foreach(var target in targets)
      {
        var potentialOption = new ContentOption(target, target, ViewContentType.Potential);
        var existingOption = new ContentOption(target, existing, ViewContentType.Existing);
        var proposedOption = new ContentOption(target, proposed, ViewContentType.Proposed);

        values.Add(new ResultCloudData(Values(pointCount, random), potentialOption));
        values.Add(new ResultCloudData(Values(pointCount, random), existingOption));
        values.Add(new ResultCloudData(Values(pointCount, random), proposedOption));
      }

      return values;
    }

    public static CloudPoint[] CloudPoints(int count)
    {
      var rnd = new Random();
      var points = new CloudPoint[count];
      for(var i = 0; i<points.Length; i++)
      {
        points[i] = new CloudPoint(rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble(),
          rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble(), "1234-567-890");
      }

      return points;
    }

    public static List<int> Values(int valueCount, Random rnd = null)
    {
      rnd ??= new Random();

      var values = new List<int>();
      for(var j = 0; j<valueCount; j++)
      {
        var bytes = new byte[4];
        rnd.NextBytes(bytes);
        values.Add((int)BitConverter.ToUInt32(bytes, 0));
      }

      return values;
    }

    public static void Similar(this IResultCloudData dataA, IResultCloudData dataB)
    {
      Assert.IsTrue(dataA != default(object) && dataB != default(object));
      Assert.IsTrue(dataA.count.Equals(dataB.count));
      Assert.IsTrue(dataA.values.Count == dataB.values.Count);
      Assert.IsTrue(dataA.info.stage.Equals(dataB.info.stage)
                    && dataA.info.target.ViewId.Equals(dataB.info.target.ViewId)
                    && dataA.info.content.ViewId.Equals(dataB.info.content.ViewId)
      );
    }
    


  }

}
