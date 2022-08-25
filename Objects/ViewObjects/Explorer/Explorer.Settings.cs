using System;

namespace ViewObjects.Explorer
{

	public interface IExploreView
	{
		public int point { get; }
	}

	public interface IExploreContent
	{
		public string target { get; }

		public ResultType type { get; }
	}

	public interface IExploreRange
	{
		public double log { get; }

		public double min { get; }

		public double max { get; }

		public System.Drawing.Color[] colorRamp { get; }

		public System.Drawing.Color invalidColor { get; }

	}

	public struct ExplorerSettings : IExploreContent, IExploreRange, IExploreView
	{

		public double min { get; set; }

		public double max { get; set; }

		public double log { get; set; }

		public bool showAll { get; set; }

		public string target { get; set; }

		public int point { get; set; }

		public ResultType type { get; set; }

		public System.Drawing.Color[] colorRamp { get; set; }

		public System.Drawing.Color invalidColor { get; set; }

		public System.Drawing.Color GetColor(double t) => colorRamp[(int)Math.Round((colorRamp.Length - 1.0) * t, 0)];

	}

}