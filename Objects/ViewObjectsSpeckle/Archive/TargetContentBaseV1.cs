using System.Collections.Generic;
using Speckle.Core.Models;
using Speckle.Newtonsoft.Json;

namespace ViewObjects.Speckle
{

	public class TargetContentBaseV1 : ViewContentBaseV1
	{

		public TargetContentBaseV1()
		{ }

		public bool isolate { set; get; }

		public ViewColor viewColor { get; set; }

		[DetachProperty] public List<ViewerBundleBaseV1> bundles { get; set; }

		[JsonIgnore] public override bool IsValid => base.IsValid && ViewName.Valid();
	}
}