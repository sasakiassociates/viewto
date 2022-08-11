namespace ViewTo.Events.Args
{

  public abstract class StudyProcessArgs : AEventArgs
  {
    public string message { get; protected set; }
    public bool success { get; protected set; }
  }
}
