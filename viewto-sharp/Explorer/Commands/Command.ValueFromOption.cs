using System.Collections.Generic;
using System.Linq;
using ViewObjects.Clouds;
using ViewObjects.Contents;

namespace ViewTo.Cmd;

/// <summary>
/// Retrieves a list of values from a list of <seealso cref="IResultLayer"/>. The values are selected by the input parameter of <seealso cref="IResultCondition"/>
/// </summary>
internal class ValueFromOption : ICmdWithArgs<ValuesRawForExplorerArgs>
{

  readonly IResultCondition _option;
  readonly IReadOnlyCollection<IResultLayer> _data;

  public ValueFromOption(IReadOnlyCollection<IResultLayer> data, IResultCondition option)
  {
    this._data = data;
    this._option = option;
  }

  public ValuesRawForExplorerArgs args { get; private set; }

  public void Execute()
  {
    if(_option?.focus == null || _option.obstruct == null)
    {
      args = new ValuesRawForExplorerArgs($"Input parameters, {typeof(IResultCondition)}, is not valid");
      return;
    }

    if(_data == null || !_data.Any())
    {
      args = new ValuesRawForExplorerArgs("No data found to use");
      return;
    }

    IResultLayer dataFound = default;

    foreach(var d in _data)
    {
      if(_option.obstruct.appId.Equals(d.info.content.guid)
         && _option.focus.appId.Equals(d.info.target.guid)
         && _option.stage == d.info.stage)
      {
        dataFound = d;
        break;
      }
    }


    args = dataFound == default(object) ?
      args = new ValuesRawForExplorerArgs($"No id found in the data set\n{ToStringHack(_option)}")
      :
      args = new ValuesRawForExplorerArgs(dataFound.values, $"Data found!({dataFound.values.Count})\n{ToStringHack(_option)}");
  }

  static string ToStringHack(IResultCondition o)
  {
    return$"{o.focus.name}({nameof(o.focus)})={o.focus.contentType} : {o.focus.appId}\n" +
          $"{o.obstruct.name}({nameof(o.obstruct)})={o.obstruct.contentType} : {o.obstruct.appId}\n" +
          $"({nameof(o.obstruct)})={o.stage}\n";
  }

}
