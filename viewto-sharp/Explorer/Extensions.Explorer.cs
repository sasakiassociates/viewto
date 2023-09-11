using System;
using System.Collections.Generic;
using System.Linq;
using ViewObjects;
using Sasaki.Common;
using ViewObjects.Contents;
using ViewObjects.Results;
using ViewTo.Cmd;
using ViewTo.Values;
using static System.Int64;

namespace ViewTo
{

  public interface IExplorerCommand
  { }

  public static partial class ViewCoreExtensions
  {

    public const int MAX_PIXEL_COUNT = 2147483647;
    
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

      if(!exp.TryGetRawViews(optionA, out var resultsA) || !exp.TryGetRawViews(optionB, out var resultsB))
      {
        Console.WriteLine("Stopping Command");
        return false;
      }

      var normalizeCmd = new NormalizeValueByListCommand(resultsA, resultsB);
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
    /// Retrieve all raw view data, the total count of pixels.
    /// This will clamp the pixel values with <see cref="pixelRange"/> 
    /// </summary>
    /// <param name="exp"></param>
    /// <param name="valueOption">the option to find</param>
    /// <param name="pixelRange">by default this will be set with a min of 0 and max of 2147483647</param>
    /// <returns></returns>
    public static int[] GetViewsRaw(this IExplorer exp, ContentOption valueOption, IntRange pixelRange)
    {
      var results = Array.Empty<int>();

      pixelRange ??= new IntRange {max = 2147483647, min = 0};

      if(!exp.TryGetRawViews(optionA: valueOption, results: out var raw))
      {
        Console.WriteLine("Stopping Command");
        return results;
      }

      var clampedCmd = new ClampValueListCommand<int>(provider: new IntMath(), data: raw, min: pixelRange.min, max: pixelRange.max);
      clampedCmd.Execute();

      if(!clampedCmd.args.IsValid())
      {
        Console.WriteLine(clampedCmd.args.Message);
        return results;
      }

      results = clampedCmd.args.values.ToArray();
      return results;
    }

    /// <summary>
    /// Retrieve all raw view data, the total count of pixels. This will also clamp the pixel values and filter them by index 
    /// </summary>
    /// <param name="exp"></param>
    /// <param name="valueOption">the option to find</param>
    /// <param name="pixelRange">by default this will be set with a min of 0 and max of 2147483647</param>
    /// <param name="indexes">A list of indexes to filter by</param>
    /// <returns></returns>
    public static int[] GetViewsRaw(this IExplorer exp, ContentOption valueOption, IntRange pixelRange, int[] indexes)
    {
      var results = exp.GetViewsRaw(valueOption: valueOption, pixelRange: pixelRange);

      if(!indexes.Valid()) return results;

      var valuesFiltered = new GetValuesFromPointListCommand(data: results, index: indexes);
      valuesFiltered.Execute();

      if(!valuesFiltered.args.IsValid())
      {
        Console.WriteLine(valuesFiltered.args.Message);
        return results;
      }

      return valuesFiltered.args.values.ToArray();
    }


    public static IEnumerable<double> GetSols(this IExplorer exp, ContentOption valueOption, ContentOption maxOption, ExplorerFilterInput filter, bool normalizeByFilter = false)
    {
      var results = Array.Empty<double>();
      filter ??= new ExplorerFilterInput();

      var values = exp.GetViewsRaw(valueOption: valueOption, pixelRange: filter.pixelRange, indexes: filter.indexes);
      var maxies = exp.GetViewsRaw(valueOption: maxOption, pixelRange: filter.pixelRange, indexes: filter.indexes);

      if(values == null || maxies == null) return results;

      if(normalizeByFilter)
      {

        var max = maxies.Max();
        var min = values.Min();

        var normalizeCmd = new NormalizeValues(value: values, max: max, min: min);
        normalizeCmd.Execute();

        if(!normalizeCmd.args.IsValid())
        {
          // TODO: report
          Console.WriteLine("Normalized values is invalid");
          return results;
        }
        results = normalizeCmd.args.values.ToArray();
      }
      else
      {
        var normalizeCmd = new NormalizeValueByListCommand(valueA: values, valueB: maxies);
        normalizeCmd.Execute();

        if(!normalizeCmd.args.IsValid())
        {
          // TODO: report
          Console.WriteLine("Normalized values is invalid");
          return results;
        }

        results = normalizeCmd.args.values.ToArray();
      }

      return results;
    }


