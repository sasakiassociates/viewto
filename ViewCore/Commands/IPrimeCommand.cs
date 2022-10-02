using System.Collections.Generic;
using ViewTo.Events.Args;

namespace ViewTo.Cmd
{

	internal interface ICommandWithResult<TResult> : ICmd where TResult : CommandDataArgs
	{
		public TResult args { get; }
	}

	internal interface IPrimeCommand : ICmd
	{
		public List<PrimeProcessArgs> args { get; }

		public bool greatSuccess { get; }
	}
}