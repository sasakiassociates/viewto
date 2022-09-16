using System;
using System.Collections.Generic;
using ViewObjects.Viewer;

namespace ViewObjects
{

	public class ResultDataV2
	{
		public ResultDataV2(string content, string stage, string layout, List<int> values)
		{
			this.content = content;
			this.stage = stage;
			this.layout = layout;
			this.values = values;
		}

		string content { get; }

		string stage { get; }

		string layout { get; }

		List<int> values { get; }

	}

	[Serializable]
	public class RigParamArgsV2
	{
		public RigParamArgsV2(List<IViewerBundle> bundles, List<ViewColor> colors)
		{
			this.bundles = bundles;
			this.colors = colors;
		}

		List<IViewerBundle> bundles { get; }

		List<ViewColor> colors { get; }
	}

	public interface IRigV2 : IViewObj
	{
		public void Load(List<RigParamArgsV2> args);
	}

	public interface IViewerLayoutV2 : IViewObj
	{
		public List<ViewerDirection> viewers { get; }
	}

	public interface IViewerSystem : IViewObj
	{
		public List<IViewerLayout> layouts { get; set; }

		/// <summary>
		/// A list of cloud ids that can be used with this bundle
		/// </summary>
		public List<string> cloudIds { get; set; }
	}

	public interface IViewCloudV2 : IViewObj, IId
	{
		/// <summary>
		/// The cloud of points to use
		/// </summary>
		public CloudPoint[] points { get; set; }

		/// <summary>
		/// The view analysis data gathered
		/// </summary>
		public List<IResultData> data { get; set; }
	}

	public interface IViewStudyV2 : IViewObj, INameable
	{
		public List<IViewObj> objs { get; set; }
	}

	/// <summary>
	/// Grouping object for bundling the objects
	/// </summary>
	public interface IContents : IViewObj
	{
		public List<IContent> contents { get; }
	}

	/// <summary>
	/// basic view content type object
	/// </summary>
	public interface IContent : IViewObj, INameable, IId
	{
		/// <summary>
		/// A list of references to be used for this view content
		/// </summary>
		public List<string> references { get; }

		/// <summary>
		/// The style of view content
		/// </summary>
		public ContentType type { get; }

	}

}