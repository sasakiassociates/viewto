using System.Collections.Generic;
using ViewObjects;
using ViewTo.Events.Args;

namespace ViewTo.Events.Prime
{
  public sealed class PrimeContentCaseArgs : PrimeProcessArgs
  {
    public readonly List<IBlockerContent> blockers;
    public readonly List<IDesignContent> designs;
    public readonly List<ITargetContent> targets;

    public PrimeContentCaseArgs(List<ITargetContent> targets, List<IBlockerContent> blockers, List<IDesignContent> designs = null)
    {
      this.targets = targets;
      this.blockers = blockers;
      this.designs = designs;
    }

    public override bool success => targets.Valid() && blockers.Valid();
  }
}
