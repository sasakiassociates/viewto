using System;
using System.Collections.Generic;
using ViewObjects.Common;

namespace ViewObjects.Studies
{

  public class ViewStudy : IViewStudy, IViewObject
  {

    public ViewStudy()
    {
      objects = new List<IViewObject>();
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
      this.objects = objects;
      ViewName = viewName;
      ViewId = ObjUtils.CheckIfValidId(viewId);
    }

    /// <inheritdoc />
    public string ViewName { get; set; }

    /// <inheritdoc />
    public string ViewId { get; set; }

    /// <inheritdoc />
    public List<IViewObject> objects { get; set; }
  }

}
