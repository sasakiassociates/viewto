using System;
using System.Collections.Generic;
using System.Linq;
using ViewObjects;
using ViewObjects.Rig;
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
    public static void Set<TObjType>(this IViewStudy study, TObjType obj) where TObjType : IViewObj
    {
      if (study.objs == null)
      {
        study.objs = new List<IViewObj>
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

    public static bool Has<TObjType>(this IViewStudy study, string name = "")
      where TObjType : IViewObj
    {
      if (study == null || study.objs == null || !study.objs.Any()) return false;

      foreach (var obj in study.objs.OfType<TObjType>())
      {
        // found at least one but no name was passed in 
        if (!name.Valid()) return true;

        // check if names match up
        if (obj.HasValidName() && obj.GetName().ToUpper().Equals(name.ToUpper()))
          return true;
      }
      return false;
    }

    public static List<TObjType> GetAll<TObjType>(this IViewStudy study)
      where TObjType : IViewObj
    {
      return study.objs.OfType<TObjType>().ToList();
    }

    public static TObjType Get<TObjType>(this IViewStudy study, string name = "")
      where TObjType : IViewObj
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

    public static bool CanRun(this IViewStudy study)
    {
      return study.Has<IViewCloud>() && study.Has<IViewContentBundle>() && study.Has<IViewerBundle>();
    }

    public static bool CanVisualize(this IViewStudy study)
    {
      return study.Has<IResultCloud>();
    }

    /// <summary>
    ///   Command for checking all clouds in a study and counting the points
    /// </summary>
    /// <param name="study"></param>
    /// <returns></returns>
    public static int GetPointCount(this IViewStudy study)
    {
      var res = 0;
      var clouds = study.GetAll<IViewCloud>();
      foreach (var cloud in clouds)
      {
        if (cloud == null || !cloud.points.Valid())
          continue;

        res += cloud.points.Length;
      }

      return res;
    }

    public static void Clear(this IViewStudy study)
    {
      study.objs = new List<IViewObj>();
    }

    public static void CheckData(this IViewStudy study, Action<StudyReportArgs> action)
    {

      if (!study.isValid) return;

      var cmd = new CheckStudyDataCommand(study);
      cmd.report += action;
      cmd.Run();
    }

    public static PrimedRigArgs LoadStudyForRig(this IViewStudy study)
    {

      IRig rig = new Rig();
      if (study.isValid)
      {
        var cmd = new LoadStudyToRigCommand(study, ref rig);
        cmd.Run();
      }

      return new PrimedRigArgs(rig);
    }

    public static void LoadStudyToRig(this IViewStudy study, ref IRig rigToBuild)
    {
      if (!study.isValid) return;

      var cmd = new LoadStudyToRigCommand(study, ref rigToBuild);
      cmd.Run();
    }

    public static void LoadStudyToRig(this IViewStudy study, out IRig rigToBuild, out List<StudyProcessArgs> argsList, out CancelStudyArgs cancelArgs)
    {
      cancelArgs = null;
      rigToBuild = default;
      argsList = new List<StudyProcessArgs>();

      if (!study.isValid) return;

      var cmd = new RunStudyCommand(study);
      cmd.Run();

      argsList = cmd.processArgs;
      cancelArgs = cmd.cancelStudyArgs;
      rigToBuild = cmd.Rig;
    }
  }
}
