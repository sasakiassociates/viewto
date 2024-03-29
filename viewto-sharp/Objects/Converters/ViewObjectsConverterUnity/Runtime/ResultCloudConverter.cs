﻿using Speckle.ConnectorUnity.Converter;
using Speckle.Core.Models;
using System.Linq;
using UnityEngine;
using ViewObjects.Clouds;
using ViewObjects.Results;
using VS = ViewObjects.Speckle;
using VU = ViewObjects.Unity;

namespace ViewObjects.Converter.Unity
{

  [CreateAssetMenu(menuName = ViewObjectUnity.ASSET_MENU + "Create " + nameof(ResultCloudConverter), fileName = nameof(ResultCloudConverter), order = 0)]
  public class ResultCloudConverter : ComponentConverter<VS.ResultCloud, VU.ResultCloud>
  {
    public override Base ConvertComponent(VU.ResultCloud component)
    {
      return new VS.ResultCloud(component.Points,
        component.Data.Select(x => new VS.ResultCloudData(x.values, x.info, x.count)).ToList(),
        component.ViewId);
    }

    protected override void ConvertBase(VS.ResultCloud obj, ref VU.ResultCloud instance)
    {
      instance.name = "ResultCloud";
      instance.ViewId = obj.ViewId;
      instance.Points = obj.Points;
      instance.Data = obj.Data.Select(x => new ResultCloudData(x.values, x.info)).Cast<IResultCloudData>().ToList();
    }
  }

}
