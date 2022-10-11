#region

#endregion

namespace ViewTo.Connector.Unity.Commands
{

	public class StudyInitArgs : AReportEventArgs
	{
		public readonly ViewerBundleInitArgs bundleArgs;

		public readonly ViewContentLoadedArgs contentArgs;

		public StudyInitArgs(ViewContentLoadedArgs contentArgs, ViewerBundleInitArgs bundleArgs)
		{
			this.contentArgs = contentArgs;
			this.bundleArgs = bundleArgs;
			message = $"Study Initialized\n{contentArgs.message}\n{bundleArgs.message}";
		}
	}

	public class ViewerBundleInitArgs : AReportEventArgs
	{

		public readonly int layoutCount, bundleCount;

		public ViewerBundleInitArgs(int layoutCount, int bundleCount)
		{
			this.layoutCount = layoutCount;
			this.bundleCount = bundleCount;

			message = $"Bundles({bundleCount}) with Layouts({layoutCount}) ";
		}
	}

	public class StudyLoadedArgs : AReportEventArgs
	{
		public readonly bool canRun, canVisualize;

		public readonly string studyName;

		public readonly int totalPointCount;
		// public readonly int globalViewerBundleCount, isolatedViewerBundleCount;

		public StudyLoadedArgs(string studyName, int totalPointCount, bool canRun, bool canVisualize)
		{
			this.studyName = studyName;
			this.totalPointCount = totalPointCount;
			this.canRun = canRun;
			this.canVisualize = canVisualize;
			message = $"{this.studyName}\nPoint Count:{this.totalPointCount}\n";
		}
	}

}