using System;
using System.Collections.Generic;
using System.Linq;
using ViewObjects;
using ViewObjects.Clouds;
using ViewObjects.Common;
using ViewObjects.Contents;
using ViewObjects.Results;
using ViewObjects.Systems.Layouts;
using ViewTo.Cmd;

namespace ViewTo
{

  public static partial class ViewCoreExtensions
  {
    
    /// <summary>
    /// Short hand for safety convert
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int ToBitForm(this uint value) =>
      BitConverter.ToInt32(BitConverter.GetBytes(value), 0);

     
    /// <summary>
    ///  Short hand for safety convert
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static uint ToBitForm(this int value) =>
      BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);

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
    /// Returns a list of normalized values. The selected values are directly related to the inputs from <paramref cref="options"/> and the <paramref name="valueType"/>
    /// </summary>
    /// <param name="obj">the explorer to use</param>
    /// <param name="valueType">is used to select the values to normalize by</param>
    /// <param name="options">the options to find</param>
    /// <param name="results">normalized data</param>
    /// <returns></returns>
    public static bool TryGetValues<TOpt>(this IExplorer obj, ExplorerValueType valueType, List<TOpt> options, out IEnumerable<double> results) where TOpt : IContentOption
    {
      results = Array.Empty<double>();

      int[] inputA = null;
      int[] inputB = null;

      valueType.GetStages(out var stageA, out var stageB);

      // get each value list from the option
      foreach(var opt in options)
      {
        // var cmdA = new ValueFromOption(obj.data, new ContentOption(opt.target, opt.content, opt.stage));
        // var cmdB = new ValueFromOption(obj.data, new ContentOption(opt.target, opt.content, opt.stage));

        // TODO: Figure out why I'm using the input value type for this
        var cmdA = new ValueFromOption(obj.data, new ContentOption(opt.target, opt.content, stageA));
        var cmdB = new ValueFromOption(obj.data, new ContentOption(opt.target, opt.content, stageB));

        cmdA.Execute();
        cmdB.Execute();

        if(!cmdA.args.IsValid() || !cmdB.args.IsValid() || cmdA.args.values.Count() != cmdB.args.values.Count())
        {
          // TODO: return issue
          continue;
        }

        var rawValuesA = cmdA.args.values.ToArray();
        var rawValuesB = cmdB.args.values.ToArray();

        inputA ??= new int[rawValuesA.Length];
        inputB ??= new int[rawValuesB.Length];

        for(int i = 0; i < rawValuesA.Length; i++)
        {
          inputA[i] += rawValuesA[i];
          inputB[i] += rawValuesB[i];
        }
      }

      if(inputA == null || inputB == null || inputA.Length != inputB.Length)
      {
        // TODO: Report
        return false;
      }

      var normalizeCmd = new NormalizeValues(inputA, inputB);
      normalizeCmd.Execute();

      if(!normalizeCmd.args.IsValid())
      {
        // TODO: report
        return false;
      }

      results = normalizeCmd.args.values.ToArray();

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
