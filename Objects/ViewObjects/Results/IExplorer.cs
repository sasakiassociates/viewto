using System.Collections.Generic;
using ViewObjects.Clouds;
using ViewObjects.Common;
using ViewObjects.Contents;
using ViewObjects.Studies;

namespace ViewObjects.Results;

public interface IExplorer : IValidate
{
  /// <summary>
  ///   The active view study being used with the source cloud.
  /// </summary>
  public IViewStudy Source { get; }

  /// <summary>
  ///   The heart and soul of the data being explored
  /// </summary>
  public IResultCloud Cloud { get; }

  /// <summary>
  ///   Set of data settings for the explorer to use
  /// </summary>
  public ExplorerSettings Settings { get; set; }

  /// <summary>
  ///   Container for result values being explored
  /// </summary>
  public List<IResultCloudData> Data { get; }

  // /// <summary>
  // ///   List of options to use for fetching values from <see cref="IExplorer" />. Multiple options will combine the values
  // /// </summary>
  // public List<ContentOption> Options { get; }

  /// <summary>
  /// </summary>
  public ContentInfo ActiveContent { get; set; }

  /// <summary>
  ///   Load in a new view study for the explorer to explore!
  /// </summary>
  /// <param name="viewObj">The view study to load in</param>
  public void Load(IViewStudy viewObj);

}
