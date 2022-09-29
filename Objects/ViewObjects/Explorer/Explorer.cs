using ViewObjects.Cloud;

namespace ViewObjects.Explorer
{
	public class Explorer : IExplorer
	{
		/// <inheritdoc />
		public IViewStudy_v2 Study { get; private set; }

		/// <inheritdoc />
		public IResultCloud_v2 Source { get; private set; }

		/// <inheritdoc />
		public ExplorerSettings Settings { get; set; } = new ExplorerSettings();

		/// <inheritdoc />
		public ExplorerData Data { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public Explorer()
		{ }

		/// <inheritdoc />
		public void Load(IResultCloud_v2 viewObj)
		{
			throw new System.NotImplementedException();
		}

		/// <inheritdoc />
		public void Load(IViewStudy_v2 viewObj)
		{
			if (viewObj == default)
				return;

			Source = viewObj.FindObject<ResultCloud_v2>();
			Settings.options = viewObj.GetContentOptions();
			// Data.ActiveValues = this.Fetch();
		}

		/// <inheritdoc />
		public ResultPoint GetResultPoint() => throw new System.NotImplementedException();
	}

	public interface IExplorer
	{
		/// <summary>
		/// The active view study being used with the source cloud. 
		/// </summary>
		public IViewStudy_v2 Study { get; }

		/// <summary>
		/// The heart and soul of the data being explored 
		/// </summary>
		public IResultCloud_v2 Source { get; }

		/// <summary>
		/// Set of data settings for the explorer to use 
		/// </summary>
		public ExplorerSettings Settings { get; set; }

		/// <summary>
		/// Container for result values being explored
		/// </summary>
		public ExplorerData Data { get; }

	}

	public struct ExplorerData
	{
		public double[] ActiveValues { get; set; }

	}

}