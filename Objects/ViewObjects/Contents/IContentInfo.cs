using ViewObjects.Common;

namespace ViewObjects.Contents
{

  /// <summary>
  /// Interface for passing around view content data
  /// </summary>
  public interface IContentInfo : INameable, IId
  {
    public ViewContentType type { get; }

  }

}
