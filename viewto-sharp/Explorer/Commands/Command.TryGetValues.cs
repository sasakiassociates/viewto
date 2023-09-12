using System.Collections.Generic;
using System.Linq;
using ViewObjects;
using ViewObjects.Clouds;
using ViewObjects.Contents;

namespace ViewTo.Cmd
{

  /// <summary>
  ///   <para>Searches through a list of <see cref="IResultLayer" /> to find a given object with a matching id and stage</para>
  /// </summary>
  internal class GetValuesFromDataCommand : ICmdWithArgs<ValuesRawForExplorerArgs>
  {
    /// <summary>
    ///   id from the content
    /// </summary>
    readonly string _targetId, _contentId;

    /// <summary>
    ///   the list of data to search through
    /// </summary>
    readonly IReadOnlyCollection<IResultLayer> _data;

    /// <summary>
    ///   stage the data is linked with
    /// </summary>
    readonly ViewContextType _stage;

    public ValuesRawForExplorerArgs args {get;private set;}

    /// <summary>
    /// Finds a data set that matches the <seealso cref="IRIResultCondition.focus> id 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="option"></param>
    public GetValuesFromDataCommand(IReadOnlyCollection<IResultLayer> data, IResultCondition option)
    {
      this._data = data;
      this._stage = option.stage;
      this._targetId = option.focus.appId;
      this._contentId = option.obstruct.appId;
    }

    public void Execute()
    {
      if(string.IsNullOrEmpty(_targetId))
      {
        args = new ValuesRawForExplorerArgs("Content ID does is not valid");
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
        if(d.info.target.guid.Equals(_targetId) &&
           d.info.content.guid.Equals(_contentId) &&
           d.info.stage == _stage)
        {
          dataFound = d;
          break;
        }
      }

      if(dataFound == default(object))
      {
        args = new ValuesRawForExplorerArgs($"No id found in the data set. Input id={_targetId}");
        return;
      }

      args = new ValuesRawForExplorerArgs(dataFound.values, $"Data found for {_targetId} with {dataFound.values.Count} ");
    }
  }

}
