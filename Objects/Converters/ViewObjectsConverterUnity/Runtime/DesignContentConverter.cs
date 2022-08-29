﻿using Speckle.ConnectorUnity.Converter;
using Speckle.Core.Models;
using UnityEngine;
using ViewObjects.Speckle;
using ViewObjects.Unity;

namespace ViewObjects.Converter.Unity
{
	[CreateAssetMenu(menuName = ViewObjectUnity.ASSET_MENU + "Create " + nameof(DesignContentConverter), fileName = nameof(DesignContentConverter), order = 0)]
	public class DesignContentConverter : ComponentConverter<DesignContentBase, DesignContentMono>
	{
		protected override Base ConvertComponent(DesignContentMono component) => throw new System.NotImplementedException();

		protected override void ConvertBase(DesignContentBase @base, ref DesignContentMono instance)
		{
			throw new System.NotImplementedException();
		}
	}
}