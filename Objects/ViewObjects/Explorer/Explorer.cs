using System;
using System.Linq;
using ViewObjects.Cloud;

namespace ViewObjects.Explorer
{
	public class Explorer : IExplorer
	{

		/// <summary>
		/// </summary>
		public Explorer()
		{ }

		/// <inheritdoc />
		public IViewStudy_v2 Study { get; private set; }

		/// <inheritdoc />
		public IResultCloud_v2 Source { get; private set; }

		/// <inheritdoc />
		public ExplorerSettings Settings { get; set; } = new();

		/// <inheritdoc />
		public ExplorerData Data { get; private set; }

		/// <inheritdoc />
		public void Load(IViewStudy_v2 viewObj)
		{
			if (viewObj == default)
				return;

			Source = viewObj.FindObject<ResultCloud_v2>();

			if (Source == null) return;

			Settings ??= new ExplorerSettings();
			Settings.options = Source.Data.Where(x => x != null).Select(x => x.Option).ToList();
			// Data.ActiveValues = this.Fetch();
		}

		/// <inheritdoc />
		public ResultPoint GetResultPoint() => throw new NotImplementedException();
	}

	public interface IExplorer
	{
		/// <summary>
		///   The active view study being used with the source cloud.
		/// </summary>
		public IViewStudy_v2 Study { get; }

		/// <summary>
		///   The heart and soul of the data being explored
		/// </summary>
		public IResultCloud_v2 Source { get; }

		/// <summary>
		///   Set of data settings for the explorer to use
		/// </summary>
		public ExplorerSettings Settings { get; set; }

		/// <summary>
		///   Container for result values being explored
		/// </summary>
		public ExplorerData Data { get; }

		/// <summary>
		///   Load in a new view study for the explorer to explore!
		/// </summary>
		/// <param name="viewObj">The view study to load in</param>
		public void Load(IViewStudy_v2 viewObj);

		/// <summary>
		///   Retrieves the active point result data
		/// </summary>
		/// <returns></returns>
		public ResultPoint GetResultPoint();
	}

	public struct ExplorerData
	{
		public double[] ActiveValues { get; set; }

	}

}