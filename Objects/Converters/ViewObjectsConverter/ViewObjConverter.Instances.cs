
namespace ViewObjects.Converter.Script
{

	public partial class ViewObjectConverter
	{
		protected IViewStudy CreateNativeViewStudy() => Schema?.nativeViewStudy;

		protected IViewerLayout CreateNativeViewerLayout() => Schema?.nativeViewerLayout;

		protected IViewerBundle CreateNativeViewerBundle() => Schema?.nativeViewerBundle;

		protected IViewerBundleLinked CreateNativeViewerBundleLinked() => Schema?.nativeViewerBundleLinked;

		#region View Content
		protected IDesignContent CreateNativeDesignContent() => Schema?.nativeDesignContent;

		protected IBlockerContent CreateNativeBlockerContent() => Schema?.nativeBlockerContent;

		protected ITargetContent CreateNativeTargetContent() => Schema?.nativeTargetContent;

		protected IViewContentBundle CreateNativeViewContentBundle() => Schema?.nativeContentBundle;
		#endregion

		#region Clouds
		protected IViewCloud CreateNativeViewCloud() => Schema?.nativeViewCloud;

		protected IResultCloud CreateNativeResultCloud() => Schema?.nativeResultCloud;
		#endregion

	}
}