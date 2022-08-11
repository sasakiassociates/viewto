using System.Collections.Generic;
using Speckle.Core.Models;

namespace ViewObjects.Speckle
{

  public class ContentBundleBase : ViewObjBase, IValidate
  {

    public ContentBundleBase()
    { }

    [DetachProperty]
    public List<TargetContentBase> targets { get; set; }

    [DetachProperty]
    public List<BlockerContentBase> blockers { get; set; }

    [DetachProperty]
    public List<DesignContentBase> designs { get; set; }

    public bool isValid => targets.Valid() && blockers.Valid();
  }
}
