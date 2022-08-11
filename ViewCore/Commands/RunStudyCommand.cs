using System.Collections.Generic;
using ViewObjects;
using ViewObjects.Rig;
using ViewTo.Events.Args;
using ViewTo.Events.Prime;
using ViewTo.Events.Process;


namespace ViewTo.Commands
{

  internal class RunStudyCommand : ILinkedCommands<IStudyCommand>, IBuildCommand
  {

    private readonly string studyName;

    public RunStudyCommand(IViewStudy study)
    {
      studyName = study.viewName;
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
    public Rig Rig { get; private set; }

    public void ReceivePrimedData(PrimeProcessArgs primeArgs)
    {
      if (primeArgs is PrimedRigArgs primedRig)
        Rig = primedRig.Rig;
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
          Rig = null;
          break;
        }
      }
    }
  }
}
