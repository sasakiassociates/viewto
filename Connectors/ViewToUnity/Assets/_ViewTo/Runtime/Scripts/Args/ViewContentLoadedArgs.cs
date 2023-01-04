#region

#endregion

namespace ViewTo.Connector.Unity.Commands
{

	public class ViewContentLoadedArgs : AReportEventArgs
	{
		public readonly int blockerCount, designCount, targetCount;

		public ViewContentLoadedArgs(int targetCount, int blockerCount, int designCount)
		{
			this.targetCount = targetCount;
			this.blockerCount = blockerCount;
			this.designCount = designCount;

			message = $"Target Count:{this.targetCount}\n"
			          + $"Blocker Count:{this.blockerCount}\n"
			          + $"Design Count:{this.designCount}\n";
		}

	}
}