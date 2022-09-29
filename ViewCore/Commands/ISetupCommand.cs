using System;
using ViewTo.Events.Args;

namespace ViewTo.Commands
{
	internal interface ISetupCommand
	{
		public event Action<PrimeProcessArgs> onPrimedEvent;
	}
}