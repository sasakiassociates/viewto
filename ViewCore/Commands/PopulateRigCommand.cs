using ViewObjects;
using ViewTo.Events.Process;

namespace ViewTo.Commands
{

  internal class PopulateRigCommand : ABuildRigCommand, ILoadCommand<IRig>
  {

    public PopulateRigCommand(IRig obj)
    {
      Obj = obj;
    }
    public IRig Obj { get; }

    public override void Run()
    {
      if (!ValidData)
      {
        processArgs.Add(new CancelRigBuildArgs("No Primed Study Data to build rig with ", errorFlag));
      }
      else
      {
        foreach (var arg in primedStudy.cloudArgs)
          Obj.Load(arg.id, arg.points);

        Obj.Load(GetGlobalColors(), GetBundles());
      }
    }
  }
}
