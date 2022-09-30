using System.Collections.Generic;

namespace ViewTo.Commands
{
	internal interface ILinkedCommands<TCommand> : ICmd
	{
		public IList<TCommand> sequence { get; }
	}
}