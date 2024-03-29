﻿using Speckle.ConnectorUnity.Converter;
using Speckle.Core.Models;
using UnityEngine;
using VS = ViewObjects.Speckle;
using VU = ViewObjects.Unity;

namespace ViewObjects.Converter.Unity
{

  [CreateAssetMenu(menuName = ViewObjectUnity.ASSET_MENU + "Create " + nameof(ViewCloudConverter), fileName = nameof(ViewCloudConverter), order = 0)]
  public class ViewCloudConverter : ComponentConverter<VS.ViewCloudReference, VU.ViewCloud>
  {
    public override Base ConvertComponent(VU.ViewCloud component)
    {
      return new VS.ViewCloudReference(component.References, component.ViewId);
    }

    protected override void ConvertBase(VS.ViewCloudReference obj, ref VU.ViewCloud instance)
    {
      instance.ViewId = obj.ViewId;
      instance.References = obj.References;
    }
  }

}
