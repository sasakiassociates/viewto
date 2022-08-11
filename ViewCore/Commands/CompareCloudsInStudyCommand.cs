using System.Collections.Generic;
using System.Linq;
using ViewObjects;
using ViewTo.Events.Args;
using ViewTo.Events.Process;

namespace ViewTo.Commands
{
  /// <summary>
  ///   Checks that clouds and content are properly setup in this study
  /// </summary>
  internal class CompareCloudsInStudyCommand : IStudyCommand
  {

    public CompareCloudsInStudyCommand(IViewStudy study)
    {
      obj = study;
      processArgs = new List<StudyProcessArgs>();
    }

    private IViewStudy obj { get; }
    public List<StudyProcessArgs> processArgs { get; }
    public bool greatSuccess { get; private set; }
    public Study.LoadError errorFlag => Study.LoadError.UnlinkedClouds;

    public void Run()
    {
      var clouds = obj.GetAll<IViewCloud>();
      var targets = obj.Get<IViewContentBundle>().GetContents<ITargetContent>();

      // use all clouds from study as the base for what should be valid 
      var currentClouds = clouds.GetIds().ToList();

      foreach (var tc in targets)
        // check each cloud within a target against collection in the study. if any error is found add to args 
      foreach (var tCloud in tc.GetClouds().Where(tCloud => !currentClouds.Any(cc => cc.Equals(tCloud.cloudID))))
        processArgs.Add(new CloudComparedArg(false, tCloud.cloudID, tCloud.linkedContent));

      // cancel if any are missing from targets
      greatSuccess = !processArgs.Any();
    }
  }
}
