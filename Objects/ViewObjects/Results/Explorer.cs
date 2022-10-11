using System.Collections.Generic;
using System.Linq;

namespace ViewObjects.Explorer
{
	public class Explorer : IExplorer
	{

		/// <summary>
		/// </summary>
		public Explorer()
		{ }

		/// <inheritdoc />
		public IViewStudy Source { get; internal set; }

		/// <inheritdoc />
		public IResultCloud Cloud { get; internal set; }

		/// <inheritdoc />
		public ExplorerSettings Settings { get; set; } = new();

		/// <inheritdoc />
		public ContentInfo ActiveContent { get; set; }

		// /// <inheritdoc />
		public List<ContentOption> Options { get; internal set; }

		/// <inheritdoc />
		public List<IResultCloudData> Data
		{
			get => Cloud?.Data ?? new List<IResultCloudData>();
		}

		/// <inheritdoc />
		public void Load(IViewStudy viewObj)
		{
			if (viewObj == default || !viewObj.CanExplore())
			{
				return;
			}

			Source = viewObj;
			Cloud = viewObj.FindObject<ResultCloud>();

			if (Cloud == null)
			{
				return;
			}

			Settings ??= new ExplorerSettings();

			Options = Cloud.Data.Where(x => x != null).Select(x => x.Option).Cast<ContentOption>().ToList();
			var opt = Options.FirstOrDefault();
			ActiveContent = new ContentInfo(opt);
		}

		public bool IsValid
		{
			get => Source != null && Cloud != null && ActiveContent != null;
		}
	}

	public interface IExplorer : IValidate
	{
		/// <summary>
		///   The active view study being used with the source cloud.
		/// </summary>
		public IViewStudy Source { get; }

		/// <summary>
		///   The heart and soul of the data being explored
		/// </summary>
		public IResultCloud Cloud { get; }

		/// <summary>
		///   Set of data settings for the explorer to use
		/// </summary>
		public ExplorerSettings Settings { get; set; }

		/// <summary>
		///   Container for result values being explored
		/// </summary>
		public List<IResultCloudData> Data { get; }

		// /// <summary>
		// ///   List of options to use for fetching values from <see cref="IExplorer" />. Multiple options will combine the values
		// /// </summary>
		// public List<ContentOption> Options { get; }

		/// <summary>
		/// </summary>
		public ContentInfo ActiveContent { get; set; }

		/// <summary>
		///   Load in a new view study for the explorer to explore!
		/// </summary>
		/// <param name="viewObj">The view study to load in</param>
		public void Load(IViewStudy viewObj);

	}

	public struct ExplorerData
	{
		public double[] ActiveValues { get; set; }

	}

}