using Sasaki.Common;

namespace ViewObjects.Contents
{

  /// <summary>
  /// Interface for passing around view content data
  /// </summary>
  public interface IContentInfo : IHaveName, IHaveId
  {

    public ViewContentType contentType { get; }

  }

}
