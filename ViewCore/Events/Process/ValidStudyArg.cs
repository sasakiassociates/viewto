using ViewTo.Events.Args;

namespace ViewTo.Events.Process
{
	public class ValidStudyArg : StudyProcessArgs
	{
		public ValidStudyArg(bool success, string name, string update)
		{
			this.success = success;
			message = $"Study {name} is valid? {success}\n Message: {update}";
		}
	}
}