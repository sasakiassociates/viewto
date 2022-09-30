using System.Collections.Generic;
using ViewObjects;
using ViewTo.Events.Args;

namespace ViewTo.Events.Prime
{
	public sealed class PrimeContentCaseArgs : PrimeProcessArgs
	{
		public readonly List<IBlockerContentV1> blockers;
		public readonly List<IDesignContentV1> designs;
		public readonly List<ITargetContentV1> targets;

		public PrimeContentCaseArgs(List<ITargetContentV1> targets, List<IBlockerContentV1> blockers, List<IDesignContentV1> designs = null)
		{
			this.targets = targets;
			this.blockers = blockers;
			this.designs = designs;
		}

		public override bool success
		{
			get => targets.Valid() && blockers.Valid();
		}
	}
}