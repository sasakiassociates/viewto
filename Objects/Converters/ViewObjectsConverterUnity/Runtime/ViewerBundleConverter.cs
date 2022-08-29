using Speckle.ConnectorUnity.Converter;
using Speckle.Core.Models;
using UnityEngine;
using ViewObjects.Speckle;
using ViewObjects.Unity;

namespace ViewObjects.Converter.Unity
{
	[CreateAssetMenu(menuName = ViewObjectUnity.ASSET_MENU + "Create " + nameof(ViewerBundleConverter), fileName = nameof(ViewerBundleConverter), order = 0)]
	public class ViewerBundleConverter : ComponentConverter<ViewerBundleBase, ViewerBundleMono>
	{
		protected override Base ConvertComponent(ViewerBundleMono component) => throw new System.NotImplementedException();

		protected override void ConvertBase(ViewerBundleBase @base, ref ViewerBundleMono instance)
		{
			throw new System.NotImplementedException();
		}
	}
}