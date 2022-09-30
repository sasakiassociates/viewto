using System;
using System.Collections.Generic;
using System.Linq;
using ViewObjects;
using ViewTo.Commands;
using ViewTo.Events.Args;
using ViewTo.Events.Process;
using ViewTo.Events.Report;

namespace ViewTo
{
	public static partial class Study
	{

		/// <summary>
		///   Method for setting a new object to the study
		/// </summary>
		/// <param name="study"></param>
		/// <param name="obj"> Object to set into the study</param>
		/// <returns></returns>
		public static void Set<TObjType>(this IViewStudy_v1 study, TObjType obj) where TObjType : IViewObject
		{
			if (study.objs == null)
			{
				study.objs = new List<IViewObject>
				{
					obj
				};
				return;
			}

			// NOTE this probably isn't worth getting caught up on just yet, will just add for now 
			study.objs = study.objs.Append(obj).ToList();
			// return;

			// check if an object with the same name has been added
			// if (obj.Name.Valid() && study.Has<TObjType>(obj.Name))
			// {
			//   // TODO throw error for sending an object with the exact name 
			//   // currently blocking if object has similar name 
			//   study.objs = study.objs.Append(obj);
			//
			// }
		}

		public static bool Has<TObjType>(this IViewStudy_v1 study, string name = "")
			where TObjType : IViewObject
		{
			if (study == null || study.objs == null || !study.objs.Any())
				return false;

			foreach (var obj in study.objs.OfType<TObjType>())
			{
				// found at least one but no name was passed in 
				if (!name.Valid())
					return true;

				// // check if names match up
				// if (obj.HasValidName() && obj.GetName().ToUpper().Equals(name.ToUpper()))
				// 	return true;
			}

			return false;
		}

		public static List<TObjType> GetAll<TObjType>(this IViewStudy_v1 study)
			where TObjType : IViewObject => study.objs.OfType<TObjType>().ToList();

		public static TObjType Get<TObjType>(this IViewStudy_v1 study, string name = "")
			where TObjType : IViewObject
		{
			TObjType result = default;
			try
			{
				foreach (var obj in study.objs.OfType<TObjType>())
				{
					result = obj;
					break;
				}
			}
			catch (Exception e)
			{
				//TODO: Write out error
				Console.WriteLine(e);
				throw;
			}

			return result;
		}

		public static bool CanRun(this IViewStudy_v1 study) =>
			study.Has<IViewCloud_v1>() && study.Has<IViewContentBundle_v1>() && study.Has<IViewerBundle_v1>();

		public static bool CanVisualize(this IViewStudy_v1 study) => study.Has<IResultCloudV1>();

		/// <summary>
		///   Command for checking all clouds in a study and counting the points
		/// </summary>
		/// <param name="study"></param>
		/// <returns></returns>
		public static int GetPointCount(this IViewStudy_v1 study)
		{
			var res = 0;
			var clouds = study.GetAll<IViewCloud_v1>();
			foreach (var cloud in clouds)
			{
				if (cloud == null || !cloud.points.Valid())
					continue;

				res += cloud.points.Length;
			}

			return res;
		}

		public static void Clear(this IViewStudy_v1 study)
		{
			study.objs = new List<IViewObject>();
		}

		public static void CheckData(this IViewStudy_v1 study, Action<StudyReportArgs> action)
		{
			if (!study.IsValid)
				return;

			var cmd = new CheckStudyDataCommand(study);
			cmd.report += action;
			cmd.Run();
		}

		public static PrimedRigArgs LoadStudyForRig(this IViewStudy_v1 study)
		{
			IRig_v1 rigV1 = new RigV1();
			if (study.IsValid)
			{
				var cmd = new LoadStudyToRigCommand(study, ref rigV1);
				cmd.Run();
			}

			return new PrimedRigArgs(rigV1);
		}

		public static void LoadStudyToRig(this IViewStudy_v1 study, ref IRig_v1 rigV1ToBuild)
		{
			if (!study.IsValid)
				return;

			var cmd = new LoadStudyToRigCommand(study, ref rigV1ToBuild);
			cmd.Run();
		}

		public static void LoadStudyToRig(
			this IViewStudy_v1 study, out IRig_v1 rigV1ToBuild, out List<StudyProcessArgs> argsList, out CancelStudyArgs cancelArgs
		)
		{
			cancelArgs = null;
			rigV1ToBuild = default;
			argsList = new List<StudyProcessArgs>();

			if (!study.IsValid)
				return;

			var cmd = new RunStudyCommand(study);
			cmd.Run();

			argsList = cmd.processArgs;
			cancelArgs = cmd.cancelStudyArgs;
			rigV1ToBuild = cmd.RigV1;
		}
	}
}