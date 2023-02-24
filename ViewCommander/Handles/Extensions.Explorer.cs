using System;
using System.Collections.Generic;
using System.Linq;
using ViewObjects;
using ViewObjects.Common;
using ViewObjects.Contents;
using ViewObjects.Results;
using ViewTo.Cmd;

namespace ViewTo
{

  public static partial class ViewCoreExtensions
  {

    /// <summary>
    ///   Retrieves the active point result data
    /// </summary>
    /// <returns></returns>
    public static bool TryGetResultPoint(this IExplorer obj, double[] values, double value, out ResultPoint point)
    {
      point = null;

      var cmd = new FindPointWithValue(values, value);
      cmd.Execute();

      if(cmd.args.IsValid())
      {
        var cp = obj.cloud.Points[cmd.args.index];
        var v = values[cmd.args.index];
        point = new ResultPoint
        {
          x = cp.x, y = cp.y, z = cp.z,
          index = cmd.args.index,
          value = value,
          color = obj.settings.GetColor(v)
        };
      }

      return point != null;
    }

    /// <summary>
    ///   Get the values
    /// </summary>
    /// <param name="exp"></param>
    /// <param name="valueType"></param>
    /// <param name="results"></param>
    /// <returns></returns>
    public static bool TryGetValues(this IExplorer exp, ExplorerValueType valueType, out IEnumerable<double> results)
    {
      results = Array.Empty<double>();
      valueType.GetStages(out var stageA, out var stageB);

      var getValueCmdA = new TryGetValues(exp.data, exp.meta.activeTarget.ViewId, stageA);
      var getValueCmdB = new TryGetValues(exp.data, exp.meta.activeTarget.ViewId, stageB);

      getValueCmdA.Execute();
      getValueCmdB.Execute();

      if(!getValueCmdA.args.IsValid() || !getValueCmdB.args.IsValid())
      {
        // TODO: return issue
        return false;
      }

      var normalizeCmd = new NormalizeValues(getValueCmdA.args.values, getValueCmdB.args.values);
      normalizeCmd.Execute();

      if(!normalizeCmd.args.IsValid())
      {
        // TODO: report
        return false;
      }

      results = normalizeCmd.args.values;

      return results.Any();
    }




    /// <summary>
    ///   Get the values
    /// </summary>
    /// <param name="exp"></param>
    /// <param name="valueType"></param>
    /// <param name="target"></param>
    /// <param name="results"></param>
    /// <returns></returns>
    public static bool TryGetValues(this IExplorer exp, ExplorerValueType valueType, string target, ref double[] results)
    {
      if(!exp.cloud.HasTarget(target))
      {
        return false;
      }

      valueType.GetStages(out var stageA, out var stageB);

      var getValueCmdA = new TryGetValues(exp.data, exp.meta.activeTarget.ViewId, stageA);
      var getValueCmdB = new TryGetValues(exp.data, exp.meta.activeTarget.ViewId, stageB);

      getValueCmdA.Execute();
      getValueCmdB.Execute();

      if(!getValueCmdA.args.IsValid() || !getValueCmdB.args.IsValid())
      {
        // TODO: return issue
        return false;
      }

      var normalizeCmd = new NormalizeValues(getValueCmdA.args.values, getValueCmdB.args.values);
      normalizeCmd.Execute();

      if(!normalizeCmd.args.IsValid())
      {
        // TODO: report
        return false;
      }

      results = normalizeCmd.args.values.ToArray();

      return results.Any();
    }

    public static void GetStages(this ExplorerValueType type, out ViewContentType stageA, out ViewContentType stageB)
    {
      switch(type)
      {
        case ExplorerValueType.ExistingOverPotential:
          stageA = ViewContentType.Existing;
          stageB = ViewContentType.Potential;
          break;
        case ExplorerValueType.ProposedOverExisting:
          stageA = ViewContentType.Proposed;
          stageB = ViewContentType.Existing;
          break;
        case ExplorerValueType.ProposedOverPotential:
          stageA = ViewContentType.Proposed;
          stageB = ViewContentType.Potential;
          break;
        default:
          stageA = ViewContentType.Potential;
          stageB = ViewContentType.Potential;
          break;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="targetId"></param>
    /// <param name="contentId"></param>
    /// <param name="stage"></param>
    /// <returns></returns>
    public static void SetOption(this IExplorer obj, string targetId, string contentId, ViewContentType stage)
    {
      if(obj.cloud == null || !obj.cloud.HasOpt(targetId, contentId, stage))
      {
        return;
      }
      obj.TrySetOption(obj.cloud.GetOpt(targetId, contentId, stage));

    }

    /// <summary>
    /// Sets a current 
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="targetId"></param>
    /// <param name="stage"></param>
    public static void SetOption(this IExplorer obj, string targetId, ViewContentType stage)
    {
      obj.SetOption(targetId, targetId, stage);
    }


    /// <summary>
    /// Check if this option is already being used and sets the <see cref="ExplorerMetaData.activeTarget"/>
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="option">the option to use</param>
    /// <param name="addToList">if true it adds the option to the <see cref="ExplorerMetaData.activeOptions"/></param>
    static void TrySetOption(this IExplorer obj, IContentOption option, bool addToList = false)
    {
      if(option?.target == null || !option.target.ViewId.Valid() || option.content == null || !option.content.ViewId.Valid())
      {
        return;
      }

      if(!addToList)
      {
        obj.meta.activeOptions = new List<IContentOption>();
      }

      if(!obj.meta.activeOptions.Any(x => x.stage == option.stage
                                          && x.target.ViewId.Equals(option.target.ViewId)
                                          && x.content.ViewId.Equals(option.content.ViewId)))
      {
        obj.meta.activeOptions.Add(option);
      }

      obj.meta.activeTarget = option.target;

    }


    public static bool InRange(this IExploreRange obj, double value)
    {
      return value >= obj.min && value <= obj.max;
    }
  }

}
