using System;
using System.Collections.Generic;
using System.Drawing;

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

		public List<IContentOption> options { get; }
	}

	public interface IExploreRange
	{

		public double min { get; }

		public double max { get; }

		public bool normalize { get; }

		public Color[] colorRamp { get; }

		public Color invalidColor { get; }
	}

	public class ExplorerSettings : IExploreContent, IExploreRange, IExploreView, IValidate
	{

		public ExplorerSettings()
		{ }

		/// <summary>
		///   When set to true will output all points and values for <see cref="options" />
		/// </summary>
		public bool showAll { get; set; } = true;

		/// <summary>
		///   List of options to use for fetching values from <see cref="IExplorer" />. Multiple options will combine the values
		/// </summary>
		public List<IContentOption> options { get; set; } = new();

		/// <summary>
		///   Comparable value type to use when exploring values
		/// </summary>
		public ExplorerValueType valueType { get; set; } = ExplorerValueType.ExistingOverPotential;

		public List<string> targets { get; set; } = new();

		/// <summary>
		///   Min value from <see cref="IExplorer" /> active values
		/// </summary>
		public double min { get; set; } = 0.0;

		/// <summary>
		///   Max value from <see cref="IExplorer" /> active values
		/// </summary>
		public double max { get; set; } = 1.0;

		/// <summary>
		///   When set to true will normalize the active values
		/// </summary>
		public bool normalize { get; set; } = false;

		/// <summary>
		///   Gradient ramp for visualizing the value of point
		/// </summary>
		public Color[] colorRamp { get; set; } = ViewColor.Ramp();

		/// <summary>
		///   Color for any point with no value in cloud
		/// </summary>
		public Color invalidColor { get; set; } = Color.Black;

		/// <summary>
		///   Active index of a point being explored
		/// </summary>
		public int point { get; set; } = 0;

		/// <summary>
		///   Returns true if <see cref="options" /> is valid
		/// </summary>
		public bool IsValid
		{
			get => options.Valid();
		}

		/// <summary>
		///   Get a color along the gradient ramp
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public Color GetColor(double t) => colorRamp[(int)Math.Round((colorRamp.Length - 1.0) * Clamp(t, 0.0, 1.0), 0)];

		static T Clamp<T>(T val, T min, T max) where T : IComparable<T>
		{
			if (val.CompareTo(min) < 0) return min;

			return val.CompareTo(max) > 0 ? max : val;
		}
	}

}