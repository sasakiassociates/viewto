using Speckle.ConnectorUnity.Converter;
using Speckle.Core.Models;
using UnityEngine;
using ViewObjects.Speckle;
using ViewObjects.Unity;

namespace ViewObjects.Converter.Unity
{
	[CreateAssetMenu(menuName = ViewObjectUnity.ASSET_MENU + "Create " + nameof(BlockerContentConverter), fileName = nameof(BlockerContentConverter), order = 0)]
	public class BlockerContentConverter : ComponentConverter<BlockerContentBase, BlockerContentMono>
	{
		protected override Base ConvertComponent(BlockerContentMono component)
		{
			
			throw new System.NotImplementedException();
		}

		protected override void ConvertBase(BlockerContentBase @base, ref BlockerContentMono instance)
		{
			instance.viewName = @base.viewName;

			throw new System.NotImplementedException();
		}
	}
}