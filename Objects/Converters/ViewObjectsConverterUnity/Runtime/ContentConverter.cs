using Speckle.ConnectorUnity.Converter;
using Speckle.Core.Models;
using UnityEngine;
using VS = ViewObjects.Speckle;
using VU = ViewObjects.Unity;

namespace ViewObjects.Converter.Unity
{

	[CreateAssetMenu(menuName = ViewObjectUnity.ASSET_MENU + "Create " + nameof(ContentConverter), fileName = nameof(ContentConverter), order = 0)]
	public class ContentConverter : ComponentConverter<VS.Content, VU.Content>
	{

		public override Base ConvertComponent(VU.Content component)
		{
			return new VS.Content(component.ContentType, component.Reference, component.ViewId, component.ViewName);
		}

		protected override void ConvertBase(VS.Content obj, ref VU.Content instance)
		{
			instance.Reference = obj.References;
			instance.ContentType = obj.ContentType;
			instance.ViewId = obj.ViewId;
			instance.ViewName = obj.ViewName.Valid() ? obj.ViewName : obj.ContentType.ToString();
		}
	}
}