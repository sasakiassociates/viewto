using ViewTo.Events.Args;

namespace ViewTo.Events.Process
{
	public class CancelRigBuildArgs : StudyProcessArgs
	{
		public CancelRigBuildArgs(string name, Study.LoadError error)
		{
			success = false;
			message = $"Study {name} was canceled! Error was {error.Message()}";
		}
	}
}