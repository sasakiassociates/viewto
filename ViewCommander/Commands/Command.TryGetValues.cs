using System.Collections.Generic;
using System.Linq;
using ViewObjects;
using ViewObjects.Clouds;

namespace ViewTo.Cmd
{

  /// <summary>
  ///   <para>Searches through a list of <see cref="IResultCloudData" /> to find a given object with a matching id and stage</para>
  /// </summary>
  internal class TryGetValues : ICmdWithArgs<ValuesRawForExplorerArgs>
  {
    /// <summary>
    ///   id from the content
    /// </summary>
    readonly string _contentId;

    /// <summary>
    ///   the list of data to search through
    /// </summary>
    readonly IReadOnlyCollection<IResultCloudData> _data;

    /// <summary>
    ///   stage the data is linked with
    /// </summary>
    readonly ViewContentType _stage;

    public ValuesRawForExplorerArgs args { get; private set; }

    /// <summary>
    /// </summary>
    /// <param name="data">data to search through</param>
    /// <param name="contentId">id of the content to find</param>
    /// <param name="stage">the analysis stage to find</param>
    public TryGetValues(IReadOnlyCollection<IResultCloudData> data, string contentId, ViewContentType stage)
    {
      this._data = data;
      this._stage = stage;
      this._contentId = contentId;
    }

    public void Execute()
    {
      if(string.IsNullOrEmpty(_contentId))
      {
        args = new ValuesRawForExplorerArgs("Content ID does is not valid");
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
        if(d.info.target.ViewId.Equals(_contentId) && d.info.target.type == _stage)
        {
          dataFound = d;
          break;
        }
      }

      if(dataFound == default(object))
      {
        args = new ValuesRawForExplorerArgs($"No id found in the data set. Input id={_contentId}");
        return;
      }

      args = new ValuesRawForExplorerArgs(dataFound.values, $"Data found for {_contentId} with {dataFound.values.Count} ");
    }
  }

}
