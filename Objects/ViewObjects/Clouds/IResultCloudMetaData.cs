using ViewObjects.Contents;
using ViewObjects.Systems.Layouts;

namespace ViewObjects.Clouds;

/// <summary>
///   The meta data associated with the result values
/// </summary>
public interface IResultCloudMetaData
{
  /// <summary>
  ///   The <see cref="IContent" /> associated with these results. Includes the name, id, and stage
  /// </summary>
  public IContentOption Option { get; set; }

  /// <summary>
  ///   The <see cref="ILayout" /> used to gather the data
  /// </summary>
  string Layout { get; set; }
}
