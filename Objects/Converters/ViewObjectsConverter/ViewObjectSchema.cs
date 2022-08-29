using System;
using ViewObjects.Cloud;
using ViewObjects.Content;
using ViewObjects.Study;
using ViewObjects.Viewer;

namespace ViewObjects.Converter
{

	public class ViewObjectSchema : IViewObjSchema
	{
		public virtual IViewContentBundle nativeContentBundle => Create<ContentBundle>();

		public virtual ITargetContent nativeTargetContent => Create<TargetContent>();

		public virtual IBlockerContent nativeBlockerContent => Create<BlockerContent>();

		public virtual IDesignContent nativeDesignContent => Create<DesignContent>();

		public virtual IViewerLayout nativeViewerLayout => Create<ViewerLayout>();

		public virtual IViewerBundle nativeViewerBundle => Create<ViewerBundle>();

		public virtual IViewerBundleLinked nativeViewerBundleLinked => Create<ViewerBundleLinked>();

		public virtual IViewStudy nativeViewStudy => Create<ViewStudy>();

		public virtual IViewCloud nativeViewCloud => Create<ViewCloud>();

		public virtual IResultCloud nativeResultCloud => Create<ResultCloud>();

		TObj Create<TObj>() where TObj : IViewObj => Activator.CreateInstance<TObj>();
	}

	public interface IViewObjSchema
	{
		IViewStudy nativeViewStudy { get; }

		IViewCloud nativeViewCloud { get; }

		IResultCloud nativeResultCloud { get; }

		IViewerBundle nativeViewerBundle { get; }

		IViewerBundleLinked nativeViewerBundleLinked { get; }

		IViewContentBundle nativeContentBundle { get; }

		ITargetContent nativeTargetContent { get; }

		IBlockerContent nativeBlockerContent { get; }

		IDesignContent nativeDesignContent { get; }
	}

}