using System.Collections.Generic;
using ViewTo.Events.Args;

namespace ViewTo.Commands
{
  internal abstract class PrimeObjectCommand<TObject> : IPrimeCommand
  {
    protected PrimeObjectCommand(TObject obj)
    {
      Obj = obj;
      args = new List<PrimeProcessArgs>();
    }

    public TObject Obj { get; }
    public List<PrimeProcessArgs> args { get; protected set; }
    public bool greatSuccess { get; protected set; }

    public abstract void Run();
  }
}
