using System.Collections.Generic;
using System.Linq;
using ViewObjects.Clouds;
using ViewObjects.Contents;

namespace ViewTo.Cmd;

/// <summary>
/// Retrieves a list of values from a list of <seealso cref="IResultCloudData"/>. The values are selected by the input parameter of <seealso cref="IContentOption"/>
/// </summary>
internal class ValueFromOption : ICmdWithArgs<ValuesRawForExplorerArgs>
{

  readonly IContentOption _option;
  readonly IReadOnlyCollection<IResultCloudData> _data;
  
  public ValueFromOption(IReadOnlyCollection<IResultCloudData> data, IContentOption option)
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

    IResultCloudData dataFound = default;

    foreach(var d in _data)
    {
      if(_option.content.ViewId.Equals(d.info.content.ViewId)
         && _option.target.ViewId.Equals(d.info.target.ViewId)
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
    return$"{o.target.ViewName}({nameof(o.target)})={o.target.type} : {o.target.ViewId}\n" +
          $"{o.content.ViewName}({nameof(o.content)})={o.content.type} : {o.content.ViewId}\n" +
          $"({nameof(o.content)})={o.stage}\n";
  }



}
