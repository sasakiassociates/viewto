using System;
using ViewTo.Events.Args;

namespace ViewTo.Cmd
{
	internal interface ISetupCommand
	{
		public event Action<PrimeProcessArgs> onPrimedEvent;
	}
}