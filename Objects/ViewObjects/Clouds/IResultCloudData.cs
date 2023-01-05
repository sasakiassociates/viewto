using System.Collections.Generic;

namespace ViewObjects.Clouds;

/// <summary>
///   The main structure for organizing result data
/// </summary>
public interface IResultCloudData : IResultCloudMetaData
{

  /// <summary>
  ///   the raw values gathered
  /// </summary>
  List<int> Values { get; set; }
}
