using System.Collections.Generic;
using ViewObjects.Systems.Layouts;

namespace ViewObjects.Systems;

public interface IViewerLinked : IViewer
{
  /// <summary>
  ///   A list of cloud ids that can be used with this bundle
  /// </summary>
  public List<string> Clouds { get; set; }
}

public interface IViewerLinked<TLayout> : IViewer<TLayout> where TLayout : ILayout
{
  /// <summary>
  ///   A list of cloud ids that can be used with this bundle
  /// </summary>
  public List<string> Clouds { get; set; }
}
