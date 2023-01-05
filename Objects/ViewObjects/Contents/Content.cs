using System;
using System.Collections.Generic;
using ViewObjects.Common;
using ViewObjects.Studies;

namespace ViewObjects.Contents;

/// <summary>
/// A standard content object used to organized the different geometry of a <see cref="ViewStudy"/>.
/// </summary>
public class Content : IViewObject, IContent, IContentObjects<object>
{

  /// <summary>
  /// Creates a new content object with a randomize <see cref="ViewId"/>
  /// </summary>
  public Content()
  {
    ViewId = Guid.NewGuid().ToString();
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
  public Content(ContentType type, string viewId = null, string viewName = null)
  {
    ContentType = type;
    ViewName = viewName;
    ViewId = ObjUtils.CheckIfValidId(viewId);
  }

  /// <inheritdoc />
  public string ViewId { get; }

  /// <inheritdoc />
  public string ViewName { get; set; }

  /// <inheritdoc />
  public ContentType ContentType { get; }

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
    return obj != default(object) && ViewId.Equals(obj.ViewId) && ContentType == obj.ContentType;
  }
}
