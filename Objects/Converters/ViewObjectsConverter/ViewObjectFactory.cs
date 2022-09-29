using System;
using ViewObjects.Cloud;
using ViewObjects.Content;
using ViewObjects.Study;
using ViewObjects.Viewer;

namespace ViewObjects.Converter
{

	public class ViewObjectFactory : IViewObjectFactory
	{

		public virtual IViewerLayout nativeViewerLayout
		{
			get => Create<ViewerLayout>();
		}

		public virtual IViewContentBundle nativeContentBundle
		{
			get => Create<ContentBundle>();
		}

		public virtual ITargetContent nativeTargetContent
		{
			get => Create<TargetContent>();
		}

		public virtual IBlockerContent nativeBlockerContent
		{
			get => Create<BlockerContent>();
		}

		public virtual IDesignContent nativeDesignContent
		{
			get => Create<DesignContent>();
		}

		public virtual IViewerBundle nativeViewerBundle
		{
			get => Create<ViewerBundle>();
		}

		public virtual IViewerBundleLinked nativeViewerBundleLinked
		{
			get => Create<ViewerBundleLinked>();
		}

		public virtual IViewStudy nativeViewStudy
		{
			get => Create<ViewStudy>();
		}

		public virtual IViewCloud nativeViewCloud
		{
			get => Create<ViewCloud>();
		}

		public virtual IResultCloud nativeResultCloud
		{
			get => Create<ResultCloud>();
		}

		TObj Create<TObj>() where TObj : IViewObj => Activator.CreateInstance<TObj>();
	}

	public interface IViewObjectFactory
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