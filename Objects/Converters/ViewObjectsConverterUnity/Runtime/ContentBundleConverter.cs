using Speckle.ConnectorUnity.Converter;
using Speckle.Core.Models;
using UnityEngine;
using ViewObjects.Speckle;
using ViewObjects.Unity;

namespace ViewObjects.Converter.Unity
{
	[CreateAssetMenu(menuName = ViewObjectUnity.ASSET_MENU + "Create " + nameof(ContentBundleConverter), fileName = nameof(ContentBundleConverter), order = 0)]
	public class ContentBundleConverter : ComponentConverter<ContentBundleBaseV1, ContentBundleMono>
	{
		protected override Base ConvertComponent(ContentBundleMono component) => throw new System.NotImplementedException();

		protected override void ConvertBase(ContentBundleBaseV1 @base, ref ContentBundleMono instance)
		{
			throw new System.NotImplementedException();
		}
	}
}