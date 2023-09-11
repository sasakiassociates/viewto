using System;
using System.Collections.Generic;

namespace Sasaki.Common
{

  public interface IVersionReference : IHaveId, IHaveName
  {
    /// <summary>
    /// The type of object that is pointing to these versions
    /// </summary>
    public Type type {get;}

    /// <summary>
    ///  A list of version ids to use for connection speckle data into view to
    /// </summary>
    public List<string> references {get;}
  }

}
