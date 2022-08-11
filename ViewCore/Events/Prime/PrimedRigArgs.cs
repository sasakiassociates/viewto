using ViewObjects.Rig;
using ViewTo.Events.Args;

namespace ViewTo.Events.Prime
{
  internal sealed class PrimedRigArgs : PrimeProcessArgs
  {
    public readonly Rig Rig;
    
    public PrimedRigArgs(Rig rig)
    {
      Rig = rig;
    }
  }
}
