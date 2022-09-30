using System.Collections.Generic;
using ViewObjects.Cloud;

namespace ViewObjects
{

	public interface IViewCloud_v1 : IViewObject, IId
	{
		CloudPoint[] points { get; set; }

		int count { get; }
	}

	public interface IResultCloudV1 : IViewCloud_v1
	{
		List<IResultData_v1> data { get; set; }
	}

	public interface IViewer_v1
	{
		ViewerDirection Direction { get; }
	}

	public interface IViewerLayout_v1 : IViewObject
	{
		List<IViewer_v1> viewers { get; }
	}

	public interface IViewerBundle_v1 : IViewObject
	{
		List<IViewerLayout_v1> layouts { get; set; }
	}

	public interface IViewerBundleLinked_v1 : IViewerBundle_v1
	{
		List<CloudShell> linkedClouds { get; set; }
	}

	public interface IViewStudy_v1 : IViewObject, INameable, IValidate
	{
		List<IViewObject> objs { get; set; }
	}

	public interface IViewContentBundle_v1 : IViewObject
	{
		List<IViewContent_v1> contents { get; set; }
	}

	public interface IViewContent_v1 : IViewObject, INameable
	{
		ViewColor viewColor { get; set; }

		List<object> objects { get; set; }
	}

	public interface IBlockerContentV1 : IViewContent_v1
	{ }

	public interface IDesignContentV1 : IViewContent_v1
	{ }

	public interface ITargetContentV1 : IViewContent_v1
	{
		bool isolate { get; set; }

		List<IViewerBundle_v1> bundles { get; set; }
	}

	public interface IRigParam_v1
	{
		List<IViewerBundle_v1> bundles { get; set; }
	}

	public interface IRig_v1 : IViewObject
	{
		List<IRigParam_v1> globalParams { get; set; }

		List<ViewColor> globalColors { get; set; }

		Dictionary<string, CloudPoint[]> clouds { get; set; }
	}
}