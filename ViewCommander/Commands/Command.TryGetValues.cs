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
    readonly string contentId;

    /// <summary>
    ///   the list of data to search through
    /// </summary>
    readonly IReadOnlyCollection<IResultCloudData> data;

    /// <summary>
    ///   stage the data is linked with
    /// </summary>
    readonly ContentType stage;

    /// <summary>
    /// </summary>
    /// <param name="data">data to search through</param>
    /// <param name="contentId">id of the content to find</param>
    /// <param name="stage">the analysis stage to find</param>
    public TryGetValues(IReadOnlyCollection<IResultCloudData> data, string contentId, ContentType stage)
    {
      this.data = data;
      this.stage = stage;
      this.contentId = contentId;
    }

    public ValuesRawForExplorerArgs args { get; private set; }

    public void Execute()
    {
      if(string.IsNullOrEmpty(contentId))
      {
        args = new ValuesRawForExplorerArgs("Content ID does is not valid");
        return;
      }

      if(data == null || !data.Any())
      {
        args = new ValuesRawForExplorerArgs("No data found to use");
        return;
      }

      IResultCloudData dataFound = default;

      foreach(var d in data)
      {
        if(d.Option.Id.Equals(contentId) && d.Option.Stage == stage)
        {
          dataFound = d;
          break;
        }
      }

      if(dataFound == default(object))
      {
        args = new ValuesRawForExplorerArgs($"No id found in the data set. Input id={contentId}");
        return;
      }

      args = new ValuesRawForExplorerArgs(dataFound.Values, $"Data found for {contentId} with {dataFound.Values.Count} ");
    }
  }

}
