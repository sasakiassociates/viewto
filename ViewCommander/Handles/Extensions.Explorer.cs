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

  public interface IExplorerCommand
  { }

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
    ///  Get the values
    /// </summary>
    /// <param name="exp"></param>
    /// <param name="results"></param>
    /// <param name="optionA"></param>
    /// <param name="optionB"></param>
    /// <returns></returns>
    public static bool TryGetSols(this IExplorer exp, ContentOption optionA, ContentOption optionB, out IEnumerable<double> results)
    {
      results = Array.Empty<double>();

      if(!exp.TryGetRawSols(optionA, out var resultsA) || !exp.TryGetRawSols(optionB, out var resultsB))
      {
        Console.WriteLine("Stopping Command");
        return false;
      }

      var normalizeCmd = new NormalizeValues(resultsA, resultsB);
      normalizeCmd.Execute();

      if(!normalizeCmd.args.IsValid())
      {
        // TODO: report
        Console.WriteLine("Normalized values is invalid");
        return false;
      }

      results = normalizeCmd.args.values;

      return results.Any();
    }


    public static bool TryGetSols(this IExplorer exp, ContentOption optionA, ContentOption optionB, List<int> indexes, out IEnumerable<double> results)
    {
      results = Array.Empty<double>();

      if(!exp.TryGetRawSols(optionA, out var resultsA) || !exp.TryGetRawSols(optionB, out var resultsB))
      {
        Console.WriteLine("Stopping Command");
        return false;
      }

      var filteredResultsA = new GetValuesFromPointListCommand(resultsA.ToList(), indexes);
      filteredResultsA.Execute();
      if(!filteredResultsA.args.IsValid())
      {
        Console.WriteLine(filteredResultsA.args.Message);
        return false;
      }

      var filteredResultsB = new GetValuesFromPointListCommand(resultsB.ToList(), indexes);
      filteredResultsB.Execute();
      
      if(!filteredResultsB.args.IsValid())
      {
        Console.WriteLine(filteredResultsB.args.Message);
        return false;
      }
      

      var normalizeCmd = new NormalizeValues(filteredResultsA.args.values, filteredResultsB.args.values);
      normalizeCmd.Execute();

      if(!normalizeCmd.args.IsValid())
      {
        // TODO: report
        Console.WriteLine("Normalized values is invalid");
        return false;
      }

      results = normalizeCmd.args.values;

      return results.Any();
    }

    /// <summary>
    ///  Get the values
    /// </summary>
    /// <param name="exp"></param>
    /// <param name="results"></param>
    /// <param name="optionA"></param>
    /// <returns></returns>
    public static bool TryGetRawSols(this IExplorer exp, ContentOption optionA, out IEnumerable<int> results)
    {
      results = Array.Empty<int>();

      // TODO: this needs to be cleaned up :(
      var getValueCmdA = new GetValuesFromDataCommand(exp.data, optionA);

      getValueCmdA.Execute();

      if(!getValueCmdA.args.IsValid())
      {
        // TODO: return issue
        Console.WriteLine("Input was invalid\n"+$"Input A={getValueCmdA.args.Message}");
        return false;
      }

      results = getValueCmdA.args.values;
      return results.Any();
    }

    //
    // public static bool TryCompareSols(this IExplorer exp, ContentOption optionA, ContentOption optionB, out IEnumerable<double> results)
    // {
    //   results = Array.Empty<double>();
    //
    //   // TODO: this needs to be cleaned up :(
    //   var getValueCmdA = new GetValuesFromDataCommand(exp.data, optionA);
    //   var getValueCmdB = new GetValuesFromDataCommand(exp.data, optionB);
    //
    //   getValueCmdA.Execute();
    //   getValueCmdB.Execute();
    //
    //   if(!getValueCmdA.args.IsValid() || !getValueCmdB.args.IsValid())
    //   {
    //     // TODO: return issue
    //     Console.WriteLine("Input was invalid\n" +
    //                       $"Input A={getValueCmdA.args.Message}\n" +
    //                       $"Input B={getValueCmdB.args.Message}\n");
    //     return false;
    //   }
    //
    //   var normalizeCmd = new NormalizeValues(getValueCmdA.args.values, getValueCmdB.args.values);
    //   normalizeCmd.Execute();
    //
    //   if(!normalizeCmd.args.IsValid())
    //   {
    //     // TODO: report
    //     Console.WriteLine("Normalized values is invalid");
    //     return false;
    //   }
    //
    //   results = normalizeCmd.args.values;
    //
    //   return results.Any();
    // }
    //


    /// <summary>
    /// Check if this option is already being used and sets the <see cref="ExplorerMetaData.activeTarget"/>
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="option">the option to use</param>
    /// <param name="addToList">if true it adds the option to the <see cref="ExplorerMetaData.activeOptions"/></param>
    static void TrySetOption(this IExplorer obj, ContentOption option, bool addToList = false)
    {
      if(option?.target == null || !option.target.ViewId.Valid() || option.content == null || !option.content.ViewId.Valid())
      {
        return;
      }

      if(!addToList)
      {
        obj.meta.activeOptions = new List<ContentOption>();
      }

      if(!obj.meta.activeOptions.Any(x => x.stage == option.stage
                                          && x.target.ViewId.Equals(option.target.ViewId)
                                          && x.content.ViewId.Equals(option.content.ViewId)))
      {
        obj.meta.activeOptions.Add(option);
      }

      obj.meta.activeTarget = option.target;

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



    public static bool InRange(this IExploreRange obj, double value)
    {
      return value>=obj.min && value<=obj.max;
    }

  }

}
