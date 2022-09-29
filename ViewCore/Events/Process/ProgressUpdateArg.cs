using ViewTo.Events.Args;

namespace ViewTo.Events.Process
{
	public class ProgressUpdateArg : StudyProcessArgs
	{

		public ProgressUpdateArg(Study.ProgressCheck check, bool success)
		{
			this.success = success;
			message = $"Study Progress Update: {check.Message(success)}";
		}
	}
}