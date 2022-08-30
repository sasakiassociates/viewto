using System;
using System.Collections.Generic;

namespace ViewObjects.Explorer
{

	[Serializable]
	public struct ContentOption
	{

		public string target;
		public ResultStage stage;
	}

	public interface IExploreView
	{
		public int point { get; }
	}

	public interface IExploreContent
	{
		public List<ContentOption> options { get; set; }
	}

	public interface IExploreRange
	{

		public double min { get; }

		public double max { get; }

		public System.Drawing.Color[] colorRamp { get; }

		public System.Drawing.Color invalidColor { get; }

	}

	public struct ExplorerSettings : IExploreContent, IExploreRange, IExploreView, IValidate
	{
		public double min { get; set; }

		public double max { get; set; }

		public bool showAll { get; set; }

		public int point { get; set; }

		public System.Drawing.Color[] colorRamp { get; set; }

		public System.Drawing.Color invalidColor { get; set; }

		public List<ContentOption> options { get; set; }

		public bool isValid => options.Valid();

		public System.Drawing.Color GetColor(double t) => colorRamp[(int)Math.Round((colorRamp.Length - 1.0) * t, 0)];

	}

}