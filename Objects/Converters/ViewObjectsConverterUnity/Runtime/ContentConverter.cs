using Speckle.ConnectorUnity.Converter;
using Speckle.Core.Models;
using UnityEngine;
using ViewObjects.Common;
using VS = ViewObjects.Speckle;
using VU = ViewObjects.Unity;

namespace ViewObjects.Converter.Unity
{

  [CreateAssetMenu(menuName = ViewObjectUnity.ASSET_MENU + "Create " + nameof(ContentConverter), fileName = nameof(ContentConverter), order = 0)]
  public class ContentConverter : ComponentConverter<VS.ContentReference, VU.Content>
  {

    public override Base ConvertComponent(VU.Content component)
    {
      return new VS.ContentReference(component.type, component.References, component.ViewId, component.ViewName);
    }

    protected override void ConvertBase(VS.ContentReference obj, ref VU.Content instance)
    {
      instance.ViewId = obj.ViewId;
      instance.ViewName = obj.ViewName.Valid() ? obj.ViewName : obj.type.ToString();
      instance.References = obj.References;
      instance.type = obj.type;
    }
  }

}
