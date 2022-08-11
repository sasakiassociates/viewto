namespace ViewTo.Commands
{

  internal interface ILoadCommand<TObject>
  {
    public TObject Obj { get; }
  }
}
