using System.Collections.Generic;

namespace ViewObjects.Common;

public interface IStreamReference
{
  /// <summary>
  ///   A list of reference ids to use for connection speckle data into view to
  /// </summary>
  public List<string> References { get; }
}
