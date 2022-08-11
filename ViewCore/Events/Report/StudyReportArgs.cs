using ViewTo.Events.Args;

namespace ViewTo.Events.Report
{

  public class StudyReportArgs : AReportEventArgs
  {
    public readonly int blockerCount, designCount, targetCount, isolatedTargetCount, totalBundleCount;
    public readonly bool canRun, canVisualize;
    public readonly int globalViewerBundleCount, isolatedViewerBundleCount;

    public readonly string studyName;
    public readonly int viewCloudCount, totalPointCount;
    //TODO missing result cloud values

    public StudyReportArgs(
      string studyName, bool canRun, bool canVisualize,
      int blockerCount, int designCount, int targetCount, int isolatedTargetCount,
      int globalViewerBundleCount, int isolatedViewerBundleCount, int viewCloudCount, int totalPointCount
    )
    {
      this.studyName = studyName;
      this.canRun = canRun;
      this.canVisualize = canVisualize;
      this.blockerCount = blockerCount;
      this.designCount = designCount;
      this.targetCount = targetCount;
      this.isolatedTargetCount = isolatedTargetCount;
      this.globalViewerBundleCount = globalViewerBundleCount;
      this.isolatedViewerBundleCount = isolatedViewerBundleCount;
      this.viewCloudCount = viewCloudCount;
      this.totalPointCount = totalPointCount;

      totalBundleCount = globalViewerBundleCount + isolatedViewerBundleCount;

      message = CreateMessage();
    }

    private string CreateMessage()
    {
      return$"Report For {studyName}: CanRun={canRun} CanVisualize={canVisualize}\n"
            + $"Contents: Targets/Isolated={targetCount}/{isolatedTargetCount}, Blockers={blockerCount}, Designs={designCount}\n"
            + $"Clouds: Total={viewCloudCount}, Points={totalPointCount}\n"
            + $"Bundles: Global={globalViewerBundleCount}, Isolated={isolatedViewerBundleCount}, Total={totalBundleCount}\n";
    }
  }
}
