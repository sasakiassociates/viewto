using System.Collections.Generic;
using ViewObjects;
using ViewTo.Cmd;

namespace ViewTo
{
	public static partial class ViewCoreExtensions
	{
		public static List<string> LoadStudyToRig(this IViewStudy study, ref IRig rig)
		{
			var contents = study.GetAll<IContent>();
			var clouds = study.GetAll<IViewCloud>();
			var viewers = study.GetAll<IViewer>();

			var reports = new List<string>();

			var sequence = new List<ICmd>
			{
				new CanStudyRun(contents, clouds, viewers),
				new AssignViewColors(contents),
				new InitializeAndBuildRig(rig, contents, clouds, viewers)
			};

			foreach (var s in sequence)
			{
				s.Execute();

				if (s is ICmdWithArgs<CommandArgs> cmdWithArgs)
				{
					reports.Add(cmdWithArgs.args.Message);
				}
			}

			return reports;
		}

	}
}