using System.Collections.Generic;
using System.Linq;
using ViewObjects;
using ViewTo.Events.Args;

namespace ViewTo.Events.Prime
{
	public class PrimeViewerBundleArgs : PrimeProcessArgs
	{
		public readonly IEnumerable<IViewerBundle_v1> bundles;

		public PrimeViewerBundleArgs(IEnumerable<IViewerBundle_v1> bundles) => this.bundles = bundles;

		public override bool success
		{
			get => bundles.Any();
		}
	}
}