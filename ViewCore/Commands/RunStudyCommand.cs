using System.Collections.Generic;
using ViewObjects;
using ViewTo.Events.Args;
using ViewTo.Events.Process;

namespace ViewTo.Cmd
{

	internal class RunStudyCommand : ILinkedCommands<IStudyCommand>, IBuildCommand
	{

		readonly string studyName;

		public RunStudyCommand(IViewStudy_v1 study)
		{
			studyName = study.ViewName;
			processArgs = new List<StudyProcessArgs>();

			sequence = new List<IStudyCommand>
			{
				new CheckStudyDataCommand(study), new CompareCloudsInStudyCommand(study)
			};

			var setupStudy = new SetupStudyObjectsCommand(study);
			var rigBuild = new BuildRigCommand();
			setupStudy.onPrimedEvent += data => rigBuild.ReceivePrimedData(data);
			rigBuild.onPrimedEvent += ReceivePrimedData;

			sequence.Add(setupStudy);
			sequence.Add(rigBuild);
		}

		public CancelStudyArgs cancelStudyArgs { get; private set; }

		public List<StudyProcessArgs> processArgs { get; }

		public RigV1 RigV1 { get; private set; }

		public void ReceivePrimedData(PrimeProcessArgs primeArgs)
		{
			// if (primeArgs is PrimedRigArgs primedRig)
			// 	RigV1 = primedRig.RigV1;?
		}

		public IList<IStudyCommand> sequence { get; }

		public void Run()
		{
			foreach (var cmd in sequence)
			{
				cmd.Run();

				if (cmd.processArgs != null)
					processArgs.AddRange(cmd.processArgs);

				if (!cmd.greatSuccess)
				{
					cancelStudyArgs = new CancelStudyArgs(studyName, cmd.errorFlag);
					RigV1 = null;
					break;
				}
			}
		}
	}
}