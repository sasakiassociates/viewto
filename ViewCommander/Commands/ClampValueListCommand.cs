using System;
using System.Collections.Generic;
using System.Linq;
using ViewTo.Values;

namespace ViewTo.Cmd;

public class ClampValueListCommand<T> : ICmdWithArgs<ValuesRawExplorerArgs<T>> where T : IComparable<T>
{
  readonly T[] _data;
  readonly T _max;
  readonly T _min;
  readonly MathProvider<T> _provider;

  public ValuesRawExplorerArgs<T> args {get;private set;}

  public ClampValueListCommand(MathProvider<T> provider, IEnumerable<T> data, T min, T max)
  {
    this._min = min;
    this._max = max;
    this._data = data == null ? Array.Empty<T>() : data.ToArray();
    this._provider = provider;
  }

  public void Execute()
  {
    if(_data == null || !_data.Any())
    {
      args = new ValuesRawExplorerArgs<T>("No data found to use");
      return;
    }

    if(_provider == null)
    {
      args = new ValuesRawExplorerArgs<T>("No provider found to use. Create a provider first before running the command");
      return;
    }
    T[] result = new T[_data.Length];

    for(int i = 0; i<_data.Length; i++) result[i] = _provider.Clamp(val: _data[i], min: _min, max: _max);

    args = new ValuesRawExplorerArgs<T>(result, $"Data clamped between {_min} to {_max}");
  }

}
