using ViewTo.Events.Args;

namespace ViewTo.Cmd
{
	internal interface IBuildCommand
	{
		public void ReceivePrimedData(PrimeProcessArgs primeArgs);
	}
}