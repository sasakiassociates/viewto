using System;
using System.Collections.Generic;
using ViewObjects.Clouds;
using ViewObjects.Contents;
using ViewObjects.Systems.Layouts;

namespace ViewObjects.Systems;

[Serializable]
public class RigParameters
{
  public RigParameters(List<string> clouds, List<ViewColor> colors, List<ILayout> viewer)
  {
    Clouds = clouds;
    Colors = colors;
    Viewer = viewer;
  }

  public List<ILayout> Viewer { get; set; }

  /// <summary>
  ///   The lists of <see cref="IViewCloud" /> by <see cref="IViewCloud.ViewId" /> associated with the args
  /// </summary>
  public List<string> Clouds { get; }

  /// <summary>
  ///   List of colors to use for run time analysis
  /// </summary>
  public List<ViewColor> Colors { get; }
}
