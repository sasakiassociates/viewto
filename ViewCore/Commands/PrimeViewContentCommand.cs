using System;
using System.Linq;
using ViewObjects;
using ViewTo.Events.Args;
using ViewTo.Events.Prime;

namespace ViewTo.Commands
{

  internal class PrimeViewContentCommand : PrimeObjectCommand<IViewContentBundle>, ISetupCommand
  {
    public PrimeViewContentCommand(IViewContentBundle obj) : base(obj)
    { }

    public event Action<PrimeProcessArgs> onPrimedEvent;

    public override void Run()
    {
      // check if assigned colors worked
      Obj.AssignColors();
      var targets = Obj.GetContents<ITargetContent>();
      if (targets.Valid())
        foreach (var tc in targets.Where(tc => tc.bundles.Valid()))
          onPrimedEvent?.Invoke(tc.isolate ?
                                  new PrimeViewerBundleIsolatedArgs(tc.bundles, tc.viewColor) :
                                  new PrimeViewerBundleArgs(tc.bundles));

      // NOTE: Not sure what this is doing, I remember there was some reason to have targets and designs in this command
      // if (Obj.designs.Valid())
      // {
      //   foreach (var tc in Obj.targets.Where(tc => tc.bundles.Valid()))
      //     onPrimedEvent?.Invoke(tc.isolate ?
      //                             new PrimeViewerBundleIsolatedArgs(tc.bundles, tc.viewColor) :
      //                             new PrimeViewerBundleArgs(tc.bundles));
      // }

      args.Add(new PrimeContentCaseArgs(targets, Obj.GetContents<IBlockerContent>(), Obj.GetContents<IDesignContent>()));
      greatSuccess = args.Any();
    }
  }
}
