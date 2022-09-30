using Speckle.ConnectorUnity.Converter;
using Speckle.Core.Models;
using UnityEngine;
using ViewObjects.Speckle;
using ViewObjects.Unity;

namespace ViewObjects.Converter.Unity
{
	[CreateAssetMenu(menuName = ViewObjectUnity.ASSET_MENU + "Create " + nameof(BlockerContentConverter), fileName = nameof(BlockerContentConverter), order = 0)]
	public class BlockerContentConverter : ComponentConverter<BlockerContentBaseV1, BlockerContentMono>
	{
		protected override Base ConvertComponent(BlockerContentMono component)
		{
			
			throw new System.NotImplementedException();
		}

		protected override void ConvertBase(BlockerContentBaseV1 @base, ref BlockerContentMono instance)
		{
			instance.ViewName = @base.ViewName;

			throw new System.NotImplementedException();
		}
	}
}