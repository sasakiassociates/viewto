using System.Collections.Generic;
using System.Linq;

namespace ViewTo.Cmd;

internal class NormalizeValues : ICmdWithArgs<ValuesForExplorerArgs>
{

  readonly double _min;

  readonly double _max;

  readonly IReadOnlyList<int> _value;

  /// <summary>
  /// </summary>
  /// <param name="value">The dividend value to use</param>
  /// <param name="max">The divisor value to use</param>
  /// <param name="min">The minimum value to use when normalizing</param>
  public NormalizeValues(IEnumerable<int> value, double max = 1.0, double min = 0.0)
  {
    this._value = value.ToArray();
    this._max = max;
    this._min = min;
  }

  public ValuesForExplorerArgs args {get;private set;}

  public void Execute()
  {
    if(_value == null)
    {
      return;
    }
    

    var values = new double[_value.Count];

    for(var i = 0; i<values.Length; i++) values[i] = (_value[i]-_min)/(_max-_min);

    args = new ValuesForExplorerArgs(values: values, message: $"Found values! {values.Length}");
  }
}
