using System.Collections.Generic;
using ViewObjects.Cloud;
using ViewObjects.Viewer;

namespace ViewObjects
{

	public interface IViewObj
	{ }

	public interface IViewCloud : IViewObj, IId
	{
		CloudPoint[] points { get; set; }

		int count { get; }
	}

	public interface IResultCloud : IViewCloud
	{
		List<IResultData> data { get; set; }
	}

	public interface IViewer
	{
		ViewerDirection Direction { get; }
	}

	public interface IViewerLayout : IViewObj
	{
		List<IViewer> viewers { get; }
	}

	public interface IViewerBundle : IViewObj
	{
		List<IViewerLayout> layouts { get; set; }
	}

	public interface IViewerBundleLinked : IViewerBundle
	{
		List<CloudShell> linkedClouds { get; set; }
	}

	public interface IViewStudy : IViewObj, INameable, IValidate
	{
		List<IViewObj> objs { get; set; }
	}

	public interface IViewContentBundle : IViewObj
	{
		List<IViewContent> contents { get; set; }
	}

	public interface IViewContent : IViewObj, INameable
	{
		ViewColor viewColor { get; set; }

		List<object> objects { get; set; }
	}

	public interface IBlockerContent : IViewContent
	{ }

	public interface IDesignContent : IViewContent
	{ }

	public interface ITargetContent : IViewContent
	{
		bool isolate { get; set; }

		List<IViewerBundle> bundles { get; set; }
	}

	public interface IRigParam
	{
		List<IViewerBundle> bundles { get; set; }
	}

	public interface IRig : IViewObj
	{
		List<IRigParam> globalParams { get; set; }

		List<ViewColor> globalColors { get; set; }

		Dictionary<string, CloudPoint[]> clouds { get; set; }
	}
}