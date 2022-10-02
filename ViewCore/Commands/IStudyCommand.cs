using System.Collections.Generic;
using ViewTo.Events.Args;

namespace ViewTo.Cmd
{

	internal interface IStudyCommand : ICmd
	{
		public List<StudyProcessArgs> processArgs { get; }

		public bool greatSuccess { get; }

		public Study.LoadError errorFlag { get; }
	}

}