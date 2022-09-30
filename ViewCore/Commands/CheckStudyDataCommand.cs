using System;
using System.Collections.Generic;
using System.Linq;
using ViewObjects;
using ViewTo.Events.Args;
using ViewTo.Events.Process;
using ViewTo.Events.Report;

namespace ViewTo.Commands
{

	internal class CheckStudyDataCommand : IStudyCommand
	{

		public CheckStudyDataCommand(IViewStudy_v1 study)
		{
			obj = study;
			processArgs = new List<StudyProcessArgs>();
		}

		IViewStudy_v1 obj { get; }

		public Study.LoadError errorFlag
		{
			get => Study.LoadError.MissingObjects;
		}

		public List<StudyProcessArgs> processArgs { get; }

		public bool greatSuccess { get; set; }

		public void Run()
		{
			try
			{
				if (!obj.CanRun())
				{
					greatSuccess = false;
				}
				else
				{
					var content = obj.Get<IViewContentBundle_v1>();
					var blockersCount = content.GetContentCount<IBlockerContentV1>();
					var designsCount = content.GetContentCount<IDesignContentV1>();
					var targetCount = content.GetContentCount<ITargetContentV1>();

					var clouds = obj.GetAll<IViewCloud_v1>().ToDictionary(cld => cld?.ViewId, cld => cld != null && cld.points.Valid() ? cld.points.Length : 0);

					var bundles = obj.GetAll<IViewerBundle_v1>().ToList();
					var globalBundleCount = 0;
					foreach (var bundle in bundles)
					{
						var layouts = bundle.layouts;
						if (layouts.Valid())
							globalBundleCount += layouts.Count;
					}

					var isoTargetCount = 0;
					var isoBundleCount = 0;
					foreach (var target in content.GetContents<ITargetContentV1>())
					{
						var cl = target.SearchForClouds();
						if (cl.Valid())
							foreach (var c in cl.Where(c => !clouds.ContainsKey(c.objId)))
								clouds.Add(c.objId, c.count);

						if (target.isolate)
						{
							isoTargetCount++;
							isoBundleCount += target.bundles.Valid() ? target.bundles.Count : 0;
						}
					}

					report?.Invoke(new StudyReportArgs(
						               obj.ViewName, obj.CanRun(), obj.CanVisualize(),
						               blockersCount, designsCount, targetCount,
						               isoTargetCount, globalBundleCount, isoBundleCount, clouds.Count,
						               clouds.Values.Sum()));
					// basic inputs needed for a project to run 
					greatSuccess = clouds.Any() && targetCount > 0 && globalBundleCount > 0;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}

			if (!greatSuccess)
				processArgs.Add(new ValidStudyArg(false, obj.ViewName, errorFlag.Message()));
		}

		public event Action<StudyReportArgs> report;
	}
}