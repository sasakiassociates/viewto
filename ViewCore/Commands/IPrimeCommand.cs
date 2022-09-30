using System.Collections.Generic;
using ViewTo.Events.Args;

namespace ViewTo.Commands
{

	internal interface ICommandWithResult<TResult> : ICommand where TResult : CommandDataArgs
	{
		public TResult args { get; }
	}

	internal interface IPrimeCommand : ICommand
	{
		public List<PrimeProcessArgs> args { get; }

		public bool greatSuccess { get; }
	}
}