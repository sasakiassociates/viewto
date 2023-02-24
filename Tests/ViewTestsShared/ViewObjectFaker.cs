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

    public static IViewStudy Study(string name = "test", bool hasResults = true)
    {
      List<IViewObject> objects = new();
      List<Content> content = new();

      foreach(ViewContentType type in Enum.GetValues(typeof(ViewContentType)))
      {
        content.Add(Content(type, $"test-{type}"));
      }

      var cloud = Cloud<ViewCloud>(100);

      if(hasResults)
      {
        List<IResultCloudData> data = new();

        foreach(var c in content)
        {
          if(c.type == ViewContentType.Potential)
          {
            foreach(ViewContentType type in Enum.GetValues(typeof(ViewContentType)))
            {
              data.Add(Result<ResultCloudData>(
                  cloud.Points.Length,
                  type,
                  c.ViewId,
                  nameof(Layout),
                  c.ViewName
                )
              );
            }
          }
        }

        ResultCloud rc = new() {Points = cloud.Points, Data = data};
        objects.Add(rc);
      }

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
      where TCloud : IResultCloud
    {
      var obj = Activator.CreateInstance<TCloud>();
      obj.Points = CloudPoints(pointCount);
      obj.Data = Results<ResultCloudData>(pointCount, colorCount).Cast<IResultCloudData>().ToList();
      return obj;
    }

    public static TCloud ResultCloud<TCloud, TData>(int pointCount, int colorCount)
      where TCloud : IResultCloud<TData> where TData : IResultCloudData
    {
      var obj = Activator.CreateInstance<TCloud>();
      obj.Points = CloudPoints(pointCount);
      obj.Data = Results<TData>(pointCount, colorCount);
      return obj;
    }

    public static List<TData> Results<TData>(int pointCount, int colorCount) where TData : IResultCloudData
    {
      var values = new List<TData>();
      var random = new Random();

      for(var c = 0; c < colorCount; c++)
      {
        var id = ObjUtils.InitGuid;
        foreach(ViewContentType stage in Enum.GetValues(typeof(ViewContentType)))
        {
          values.Add(Result<TData>(pointCount, stage, id, nameof(Layout), $"Test{c}", random));
        }
      }

      return values;
    }

    public static TData Result<TData>(
      int pointCount,
      ViewContentType stage,
      string id = null,
      string layout = null,
      string contentName = null,
      Random random = null
    ) where TData : IResultCloudData
    {
      random ??= new Random();
      var obj = Activator.CreateInstance<TData>();
      // obj.layout = string.IsNullOrEmpty(layout) ? nameof(Layout) : layout;
      // obj.values = Values(pointCount, random);
      // obj.Option = ContentOption(
      //   string.IsNullOrEmpty(contentName) ? "Test" : contentName,
      //   ObjUtils.CheckIfValidId(id),
      //   stage);
      return obj;
    }

    public static IContentOption ContentOption(string name = "test", string id = null, ViewContentType stage = ViewContentType.Existing)
    {
      return null;
      // return new ContentOption
      // {
      // Id = ObjUtils.CheckIfValidId(id), Stage = stage, Name = name
      // };
    }

    public static CloudPoint[] CloudPoints(int count)
    {
      var rnd = new Random();
      var points = new CloudPoint[count];
      for(var i = 0; i < points.Length; i++)
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
      for(var j = 0; j < valueCount; j++)
      {
        var bytes = new byte[4];
        rnd.NextBytes(bytes);
        values.Add((int)BitConverter.ToUInt32(bytes, 0));
      }

      return values;
    }
  }

}
