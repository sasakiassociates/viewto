using System.Collections.Generic;
using Speckle.Newtonsoft.Json;

namespace ViewObjects.Speckle
{

	public class ViewerBundleBaseV1 : ViewObjectBase_v1, IViewerBundle
	{

		public ViewerBundleBaseV1()
		{ }

		[JsonIgnore] public virtual bool isValid => layouts.Valid() && layouts.Valid();

		public List<IViewerLayout> layouts { get; set; }
	}
}