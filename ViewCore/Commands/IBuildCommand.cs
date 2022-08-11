using ViewTo.Events.Args;

namespace ViewTo.Commands
{
  internal interface IBuildCommand
  {
    public void ReceivePrimedData(PrimeProcessArgs primeArgs);
  }
}
