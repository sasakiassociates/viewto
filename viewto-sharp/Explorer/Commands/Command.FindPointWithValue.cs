using System;
using System.Collections.Generic;
using System.Linq;
using Sasaki.Common;
using ViewTo.Receivers;

namespace ViewTo.Cmd
{

  /// <summary>
  ///   <para>
  ///     Looks for any values that are nearly equal to <see cref="_valueToFind" /> and returns a random index within the
  ///     searched for values
  ///   </para>
  ///   <para>If no value is found <see cref="PointWithValueArgs" /> returns invalid with a -1 index</para>
  /// </summary>
  internal class FindPointWithValue : ICmdWithArgs<PointWithValueArgs>
  {

    /// <summary>
    ///   difference values to not care about
    /// </summary>
    private readonly double _unimportantDifference;
    /// <summary>
    ///   values to search
    /// </summary>
    private readonly IReadOnlyList<double> _values;

    /// <summary>
    ///   given value to search for
    /// </summary>
    private readonly double _valueToFind;

    /// <summary>
    ///   constructs the command to run
    /// </summary>
    /// <param name="values">values to search</param>
    /// <param name="valueToFind">value to search for</param>
    /// <param name="unimportantDifference">difference in values to not care about when comparing</param>
    public FindPointWithValue(IReadOnlyList<double> values, double valueToFind, double unimportantDifference = 0.0001)
    {
      this._values = values;
      this._valueToFind = valueToFind;
      this._unimportantDifference = unimportantDifference;
    }

    public PointWithValueArgs args { get; private set; }

    public void Execute()
    {
      var res = -1;
      var receiver = new ValueReceiver();

      // if the value being found is trash
      if(double.IsNaN(_valueToFind))
      {
        args = new PointWithValueArgs(res, $"No valid value was used to find. Value={_valueToFind}");
        return;
      }

      // if the values being searched is trash
      if(_values == null || !_values.Any())
      {
        args = new PointWithValueArgs(res, "No valid list of values to search for");
        return;
      }

      var sampleOfValues = new List<int>();

      // compare data 
      for(var i = 0; i < _values.Count; i++)
      {
        if(receiver.NearlyEqual(_values[i], _valueToFind, _unimportantDifference))
        {
          sampleOfValues.Add(i);
        }
      }

      // if no values were found from sample set we keep searching
      if(!sampleOfValues.Valid())
      {
        // if no values were found we look for the nearest values
        var nearest = 1.0;

        for(var i = 0; i < _values.Count; i++)
        {
          var diff = Math.Abs(_values[i] - _valueToFind);
          if(diff < nearest)
          {
            nearest = diff;
            res = i;
          }
        }

        sampleOfValues.Add(res);
      }

      args = new PointWithValueArgs(sampleOfValues[new Random(DateTime.Now.Millisecond).Next(0, sampleOfValues.Count - 1)], "Success!");
    }
  }

}
