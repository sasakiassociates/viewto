using UnityEngine;
using ViewObjects.Unity;

namespace ViewObjects.Converter.Unity
{
	public class ViewObjectUnitySchema : IViewObjSchema
	{
		public IViewStudy nativeViewStudy
		{
			get => Create<ViewStudyMono>();
		}

		public IViewCloud nativeViewCloud
		{
			get => Create<ViewCloudMono>();
		}

		public IResultCloud nativeResultCloud
		{
			get => Create<ResultCloudMono>();
		}

		public IViewerLayout nativeViewerLayout
		{
			get => Create<ViewerLayoutMono>();
		}

		public IViewerBundle nativeViewerBundle
		{
			get => Create<ViewerBundleMono>();
		}

		public IViewerBundleLinked nativeViewerBundleLinked
		{
			get => Create<ViewerBundleLinkedMono>();
		}

		public IViewContentBundle nativeContentBundle
		{
			get => Create<ContentBundleMono>();
		}

		public ITargetContent nativeTargetContent
		{
			get => Create<TargetContentMono>();
		}

		public IBlockerContent nativeBlockerContent
		{
			get => Create<BlockerContentMono>();
		}

		public IDesignContent nativeDesignContent
		{
			get => Create<DesignContentMono>();
		}

		TObj Create<TObj>() where TObj : MonoBehaviour
		{
			var obj = new GameObject().AddComponent<TObj>();
			obj.name = obj.TypeName();
			return obj;
		}
	}
}