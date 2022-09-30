using System;
using ViewObjects.Cloud;
using ViewObjects.Study;
using ViewObjects.Viewer;

namespace ViewObjects.Converter
{

	public class ViewObjectFactory : IViewObjectFactory
	{

		public virtual IViewerLayout_v1 NativeViewerLayoutV1
		{
			get => Create<ViewerLayoutV1>();
		}

		public virtual IViewContentBundle_v1 NativeContentBundleV1
		{
			get => Create<ContentBundleV1>();
		}

		public virtual ITargetContentV1 NativeTargetContentV1
		{
			get => Create<TargetContentV1>();
		}

		public virtual IBlockerContentV1 NativeBlockerContentV1
		{
			get => Create<BlockerContentV1>();
		}

		public virtual IDesignContentV1 NativeDesignContentV1
		{
			get => Create<DesignContentV1>();
		}

		public virtual IViewerBundle_v1 NativeViewerBundleV1
		{
			get => Create<ViewerBundleV1>();
		}

		public virtual IViewerBundleLinked_v1 NativeViewerBundleLinkedV1
		{
			get => Create<ViewerBundleLinkedV1>();
		}

		public virtual IViewStudy_v1 nativeViewStudy
		{
			get => Create<ViewStudy_v1>();
		}

		public virtual IViewCloud_v1 NativeViewCloudV1
		{
			get => Create<ViewCloudV1V1>();
		}

		public virtual IResultCloudV1 NativeResultCloudV1
		{
			get => Create<ResultCloudV1V1>();
		}

		TObj Create<TObj>() where TObj : IViewObject => Activator.CreateInstance<TObj>();
	}

	public interface IViewObjectFactory
	{
		IViewStudy_v1 nativeViewStudy { get; }

		IViewCloud_v1 NativeViewCloudV1 { get; }

		IResultCloudV1 NativeResultCloudV1 { get; }

		IViewerBundle_v1 NativeViewerBundleV1 { get; }

		IViewerBundleLinked_v1 NativeViewerBundleLinkedV1 { get; }

		IViewContentBundle_v1 NativeContentBundleV1 { get; }

		ITargetContentV1 NativeTargetContentV1 { get; }

		IBlockerContentV1 NativeBlockerContentV1 { get; }

		IDesignContentV1 NativeDesignContentV1 { get; }
	}

}