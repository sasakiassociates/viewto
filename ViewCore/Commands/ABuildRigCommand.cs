using System.Collections.Generic;
using System.Linq;
using ViewObjects;
using ViewObjects.Rig;
using ViewTo.Events.Args;
using ViewTo.Events.Prime;

namespace ViewTo.Commands
{
  internal abstract class ABuildRigCommand : IStudyCommand
  {
    protected PrimeStudyArgs primedStudy;

    public ABuildRigCommand()
    {
      processArgs = new List<StudyProcessArgs>();
    }

    protected bool ValidData => primedStudy != null;
    public Study.LoadError errorFlag => Study.LoadError.BuildingRig;

    public List<StudyProcessArgs> processArgs { get; protected set; }
    public bool greatSuccess { get; protected set; }

    public abstract void Run();

    public virtual void ReceivePrimedData(PrimeProcessArgs primeArgs)
    {
      if (primeArgs is PrimeStudyArgs studyArgs) primedStudy = studyArgs;
    }

    protected List<ViewColor> GetGlobalColors()
    {
      var globalColors = new List<ViewColor>();

      foreach (var arg in primedStudy.contentArgs)
        globalColors.AddRange(arg.targets.Select(t => t.viewColor).ToList());

      return globalColors;
    }

    protected List<IRigParam> GetBundles()
    {
      var globe = new List<IRigParam>();

      foreach (var arg in primedStudy.bundleArgs)
      {
        IRigParam param = default;

        if (arg is PrimeViewerBundleIsolatedArgs isolatedArgs)
          param = new RigParametersIsolated
          {
            bundles = isolatedArgs.bundles.ToList(),
            colors = new List<ViewColor>
            {
              isolatedArgs.viewColor
            }
          };
        else
          param = new RigParameters
          {
            bundles = arg.bundles.ToList()
          };

        globe.Add(param);
      }
      return globe;
    }
  }
}