    /// <summary>
    /// Creates a normalize list form  two sets of values from and filters out a given set of indexes.
    /// </summary>
    /// <param name="exp"></param>
    /// <param name="valueOption">The value that will be normalize by</param>
    /// <param name="maxOption">The option that will act as the max value to normalize by</param>
    /// <param name="indexes">A list of point indexes to filter by</param>
    /// <param name="normalizeByFilter">When false this will use the list provided by the <see cref="maxOption"/> as the normalizing value
    /// If set to true it will find the max and min values from the list of values from <see cref="maxOption"/> after being filtered by <see cref="indexes"/></param>
    /// <returns></returns>
    public static IEnumerable<double> GetSols(this IExplorer exp, ContentOption valueOption, ContentOption maxOption, List<int> indexes, bool normalizeByFilter = true)
    {
      var results = Array.Empty<double>();

      if(!exp.TryGetRawViews(valueOption, out var valueRaw) || !exp.TryGetRawViews(maxOption, out var maxRaw))
      {
        Console.WriteLine("Stopping Command");
        return results;
      }

      int[] values = null;
      int[] maxies = null;

      if(indexes.Valid())
      {

        var valuesFiltered = new GetValuesFromPointListCommand(valueRaw.ToList(), indexes);
        valuesFiltered.Execute();

        if(!valuesFiltered.args.IsValid())
        {
          Console.WriteLine(valuesFiltered.args.Message);
          return results;
        }


        var maxFiltered = new GetValuesFromPointListCommand(maxRaw.ToList(), indexes);
        maxFiltered.Execute();

        if(!maxFiltered.args.IsValid())
        {
          Console.WriteLine(maxFiltered.args.Message);
          return results;
        }

        values = valuesFiltered.args.values.ToArray();
        maxies = maxFiltered.args.values.ToArray();
      }
      else
      {
        values = valueRaw.ToArray();
        maxies = maxRaw.ToArray();

      }

      if(normalizeByFilter)
      {

        var max = 0.0;
        var min = 1.0;
        foreach(var v in maxies)
        {
          if(v<0 || double.IsNaN(v)) continue;

          if(v<min) min = v;
          if(v>max) max = v;
        }
        var normalizeCmd = new NormalizeValues(value: values, max: max, min: min);
        normalizeCmd.Execute();
        if(!normalizeCmd.args.IsValid())
        {
          // TODO: report
          Console.WriteLine("Normalized values is invalid");
          return results;
        }
        results = normalizeCmd.args.values.ToArray();
      }
      else
      {
        var normalizeCmd = new NormalizeValueByListCommand(values, maxies);
        normalizeCmd.Execute();

        if(!normalizeCmd.args.IsValid())
        {
          // TODO: report
          Console.WriteLine("Normalized values is invalid");
          return results;
        }

        results = normalizeCmd.args.values.ToArray();
      }

      return results;
    }

    /// <summary>
    ///  Get the values
    /// </summary>
    /// <param name="exp"></param>
    /// <param name="results"></param>
    /// <param name="optionA"></param>
    /// <returns></returns>
    public static bool TryGetRawViews(this IExplorer exp, ContentOption optionA, out IEnumerable<int> results)
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
      if(option?.target == null || !option.target.appId.Valid() || option.content == null || !option.content.appId.Valid())
      {
        return;
      }

      if(!addToList)
      {
        obj.meta.activeOptions = new List<ContentOption>();
      }

      if(!obj.meta.activeOptions.Any(x => x.stage == option.stage
                                          && x.target.appId.Equals(option.target.appId)
                                          && x.content.appId.Equals(option.content.appId)))
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
