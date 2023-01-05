using System.Collections.Generic;
using System.Linq;

namespace ViewTo.Cmd
{

  /// <summary>
  ///   <para>Normalizes two sets of a values</para>
  /// </summary>
  internal class NormalizeValues : ICmdWithArgs<ValuesForExplorerArgs>
  {

    /// <summary>
    ///   optional value to use when normalized values would be invalid
    /// </summary>
    readonly double invalidValue;

    /// <summary>
    ///   Minimum value to apply during normalizing
    /// </summary>
    readonly double min;
    /// <summary>
    ///   Dividend values
    /// </summary>
    readonly IReadOnlyList<int> valueA;

    /// <summary>
    ///   Divisor values
    /// </summary>
    readonly IReadOnlyList<int> valueB;

    /// <summary>
    /// </summary>
    /// <param name="valueA">The dividend value to use</param>
    /// <param name="valueB">The divisor value to use</param>
    /// <param name="min">The minimum value to use when normalizing</param>
    /// <param name="invalidValue">optional value to use when <paramref name="valueB" /> is 0 </param>
    public NormalizeValues(IEnumerable<int> valueA, IEnumerable<int> valueB, double min = 0.0, double invalidValue = -1)
    {
      this.valueA = valueA.ToArray();
      this.valueB = valueB.ToArray();
      this.min = min;
      this.invalidValue = invalidValue;
    }

    public ValuesForExplorerArgs args { get; private set; }

    public void Execute()
    {
      if(valueA == null || valueB == null || valueA.Count != valueB.Count)
      {
        return;
      }

      var values = new double[valueA.Count];

      for(var i = 0; i < values.Length; i++)
      {
        values[i] = valueB[i] == 0 ? invalidValue : (valueA[i] - min) / (valueB[i] - min);
      }

      args = new ValuesForExplorerArgs(values, $"Found values! {values.Length}");
    }
  }

}
