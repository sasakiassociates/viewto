using System.Linq;
using Speckle.ConnectorUnity.Converter;
using Speckle.Core.Models;
using UnityEngine;
using ViewObjects.Systems.Layouts;
using VS = ViewObjects.Speckle;
using VU = ViewObjects.Unity;

namespace ViewObjects.Converter.Unity
{
	[CreateAssetMenu(menuName = ViewObjectUnity.ASSET_MENU + "Create " + nameof(ViewerConverter), fileName = nameof(ViewerConverter), order = 0)]
	public class ViewerConverter : ComponentConverter<VS.Viewer, VU.Viewer>
	{
		public override Base ConvertComponent(VU.Viewer component)
		{
			return new VS.Viewer(component.Layouts.Select(x => new VS.Layout(x.Viewers)).ToList());
		}

		protected override void ConvertBase(VS.Viewer obj, ref VU.Viewer instance)
		{
			instance.Layouts = obj.Layouts.Select(x => (ILayout)new Layout(x.Viewers)).ToList();
		}

	}
}