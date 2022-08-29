using Speckle.ConnectorUnity.Converter;
using Speckle.Core.Models;
using UnityEngine;
using ViewObjects.Speckle;
using ViewObjects.Unity;

namespace ViewObjects.Converter.Unity
{
	[CreateAssetMenu(menuName = ViewObjectUnity.ASSET_MENU + "Create " + nameof(TargetContentConverter), fileName = nameof(TargetContentConverter), order = 0)]
	public class TargetContentConverter : ComponentConverter<TargetContentBase, TargetContentMono>
	{
		protected override Base ConvertComponent(TargetContentMono component) => throw new System.NotImplementedException();

		protected override void ConvertBase(TargetContentBase @base, ref TargetContentMono instance)
		{
			throw new System.NotImplementedException();
		}
	}
}