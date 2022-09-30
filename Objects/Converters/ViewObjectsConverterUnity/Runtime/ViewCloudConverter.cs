using Speckle.ConnectorUnity.Converter;
using Speckle.Core.Models;
using UnityEngine;
using ViewObjects.Speckle;
using ViewObjects.Unity;

namespace ViewObjects.Converter.Unity
{
	[CreateAssetMenu(menuName = ViewObjectUnity.ASSET_MENU + "Create " + nameof(ViewCloudConverter), fileName = nameof(ViewCloudConverter), order = 0)]
	public class ViewCloudConverter : ComponentConverter<ViewCloudBaseV1, ViewCloudMono>
	{
		protected override Base ConvertComponent(ViewCloudMono component) => throw new System.NotImplementedException();

		protected override void ConvertBase(ViewCloudBaseV1 @base, ref ViewCloudMono instance)
		{
			throw new System.NotImplementedException();
		}
	}
}