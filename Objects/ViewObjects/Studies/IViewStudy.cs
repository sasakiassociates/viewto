using System.Collections.Generic;
using ViewObjects.Common;

namespace ViewObjects.Studies
{

  /// <summary>
  ///   An interface for organizing <typeparamref name="TObject" /> types processing view studies
  /// </summary>
  /// <typeparam name="TObject"></typeparam>
  public interface IViewStudy<TObject> : INameable, IId where TObject : IViewObject
  {
    /// <summary>
    ///   A list of <typeparamref name="TObject" /> objects to group in a study
    /// </summary>
    public List<TObject> Objects { get; set; }

  }

  public interface IViewStudy : IViewStudy<IViewObject>
  { }

}
