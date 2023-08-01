using System;
using System.Collections.Generic;
using ViewObjects.Clouds;

namespace ViewObjects.References
{

  [Serializable]
  public class ViewCloudReference : ViewObjectReference<ViewCloud>
  {
    /// <inheritdoc />
    public ViewCloudReference(ViewCloud obj, List<string> references) : base(obj, references)
    { }

    /// <inheritdoc />
    public ViewCloudReference(List<string> references, string viewId, string viewName = null) : base(references, viewId, viewName)
    { }
  }

}
