using System;
using System.Collections.Generic;

namespace ViewTo.Cmd
{
	internal class Invoker
	{

		public List<ICmd> sequence { get; private set; }

		public List<string> reports { get; private set; }

		public Action<CommandArgs> OnCommandReport;

		public Invoker(List<ICmd> commands, Action<CommandArgs> onCommandReport = null)
		{
			sequence = commands;
			OnCommandReport = onCommandReport;
			reports = new List<string>();
		}

		public void Run()
		{
			foreach (var s in sequence)
			{
				s.Execute();

				if (s is ICmdWithArgs<CommandArgs> cmdWithArgs)
				{
					reports.Add(cmdWithArgs.args.Message);
					OnCommandReport?.Invoke(cmdWithArgs.args);
				}
			}
		}
	}

}