using System.Collections.Generic;
using Speckle.Newtonsoft.Json;

namespace ViewObjects.Speckle
{

  public class ViewerBundleBase : ViewObjBase, IViewerBundle
  {

    public ViewerBundleBase()
    { }

    [JsonIgnore]
    public virtual bool isValid => layouts.Valid() && layouts.Valid();
    public List<IViewerLayout> layouts { get; set; }
  }
}
