using System;
using System.Collections.Generic;
using System.Linq;
using ViewObjects;
using ViewTo.Events.Args;
using ViewTo.Events.Prime;
using ViewTo.Events.Process;
using ViewTo.Primers;

namespace ViewTo.Cmd
{
	internal class SetupStudyObjectsCommand : ISetupCommand, IStudyCommand, ILinkedCommands<IPrimeCommand>
	{

		public SetupStudyObjectsCommand(IViewStudy_v1 study)
		{
			processArgs = new List<StudyProcessArgs>();
			sequence = new List<IPrimeCommand>
			{
				new PrimeViewCloudsCommand(study.GetAll<IViewCloud_v1>())
			};

			var content = new PrimeViewContentCommand(study.Get<IViewContentBundle_v1>());
			var bundles = new PrimeViewerBundlesCommand(study.GetAll<IViewerBundle_v1>().ToList());

			// pass in any target data
			content.onPrimedEvent += args => bundles.ReceivePrimedData(args);
			sequence.Add(content);
			sequence.Add(bundles);
		}

		public IList<IPrimeCommand> sequence { get; }

		public event Action<PrimeProcessArgs> onPrimedEvent;

		public List<StudyProcessArgs> processArgs { get; }

		public bool greatSuccess { get; private set; }

		public Study.LoadError errorFlag
		{
			get => Study.LoadError.ObjectSetup;
		}

		public void Run()
		{
			var data = new PrimedStudy();
			foreach (var cmd in sequence)
			{
				cmd.Run();
				if (!cmd.greatSuccess)
				{
					processArgs.Add(new CancelObjectSetupArgs(cmd.ToString(), errorFlag));
					break;
				}

				data.args.AddRange(cmd.args);
			}

			greatSuccess = !processArgs.Any();
			if (greatSuccess)
				onPrimedEvent?.Invoke(new PrimeStudyArgs(data));
		}
	}
}