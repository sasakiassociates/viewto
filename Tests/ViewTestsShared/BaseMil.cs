using System;
using System.Collections.Generic;
using System.Drawing;
using Objects.Geometry;
using Speckle.Core.Models;
using ViewObjects;
using ViewObjects.Speckle;

namespace ViewTo.Tests.Integration
{
	public static class BaseMil
	{

		public const string TAR_A = "TargetA";
		public const string TAR_B = "TargetB";
		public const string OPT_A = "DesignA";
		public const string OPT_B = "DesignB";

		public static List<BlockerContentBaseV1> Blockers()
		{
			return new List<BlockerContentBaseV1>
			{
				new()
				{
					objects = new List<Base>
					{
						new Box()
					}
				},
				new()
				{
					objects = new List<Base>
					{
						new Box()
					}
				}
			};
		}

		public static List<DesignContentBaseV1> Designs()
		{
			return new List<DesignContentBaseV1>
			{
				new()
				{
					ViewName = "Target 1",
					objects = new List<Base>
					{
						new Box()
					}
				},
				new()
				{
					ViewName = "Target 2",
					objects = new List<Base>
					{
						new Box()
					}
				}
			};
		}

		public static List<TargetContentBaseV1> Targets()
		{
			return new List<TargetContentBaseV1>
			{
				new()
				{
					ViewName = "Target 1",
					objects = new List<Base>
					{
						new Box()
					}
				},
				new()
				{
					ViewName = "Target 2",
					objects = new List<Base>
					{
						new Box()
					}
				}
			};
		}

		public static ContentBundleBaseV1 Content()
		{
			return new ContentBundleBaseV1
			{
				targets = Targets(), blockers = Blockers(), designs = Designs()
			};
		}

		public static ViewCloudBaseV1 ViewCloud(int count)
		{
			return new ViewCloudBaseV1
			{
				points = CloudPoints(count)
			};
		}

		public static List<CloudPointBase> CloudPoints(int count)
		{
			var rnd = new Random();
			var points = new List<CloudPointBase>();
			for (var i = 0; i < count; i++)
				points.Add(new CloudPointBase
				{
					x = rnd.NextDouble(), y = rnd.NextDouble(), z = rnd.NextDouble(), meta = "FunSpace"
				});

			return points;
		}

		public static ViewerBundleBaseV1 ViewerBundle()
		{
			return new ViewerBundleBaseV1
			{
				layouts = new List<IViewerLayout>
				{
					new ViewerLayoutBaseV1Ortho(), new ViewerLayoutBaseV1Cube()
				}
			};
		}

		public static ResultCloudBaseV1 ResultCloud(int count)
		{
			return new ResultCloudBaseV1
			{
				points = CloudPoints(count), data = PixelCollection(count)
			};
		}

		public static ResultPixelBaseV1 PixelData(int value, string contentName, string stage, string meta = null)
		{
			return new ResultPixelBaseV1
			{
				values = ValuesInt(value),
				stage = stage,
				content = contentName,
				color = Color.Aqua.ToArgb(),
				meta = meta
			};
		}

		public static List<uint> ValuesUint(int valueCount, Random rnd = null)
		{
			rnd ??= new Random();
			var values = new List<uint>();

			for (var j = 0; j < valueCount; j++)
				values.Add((uint)rnd.Next());
			return values;
		}

		public static List<int> ValuesInt(int valueCount, Random rnd = null)
		{
			rnd ??= new Random();
			var values = new List<int>();

			for (var j = 0; j < valueCount; j++)
				values.Add((int)rnd.Next());
			return values;
		}

		public static List<double> ValuesDouble(int valueCount, Random rnd = null)
		{
			rnd ??= new Random();
			var values = new List<double>();

			for (var j = 0; j < valueCount; j++)
				values.Add(rnd.NextDouble());
			return values;
		}

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
	}
}