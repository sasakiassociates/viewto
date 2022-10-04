using System;
using System.Linq;
using ViewTo.Events.Args;
using ViewTo.Events.Process;

namespace ViewTo.Cmd
{
	internal class BuildRigCommand : ABuildRigCommand, IBuildCommand, ISetupCommand
	{
		public event Action<PrimeProcessArgs> onPrimedEvent;

		public override void Execute()
		{
			if (!ValidData)
			{
				processArgs.Add(new CancelRigBuildArgs("No Primed Study Data to build rig with ", errorFlag));
				return;
			}

			// var rig = new RigV1();
			// foreach (var arg in primedStudy.cloudArgs)
			// 	rig.Load(arg.id, arg.points);
			//
			// rig.Load(GetGlobalColors(), GetBundles());
			//
			// onPrimedEvent?.Invoke(new PrimedRigArgs(rig));
			greatSuccess = !processArgs.Any();
		}
	}
}