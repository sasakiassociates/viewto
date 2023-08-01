using System;

namespace ViewObjects.Contents
{

  [Serializable]
  public class ContentOption : IContentOption
  {

    public ContentOption()
    { }

    public ContentOption(IContentInfo target, IContentInfo value, ViewContentType stage)
    {
      this.target = target;
      this.content = value;
      this.stage = stage;

    }

    /// <inheritdoc />
    public IContentInfo target { get; }
    /// <inheritdoc />
    public IContentInfo content { get; }
    /// <inheritdoc />
    public ViewContentType stage { get; }
  }

}
