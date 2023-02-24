using System.Collections.Generic;
using ViewObjects.Contents;
using ViewObjects.Systems.Layouts;

namespace ViewObjects.Clouds
{

  /// <summary>
  ///   The main structure for organizing result data
  /// </summary>
  public interface IResultCloudData : IResultCloudMetaData
  {
    /// <summary>
    ///   the raw values gathered
    /// </summary>
    List<int> values { get; }
  }

  /// <summary>
  ///   The meta data associated with the result values
  /// </summary>
  public interface IResultCloudMetaData
  {
    /// <summary>
    /// <para>The <see cref="IContentOption" /> associated with these results.</para>
    /// <para>This gives reference to the target content used along with the associated stage it ran</para> 
    /// </summary>
    public IContentOption info { get; }

    /// <summary>
    ///   The <see cref="ILayout" /> used to gather the data
    /// </summary>
    public string layout { get; }
  }

}
