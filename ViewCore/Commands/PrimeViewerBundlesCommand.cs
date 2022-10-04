using System.Collections.Generic;
using System.Linq;
using ViewObjects;
using ViewTo.Events.Args;
using ViewTo.Events.Prime;

namespace ViewTo.Cmd
{
	internal class PrimeViewerBundlesCommand : PrimeObjectCommand<List<IViewerBundle_v1>>, IBuildCommand
	{

		public PrimeViewerBundlesCommand(List<IViewerBundle_v1> obj) : base(obj)
		{ }

		public void ReceivePrimedData(PrimeProcessArgs primeArgs)
		{
			if (primeArgs is PrimeViewerBundleArgs primed)
				args.Add(primed);
		}

		public override void Execute()
		{
			args.Add(new PrimeViewerBundleArgs(Obj));
			greatSuccess = args.Any();
		}
	}
}