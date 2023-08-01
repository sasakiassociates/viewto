namespace ViewTo.Cmd
{

  internal interface ICmd
  {
    public void Execute();
  }

  internal interface ICmdWithArgs<out TArgs> : ICmd where TArgs : CommandArgs
  {
    public TArgs args { get; }
  }

  internal interface ICommandArgs
  {

    public string Message { get; }

    public bool IsValid();
  }

}
