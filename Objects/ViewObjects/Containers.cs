using System;
using System.Collections.Generic;

namespace ViewObjects
{
	public abstract class Container
	{ }

	[Serializable]
	public class ContentOption : Container, IContentOption
	{

		/// <inheritdoc />
		public string Id { get; set; }

		/// <inheritdoc />
		public string Name { get; set; }

		/// <inheritdoc />
		public ResultStage Stage { get; set; }

		public bool Equals(IContentOption obj) => obj != default && Id.Valid() && Id.Equals(obj.Id) && Stage == obj.Stage;
	}

	[Serializable]
	public class ResultCloudData : Container, IResultCloudData
	{

		/// <summary>
		/// </summary>
		public ResultCloudData()
		{ }

		public ResultCloudData(List<int> values, IContentOption option, string layout)
		{
			Values = values;
			Option = option;
			Layout = layout;
		}

		/// <inheritdoc />
		public IContentOption Option { get; set; }

		/// <inheritdoc />
		public string Layout { get; set; }

		/// <inheritdoc />
		public List<int> Values { get; set; }
	}

	[Serializable]
	public class RigParameters : Container
	{
		public RigParameters(List<string> clouds, List<ViewColor> colors, List<IViewerLayout> viewer)
		{
			Clouds = clouds;
			Colors = colors;
			Viewer = viewer;
		}

		public List<IViewerLayout> Viewer { get; set; }

		/// <summary>
		///   The lists of <see cref="IViewCloud" /> by <see cref="IViewCloud.ViewId" /> associated with the args
		/// </summary>
		public List<string> Clouds { get; }

		/// <summary>
		///   List of colors to use for run time analysis
		/// </summary>
		public List<ViewColor> Colors { get; }
	}

}