using Speckle.ConnectorUnity.Converter;
using Speckle.Core.Models;
using UnityEngine;
using ViewObjects.Speckle;
using ViewObjects.Unity;

namespace ViewObjects.Converter.Unity
{
	[CreateAssetMenu(menuName = ViewObjectUnity.ASSET_MENU + "Create " + nameof(ResultCloudConverter), fileName = nameof(ResultCloudConverter), order = 0)]
	public class ResultCloudConverter : ComponentConverter<ResultCloudBaseV1, ResultCloudMono>
	{
		protected override Base ConvertComponent(ResultCloudMono component) => throw new System.NotImplementedException();

		protected override void ConvertBase(ResultCloudBaseV1 @base, ref ResultCloudMono instance)
		{
			throw new System.NotImplementedException();
		}
	}
}