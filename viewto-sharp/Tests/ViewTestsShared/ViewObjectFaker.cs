using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using ViewObjects;
using ViewObjects.Clouds;
using Sasaki.Common;
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
      List<Context> content = new();

      foreach(ViewContextType type in Enum.GetValues(typeof(ViewContextType)))
      {
        content.Add(Content(type, $"test-{type}"));
      }

      var cloud = Cloud<Cloud>(100);

      objects.Add(new ViewCloudReference(cloud, new List<string>()));
      objects.AddRange(content.Select(x => new ContextReferences(x, new List<string>() { })).ToList());
      objects.Add(new Layouts(new List<ILayout>
        {new LayoutCube()}));
      return new ViewStudy(objects, name);
    }

    [Obsolete("Use ViewStudy() instead")]
    public static IViewStudy Study(string name = "test", bool hasResults = true)
    {
      List<IViewObject> objects = new();
      List<Context> content = new();

      foreach(ViewContextType type in Enum.GetValues(typeof(ViewContextType)))
      {
        content.Add(Content(type, $"test-{type}"));
      }

      var cloud = Cloud<Cloud>(100);

      objects.Add(new ViewCloudReference(cloud, new List<string>()));
      objects.AddRange(content.Select(x => new ContextReferences(x, new List<string>() { })).ToList());
      objects.Add(new Layouts(new List<ILayout>
        {new LayoutCube()}));
      return new ViewStudy(objects, name);
    }

    public static Context Content(ViewContextType type, string name = "test")
    {
      return new(type, SasakiTools.InitGuid, name);
    }

    public static TCloud Cloud<TCloud>(int pointCount)
      where TCloud : ICloud
    {
      var obj = Activator.CreateInstance<TCloud>();
      obj.Points = CloudPoints(pointCount);
      return obj;
    }

    public static TCloud ResultCloud<TCloud>(int pointCount, int colorCount)
      where TCloud : IResultCloud<IResultLayer>
    {
      var obj = Activator.CreateInstance<TCloud>();
      obj.Points = CloudPoints(pointCount);
      obj.layers = Results(pointCount, colorCount);
      return obj;
    }

    public static List<IResultLayer> Results(int pointCount, int colorCount)
    {
      var values = new List<IResultLayer>();
      var random = new Random();
      var targets = new List<IContextInfo>();

      // setup the targets 
      for(var c = 0; c<colorCount; c++) targets.Add(new ContextInfo(SasakiTools.InitGuid, c.ToString()));

      // setup other data items
      var existing = new Context(ViewContextType.Existing, SasakiTools.InitGuid);
      var proposed = new Context(ViewContextType.Existing, SasakiTools.InitGuid);

      // setup data for each target
      foreach(var target in targets)
      {
        var potentialOption = new ResultCondition(target, target, ViewContextType.Potential);
        var existingOption = new ResultCondition(target, existing, ViewContextType.Existing);
        var proposedOption = new ResultCondition(target, proposed, ViewContextType.Proposed);

        values.Add(new ViewResultLayer(Values(pointCount, random), potentialOption));
        values.Add(new ViewResultLayer(Values(pointCount, random), existingOption));
        values.Add(new ViewResultLayer(Values(pointCount, random), proposedOption));
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

    public static void Similar(this IResultLayer dataA, IResultLayer dataB)
    {
      Assert.IsTrue(dataA != default(object) && dataB != default(object));
      Assert.IsTrue(dataA.count.Equals(dataB.count));
      Assert.IsTrue(dataA.values.Count == dataB.values.Count);
      Assert.IsTrue(dataA.info.stage.Equals(dataB.info.stage)
                    && dataA.info.target.guid.Equals(dataB.info.target.guid)
                    && dataA.info.content.guid.Equals(dataB.info.content.guid)
      );
    }
    


  }

}
