using System;
using System.Collections.Generic;
using System.Drawing;
using ViewObjects;
using ViewObjects.Content;

namespace ViewTo
{
  public static partial class Mil
  {
    public static partial class Fabricate
    {
      public static class Data
      {
        public const string TAR_A = "TargetA";
        public const string TAR_B = "TargetB";
        public const string OPT_A = "DesignA";
        public const string OPT_B = "DesignB";

        public static List<TargetContent> GroupContentTargets =>
          new List<TargetContent>
          {
            new TargetContent
            {
              viewName = TAR_A
            },
            new TargetContent
            {
              viewName = TAR_B
            }
          };

        public static List<BlockerContent> GroupContentBlockers =>
          new List<BlockerContent>
          {
            new BlockerContent(), new BlockerContent(), new BlockerContent()
          };

        public static List<DesignContent> GroupContentDesigns =>
          new List<DesignContent>
          {
            new DesignContent
            {
              viewName = "DesignA"
            }


          };

        // public static ResultPixelBundle ResultPixels(int valueCount)
        // {
        //   return new ResultPixelBundle()
        //   {
        //     Group = PixelCollection(valueCount)
        //   };
        // }
        public static List<IResultData> PixelCollection(int valueCount)
        {

          return new List<IResultData>
          {
            // Potential 
            PixelData(valueCount, TAR_A, "Target"),
            PixelData(valueCount, TAR_B, "Target"),
            // Existing 
            PixelData(valueCount, TAR_A, "Blocker"),
            PixelData(valueCount, TAR_B, "Blocker"),
            // Options 
            PixelData(valueCount, TAR_A, "Design", OPT_A),
            PixelData(valueCount, TAR_B, "Design", OPT_A),
            PixelData(valueCount, TAR_A, "Design", OPT_B),
            PixelData(valueCount, TAR_B, "Design", OPT_B)
          };
        }

        public static ContentResultData PixelData(int value, string contentName, string stage, string meta = null)
        {
          return new ContentResultData(ValuesDouble(value), stage, contentName, Color.Aqua.ToArgb(), meta);
        }

        public static List<double> ValuesDouble(int valueCount, Random rnd = null)
        {
          rnd ??= new Random();
          var values = new List<double>();

          for (var j = 0; j < valueCount; j++)
            values.Add(rnd.NextDouble());
          return values;
        }

        public static uint[] Values(int valueCount, Random rnd = null)
        {
          rnd ??= new Random();

          var values = new uint[valueCount];
          for (var j = 0; j < values.Length; j++)
          {
            var bytes = new byte[4];
            rnd.NextBytes(bytes);
            values[j] = BitConverter.ToUInt32(bytes, 0);
          }
          return values;
        }
        public static uint[][] ValueMD(int valueCount, int contentCount)
        {
          var rnd = new Random();
          var testValues = new uint[valueCount][];
          for (uint i = 0; i < testValues.Length; i++) testValues[i] = Values(contentCount, rnd);
          return testValues;
        }

        public static CloudPoint[] CloudPoints(int count)
        {
          var rnd = new Random();
          var points = new CloudPoint[count];
          for (var i = 0; i < points.Length; i++)
            points[i] = new CloudPoint(rnd.Next(), rnd.Next(), rnd.Next(),
                                       rnd.Next(), rnd.Next(), rnd.Next(), "1234-567-890");
          return points;
        }
      }
    }
  }
}
