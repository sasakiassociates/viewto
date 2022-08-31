using System;
using System.Collections.Generic;

namespace ViewObjects.Explorer
{

	public interface IExploreView
	{
		public int point { get; }
	}

	public interface IExploreContent
	{
		public ExplorerValueType valueType { get; }

		public List<string> targets { get; }

		public List<ContentOption> options { get; }
	}

	public interface IExploreRange
	{

		public double min { get; }

		public double max { get; }

		public bool normalize { get; }

		public System.Drawing.Color[] colorRamp { get; }

		public System.Drawing.Color invalidColor { get; }

	}

	public struct ExplorerSettings : IExploreContent, IExploreRange, IExploreView, IValidate
	{
		/// <summary>
		/// Min value from <see cref="IExplorer"/> active values
		/// </summary>
		public double min { get; set; }

		/// <summary>
		/// Max value from <see cref="IExplorer"/> active values
		/// </summary>
		public double max { get; set; }

		/// <summary>
		/// When set to true will normalize the active values
		/// </summary>
		public bool normalize { get; set; }

		/// <summary>
		/// When set to true will output all points and values for <see cref="options"/>
		/// </summary>
		public bool showAll { get; set; }

		/// <summary>
		/// Active index of a point being explored
		/// </summary>
		public int point { get; set; }

		/// <summary>
		/// Gradient ramp for visualizing the value of point 
		/// </summary>
		public System.Drawing.Color[] colorRamp { get; set; }

		/// <summary>
		/// Color for any point with no value in cloud
		/// </summary>
		public System.Drawing.Color invalidColor { get; set; }

		/// <summary>
		/// List of options to use for fetching values from <see cref="IExplorer"/>. Multiple options will combine the values
		/// </summary>
		public List<ContentOption> options { get; set; }

		/// <summary>
		/// Comparable value type to use when exploring values
		/// </summary>
		public ExplorerValueType valueType { get; set; }

		/// <summary>
		/// Returns true if <see cref="options"/> is valid
		/// </summary>
		public bool isValid => options.Valid();

		/// <summary>
		/// Get a color along the gradient ramp
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public System.Drawing.Color GetColor(double t) => colorRamp[(int)Math.Round((colorRamp.Length - 1.0) * t, 0)];

		public List<string> targets { get; set; }
	}

}