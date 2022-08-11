using ViewObjects;
using ViewTo.Events.Args;

namespace ViewTo.Events.Process
{

  public class CloudComparedArg : StudyProcessArgs
  {
    public CloudComparedArg(bool success, string cloudID, string targetID = "")
    {
      this.success = success;
      message = $"Cloud ID Compared: {Study.ProgressCheck.ViewClouds.Message(success)}\n"
                + $"Cloud ID {cloudID}\n"
                + (!targetID.Valid() ? "No Target Info to Connect" : $"{targetID}");

    }
  }
}
