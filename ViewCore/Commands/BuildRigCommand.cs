using System;
using System.Linq;
using ViewObjects.Rig;
using ViewTo.Events.Args;
using ViewTo.Events.Prime;
using ViewTo.Events.Process;

namespace ViewTo.Commands
{
  internal class BuildRigCommand : ABuildRigCommand, IBuildCommand, ISetupCommand
  {
    public event Action<PrimeProcessArgs> onPrimedEvent;

    public override void Run()
    {
      if (!ValidData)
      {
        processArgs.Add(new CancelRigBuildArgs("No Primed Study Data to build rig with ", errorFlag));
        return;
      }

      var rig = new Rig();
      foreach (var arg in primedStudy.cloudArgs)
        rig.Load(arg.id, arg.points);

      rig.Load(GetGlobalColors(), GetBundles());

      onPrimedEvent?.Invoke(new PrimedRigArgs(rig));
      greatSuccess = !processArgs.Any();
    }
  }
}
