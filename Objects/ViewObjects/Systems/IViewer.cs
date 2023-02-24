using System.Collections.Generic;
using ViewObjects.Studies;
using ViewObjects.Systems.Layouts;

namespace ViewObjects.Systems
{

  /// <summary>
  ///   An interface for organizing <typeparamref name="TLayout" /> types for <see cref="IViewStudy" />
  /// </summary>
  /// <typeparam name="TLayout"></typeparam>
  public interface IViewer<TLayout> where TLayout : ILayout
  {
    /// <summary>
    ///   The group of <typeparamref name="TLayout" /> targeted to be used during the analysis
    /// </summary>
    public List<TLayout> Layouts { get; set; }
  }

  public interface IViewer : IViewer<ILayout>
  { }

}
