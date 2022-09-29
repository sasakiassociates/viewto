using System.Collections.Generic;
using ViewTo.Events.Args;

namespace ViewTo.Commands
{

	internal interface IStudyCommand : ICommand
	{
		public List<StudyProcessArgs> processArgs { get; }

		public bool greatSuccess { get; }

		public Study.LoadError errorFlag { get; }
	}

}