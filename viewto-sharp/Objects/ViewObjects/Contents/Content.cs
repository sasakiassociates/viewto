using System;
using System.Collections.Generic;
using Sasaki.Common;
using ViewObjects.Studies;
using ViewObjects.Common;


namespace ViewObjects.Contents
{

  /// <summary>
  /// A standard content object used to organized the different geometry of a <see cref="ViewStudy"/>.
  /// </summary>
  public class Content : IViewObject, IContent, IContentObjects<object>
  {

    /// <summary>
    /// Creates a new content object with a randomize <see cref="appId"/>
    /// </summary>
    public Content()
    {
      appId = Guid.NewGuid().ToString();
    }

    /// <summary>
    /// Constructs a content for a view study
    /// </summary>
    /// <param name="type">The type of content to label this as</param>
    /// <param name="viewName">Name of the view study</param>
    /// <param name="viewId">
    ///   Id of the view study as a <see cref="System.Guid" />. If no valid value is passed in one will be
    ///   generated
    /// </param>
    public Content(ViewContentType type, string viewId = null, string viewName = null)
    {
      this.contentType = type;
      name = viewName;
      appId = ObjUtils.CheckIfValidId(viewId);
    }

    /// <inheritdoc />
    public string appId { get; }

    /// <inheritdoc />
    public string name { get; set; }

    /// <inheritdoc />
    public ViewContentType contentType { get; }

    /// <inheritdoc />
    public ViewColor Color { get; set; }

    /// <inheritdoc />
    public List<object> Objects { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool Equals(IContent obj)
    {
      return obj != default(object) && appId.Equals(obj.appId) && contentType == obj.contentType;
    }
  }

}
