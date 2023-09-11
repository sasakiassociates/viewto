using System.Collections.Generic;
using System.Linq;
using ViewObjects.Clouds;
using ViewObjects.Contents;

namespace ViewTo.Cmd;

/// <summary>
/// Retrieves a list of values from a list of <seealso cref="IResultLayer"/>. The values are selected by the input parameter of <seealso cref="IContentOption"/>
/// </summary>
internal class ValueFromOption : ICmdWithArgs<ValuesRawForExplorerArgs>
{

  readonly IContentOption _option;
  readonly IReadOnlyCollection<IResultLayer> _data;

  public ValueFromOption(IReadOnlyCollection<IResultLayer> data, IContentOption option)
  {
    this._data = data;
    this._option = option;
  }

  public ValuesRawForExplorerArgs args { get; private set; }

  public void Execute()
  {
    if(_option?.target == null || _option.content == null)
    {
      args = new ValuesRawForExplorerArgs($"Input parameters, {typeof(IContentOption)}, is not valid");
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
      if(_option.content.appId.Equals(d.info.content.guid)
         && _option.target.appId.Equals(d.info.target.guid)
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

  static string ToStringHack(IContentOption o)
  {
    return$"{o.target.name}({nameof(o.target)})={o.target.contentType} : {o.target.appId}\n" +
          $"{o.content.name}({nameof(o.content)})={o.content.contentType} : {o.content.appId}\n" +
          $"({nameof(o.content)})={o.stage}\n";
  }

}
