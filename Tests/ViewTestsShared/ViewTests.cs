using Speckle.Core.Credentials;
using System;
using System.Collections.Generic;

namespace ViewTo.Tests
{

  public static class ViewTests
  {
    public static void WriteSpeckleURL(StreamWrapperType type, string value, string streamId, string server)
    {
      var path = "";
      switch(type)
      {
        case StreamWrapperType.Stream:
          path = null;
          break;
        case StreamWrapperType.Commit:
          path = "commits";
          break;
        case StreamWrapperType.Branch:
          path = "branches";
          break;
        case StreamWrapperType.Object:
          path = "objects";
          break;
        case StreamWrapperType.Undefined:
          return;
        default:
          throw new ArgumentOutOfRangeException(nameof(type), type, null);
      }
      Console.WriteLine($"{type}-{value}\n" + server + "/streams/" + streamId + (string.IsNullOrEmpty(path) ? "" : $"/{path}/{value}"));
    }

    public const string TAR_A = "TargetA";
    public const string TAR_B = "TargetB";
    public const string OPT_A = "DesignA";
    public const string OPT_B = "DesignB";

    public static List<uint> ValuesUint(int valueCount, Random rnd = null)
    {
      rnd ??= new Random();
      var values = new List<uint>();

      for(var j = 0; j < valueCount; j++)
      {
        values.Add((uint)rnd.Next());
      }

      return values;
    }

    public static List<int> ValuesInt(int valueCount, Random rnd = null)
    {
      rnd ??= new Random();
      var values = new List<int>();

      for(var j = 0; j < valueCount; j++)
      {
        values.Add(rnd.Next());
      }

      return values;
    }

    public static List<double> ValuesDouble(int valueCount, Random rnd = null)
    {
      rnd ??= new Random();
      var values = new List<double>();

      for(var j = 0; j < valueCount; j++)
      {
        values.Add(rnd.NextDouble());
      }

      return values;
    }
  }

}
