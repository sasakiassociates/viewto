using System.Collections.Generic;
using System.Linq;
using ViewObjects.Clouds;
using ViewObjects.Contents;

namespace ViewTo.Cmd;

internal class GetValuesFromPointListCommand : ICmdWithArgs<ValuesRawForExplorerArgs>
{
  /// <summary>
  ///   the list of data to search through
  /// </summary>
  readonly IReadOnlyList<int> _data;
  readonly IReadOnlyList<int> _index;

  public ValuesRawForExplorerArgs args {get;private set;}


  /// <summary>
  /// Finds a data set that matches the <seealso cref="IContentOption.target"/> id 
  /// </summary>
  /// <param name="data"></param>
  /// <param name="index"></param>
  public GetValuesFromPointListCommand(IReadOnlyList<int> data, IReadOnlyList<int> index)
  {
    this._data = data;
    this._index = index;
  }

  public void Execute()
  {

    if(_data == null || !_data.Any())
    {
      args = new ValuesRawForExplorerArgs("No data found to use");
      return;
    }

    List<int> dataFound = new();

    foreach(var i in _index)
    {
      if(i<0 || i>=_data.Count) continue;
      dataFound.Add(_data[i]);
    }

    args = new ValuesRawForExplorerArgs(dataFound, $"Data found for {_index.Count} indexes ");
  }
}
