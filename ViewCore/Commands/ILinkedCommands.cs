using System.Collections.Generic;

namespace ViewTo.Cmd
{
	internal interface ILinkedCommands<TCommand> : ICmd
	{
		public IList<TCommand> sequence { get; }
	}
}