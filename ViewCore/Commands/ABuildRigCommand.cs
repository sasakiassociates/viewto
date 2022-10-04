using System.Collections.Generic;
using System.Linq;
using ViewObjects;
using ViewTo.Events.Args;
using ViewTo.Events.Prime;

namespace ViewTo.Cmd
{
	internal abstract class ABuildRigCommand : IStudyCommand
	{
		protected PrimeStudyArgs primedStudy;

		public ABuildRigCommand() => processArgs = new List<StudyProcessArgs>();

		protected bool ValidData
		{
			get => primedStudy != null;
		}

		public Study.LoadError errorFlag
		{
			get => Study.LoadError.BuildingRig;
		}

		public List<StudyProcessArgs> processArgs { get; protected set; }

		public bool greatSuccess { get; protected set; }

		public abstract void Execute();

		public virtual void ReceivePrimedData(PrimeProcessArgs primeArgs)
		{
			if (primeArgs is PrimeStudyArgs studyArgs)
				primedStudy = studyArgs;
		}

		protected List<ViewColor> GetGlobalColors()
		{
			var globalColors = new List<ViewColor>();

			foreach (var arg in primedStudy.contentArgs)
				globalColors.AddRange(arg.targets.Select(t => t.viewColor).ToList());

			return globalColors;
		}

		protected List<IRigParam_v1> GetBundles()
		{
			var globe = new List<IRigParam_v1>();

			// foreach (var arg in primedStudy.bundleArgs)
			// {
			// 	IRigParam_v1 paramV1 = default;
			//
			// 	if (arg is PrimeViewerBundleIsolatedArgs isolatedArgs)
			// 		paramV1 = new RigParametersIsolated
			// 		{
			// 			bundles = isolatedArgs.bundles.ToList(),
			// 			colors = new List<ViewColor>
			// 			{
			// 				isolatedArgs.viewColor
			// 			}
			// 		};
			// 	else
			// 		paramV1 = new RigParameters
			// 		{
			// 			bundles = arg.bundles.ToList()
			// 		};
			//
			// 	globe.Add(paramV1);
			// }

			return globe;
		}
	}
}