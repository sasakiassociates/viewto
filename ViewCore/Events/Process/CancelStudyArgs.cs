using ViewTo.Events.Args;

namespace ViewTo.Events.Process
{

  public class CancelStudyArgs : StudyProcessArgs
  {

    public CancelStudyArgs(string name, Study.LoadError error)
    {
      success = false;
      message = $"Study {name} was canceled! Error was {error.Message()}";
    }
  }
}
