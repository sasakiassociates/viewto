using System;
using System.Collections.Generic;
using System.Linq;
using ViewObjects;
using ViewTo.Events.Args;
using ViewTo.Events.Prime;

namespace ViewTo.Commands
{
  internal class CloudToCsvCommand : ICommandWithResult<CsvDataArgs>
  {

    public CloudToCsvCommand(IResultCloud cloud)
    {
      this.cloud = cloud;
    }

    public CsvDataArgs args { get; private set; }

    protected IResultCloud cloud;

    protected IEnumerable<string> VectorHeader =>
      new[]
      {
        "x", "y", "z", "xn", "yn", "zn", "meta"
      };



    protected static string Join(IEnumerable<string> values)
    {
      return string.Join(",", values);
    }

    protected readonly struct Columns
    {

      public const int X = 0;
      public const int Y = X + 1;
      public const int Z = Y + 1;
      public const int XNormal = Z + 1;
      public const int YNormal = XNormal + 1;
      public const int ZNormal = YNormal + 1;
      public const int FirstResult = ZNormal + 1;

    }

    public static string PointToLine(CloudPoint p)
    {
      return Join(
        new[]
        {
          p.x.ToString(), p.y.ToString(), p.z.ToString(), p.xn.ToString(), p.yn.ToString(), p.zn.ToString(), p.meta
        });
    }

    public static CloudPoint LineToPoint(string lineData)
    {
      var row = lineData.Split(',');
      float.TryParse(row[Columns.X], out var x);
      float.TryParse(row[Columns.Y], out var y);
      float.TryParse(row[Columns.Z], out var z);
      float.TryParse(row[Columns.XNormal], out var xn);
      float.TryParse(row[Columns.YNormal], out var yn);
      float.TryParse(row[Columns.ZNormal], out var zn);
      return new CloudPoint(x, y, z, xn, yn, zn);
    }

    protected readonly struct Rows
    {

      public const int NameId = 0;
      public const int Header = NameId + 1;
      public const int FirstPoint = Header + 1;
      public const int NonPointCount = 2;
    }

    public void Run()
    {
      if (cloud == null || !cloud.data.Valid() || !cloud.points.Valid())
      {
        args = new CsvDataArgs(null);
        return;
      }

      var lines = new List<string>();

      // add content types to header
      var headers = VectorHeader.ToList();
      for (var i = 0; i < cloud.data.Count; i++) 
        headers.Add(cloud.data[i].stage + "-" + cloud.data[i].content + (i == cloud.data.Count - 1 ? "," : ""));

      lines.Add(Join(headers));

      for (var pointIndex = 0; pointIndex < cloud.points.Length; pointIndex++)
      {
        var line = new List<string>
        {
          PointToLine(cloud.points[pointIndex])
        };

        foreach (var data in cloud.data)
          line.Add(data.values[pointIndex].ToString());

        lines.Add(Join(line));
      }

      args = new CsvDataArgs(data: string.Join(Environment.NewLine, lines));
    }

  }
}
