using ViewObjects.Common;

namespace ViewObjects.Clouds;

public interface IViewCloud : IId
{
  /// <summary>
  ///   The cloud of points to use
  /// </summary>
  public CloudPoint[] Points { get; set; }
}
