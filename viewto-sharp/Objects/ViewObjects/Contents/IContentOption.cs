namespace ViewObjects.Contents
{

  
  public interface IContentOption
  {
    /// <summary>
    /// The <see cref="IContentInfo"/> target object that the analysis is related too
    /// </summary>
    public IContentInfo target { get; }

    /// <summary>
    /// The <see cref="IContentInfo"/> object that the view analysis is staged in
    /// </summary>
    public IContentInfo content { get; }

    /// <summary>
    /// A very poor way of keeping track of the content object stage completed
    /// </summary>
    public ViewContentType stage { get; }
  }

}
