using Speckle.ConnectorUnity.Converter;
using Speckle.Core.Models;
using UnityEngine;
using ViewObjects.Speckle;
using ViewObjects.Unity;

namespace ViewObjects.Converter.Unity
{

	[CreateAssetMenu(menuName = ViewObjectUnity.ASSET_MENU + "Create " + nameof(ViewStudyConverter), fileName = nameof(ViewStudyConverter), order = 0)]
	public class ViewStudyConverter : ComponentConverter<ViewStudyBase_v2, ViewStudyMono>
	{
		protected override Base ConvertComponent(ViewStudyMono component) => throw new System.NotImplementedException();

		protected override void ConvertBase(ViewStudyBase_v2 @base, ref ViewStudyMono instance)
		{
			throw new System.NotImplementedException();
		}

	}
}