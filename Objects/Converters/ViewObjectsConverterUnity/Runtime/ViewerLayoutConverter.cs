using Speckle.ConnectorUnity.Converter;
using Speckle.Core.Models;
using UnityEngine;
using ViewObjects.Speckle;
using ViewObjects.Unity;

namespace ViewObjects.Converter.Unity
{
	[CreateAssetMenu(menuName = ViewObjectUnity.ASSET_MENU + "Create " + nameof(ViewerLayoutConverter), fileName = nameof(ViewerLayoutConverter), order = 0)]
	public class ViewerLayoutConverter : ComponentConverter<ViewerLayoutBase, ViewerLayoutMono>
	{
		protected override Base ConvertComponent(ViewerLayoutMono component) => throw new System.NotImplementedException();

		protected override void ConvertBase(ViewerLayoutBase @base, ref ViewerLayoutMono instance)
		{
			throw new System.NotImplementedException();
		}
	}
}