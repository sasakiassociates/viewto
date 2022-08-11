using System.Collections.Generic;
using Speckle.Core.Models;
using Speckle.Newtonsoft.Json;

namespace ViewObjects.Speckle
{

  public abstract class ViewContentBase : ViewObjBase, IValidate, INameable
  {

    public ViewContentBase()
    { }
    
    [DetachProperty]
    public List<Base> objects { get; set; }

    [JsonIgnore]
    public virtual bool isValid => objects != null;

    public string viewName { get; set; }
  }
}
