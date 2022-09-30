using ViewObjects;
using ViewTo.Events.Process;

namespace ViewTo.Commands
{

	internal class PopulateRigCommand : ABuildRigCommand, ILoadCommand<IRig_v1>
	{

		public PopulateRigCommand(IRig_v1 obj) => Obj = obj;

		public IRig_v1 Obj { get; }

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