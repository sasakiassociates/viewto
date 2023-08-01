using System.Collections.Generic;
using System.Linq;

namespace ViewTo.Cmd
{

  /// <summary>
  ///   <para>Normalizes two sets of a values</para>
  /// </summary>
  internal class NormalizeValueByListCommand : ICmdWithArgs<ValuesForExplorerArgs>
  {

    /// <summary>
    ///   optional value to use when normalized values would be invalid
    /// </summary>
    readonly double _invalidValue;

    /// <summary>
    ///   Minimum value to apply during normalizing
    /// </summary>
    readonly double _min;
    /// <summary>
    ///   Dividend values
    /// </summary>
    readonly IReadOnlyList<int> _value;

    /// <summary>
    ///   Divisor values
    /// </summary>
    readonly IReadOnlyList<int> _max;

    /// <summary>
    /// </summary>
    /// <param name="valueA">The dividend value to use</param>
    /// <param name="valueB">The divisor value to use</param>
    /// <param name="min">The minimum value to use when normalizing</param>
    /// <param name="invalidValue">optional value to use when <paramref name="valueB" /> is 0 </param>
    public NormalizeValueByListCommand(IEnumerable<int> valueA, IEnumerable<int> valueB, double min = 0.0, double invalidValue = -1)
    {
      this._value = valueA.ToArray();
      this._max = valueB.ToArray();
      this._min = min;
      this._invalidValue = invalidValue;
    }

    public ValuesForExplorerArgs args {get;private set;}

    public void Execute()
    {
      if(_value == null || _max == null || _value.Count != _max.Count)
      {
        return;
      }

      var values = new double[_value.Count];

      for(var i = 0; i<values.Length; i++)
      {
        values[i] = _max[i] == 0 ? _invalidValue : (_value[i]-_min)/(_max[i]-_min);
      }

      args = new ValuesForExplorerArgs(values, $"Found values! {values.Length}");
    }
  }

}
