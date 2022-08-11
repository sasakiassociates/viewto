using System;
using System.Collections.Generic;
using ViewObjects.Viewer;

namespace ViewObjects
{

	public class ResultDataV2
	{
		public ResultDataV2(string content, string stage, string meta, string layout, List<double> values)
		{
			this.content = content;
			this.stage = stage;
			this.meta = meta;
			this.layout = layout;
			this.values = values;
		}

		string content { get; }

		string stage { get; }

		string meta { get; }

		string layout { get; }

		List<double> values { get; }

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
		public CloudPoint[] points { get; set; }
	}

	public interface IResultCloudV2 : IViewCloudV2
	{
		public List<IResultData> data { get; set; }
	}

	public interface IViewStudyV2 : IViewObj, INameable
	{
		public List<IViewObj> objs { get; set; }
	}

	/// <summary>
	/// Grouping object for bundling the objects
	/// </summary>
	public interface IViewContents : IViewObj
	{
		public List<IViewContentV2> contents { get; }
	}

	/// <summary>
	/// basic view content type object
	/// </summary>
	public interface IViewContentV2 : IViewObj, INameable, IId
	{
		public List<object> objects { get; }

		public string[] references { get; }

		public ContentType type { get; }
	}

}