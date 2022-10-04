using Speckle.ConnectorUnity.Converter;
using Speckle.Core.Models;
using UnityEngine;
using VS = ViewObjects.Speckle;
using VU = ViewObjects.Unity;

namespace ViewObjects.Converter.Unity
{

	[CreateAssetMenu(menuName = ViewObjectUnity.ASSET_MENU + "Create " + nameof(ViewStudyConverter), fileName = nameof(ViewStudyConverter), order = 0)]
	public class ViewStudyConverter : ComponentConverter<VS.ViewStudy, VU.ViewStudy>
	{
		public override Base ConvertComponent(VU.ViewStudy component)
		{
			
			throw new System.NotImplementedException();
		}

		protected override void ConvertBase(VS.ViewStudy obj, ref VU.ViewStudy instance)
		{
			throw new System.NotImplementedException();
		}


	}
}