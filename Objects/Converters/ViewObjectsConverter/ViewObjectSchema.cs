using System;
using ViewObjects.Cloud;
using ViewObjects.Content;
using ViewObjects.Study;
using ViewObjects.Viewer;

namespace ViewObjects.Converter.Script
{

	public interface IViewObjectBuilder : IViewerSchema, IViewObjSchema, IViewContentSchema
	{
		TObj Create<TObj>();
	}

	public class ViewObjectSchema : IViewObjSchema, IViewerSchema, IViewContentSchema
	{
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

		public virtual IViewerLayout nativeViewerLayout
		{
			get => Create<ViewerLayout>();
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

		private TObj Create<TObj>() where TObj : IViewObj => Activator.CreateInstance<TObj>();
	}

	public interface IViewObjSchema
	{
		IViewStudy nativeViewStudy { get; }

		IViewCloud nativeViewCloud { get; }

		IResultCloud nativeResultCloud { get; }
	}

	public interface IViewerSchema
	{
		IViewerLayout nativeViewerLayout { get; }

		IViewerBundle nativeViewerBundle { get; }

		IViewerBundleLinked nativeViewerBundleLinked { get; }
	}

	public interface IViewContentSchema
	{
		IViewContentBundle nativeContentBundle { get; }

		ITargetContent nativeTargetContent { get; }

		IBlockerContent nativeBlockerContent { get; }

		IDesignContent nativeDesignContent { get; }
	}

}