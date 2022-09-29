using System.Collections.Generic;
using Speckle.Core.Models;

namespace ViewObjects.Speckle
{

	public class ContentBundleBaseV1 : ViewObjectBase_v1, IValidate
	{

		public ContentBundleBaseV1()
		{ }

		[DetachProperty] public List<TargetContentBaseV1> targets { get; set; }

		[DetachProperty] public List<BlockerContentBaseV1> blockers { get; set; }

		[DetachProperty] public List<DesignContentBaseV1> designs { get; set; }

		public bool IsValid => targets.Valid() && blockers.Valid();
	}
}