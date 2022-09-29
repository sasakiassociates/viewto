using System.Collections.Generic;

namespace ViewTo.Commands
{
	internal interface ILinkedCommands<TCommand> : ICommand
	{
		public IList<TCommand> sequence { get; }
	}
}