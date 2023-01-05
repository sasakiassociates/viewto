using System.Collections.Generic;

namespace ViewObjects.Systems.Layouts;

public interface ILayout
{
  /// <summary>
  ///   Setup of viewers to use with each layout type
  /// </summary>
  public List<ViewDirection> Viewers { get; }
}
