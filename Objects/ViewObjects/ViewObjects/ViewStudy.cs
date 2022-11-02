using System;
using System.Collections.Generic;
namespace ViewObjects
{
  public class ViewStudy : IViewStudy, IViewObject
  {

    public ViewStudy()
    {
      Objects = new List<IViewObject>();
      ViewId = Guid.NewGuid().ToString();
    }

    /// <summary>
    ///   Constructs a view study
    /// </summary>
    /// <param name="objects">List of <see cref="IViewObject" /> to use</param>
    /// <param name="viewName">Name of the view study</param>
    /// <param name="viewId">
    ///   Id of the view study as a <see cref="System.Guid" />. If no valid value is passed in one will be
    ///   generated
    /// </param>
    public ViewStudy(List<IViewObject> objects, string viewName, string viewId = null)
    {
      Objects = objects;
      ViewName = viewName;
      ViewId = ObjUtils.CheckIfValidId(viewId);
    }

    /// <inheritdoc />
    public string ViewName { get; set; }

    /// <inheritdoc />
    public string ViewId { get; set; }

    /// <inheritdoc />
    public List<IViewObject> Objects { get; set; }
  }

}
