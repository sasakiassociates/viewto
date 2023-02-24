using System.Collections.Generic;

namespace ViewObjects.Systems
{

  public interface IRig
  {

    /// <summary>
    ///   Handle building the different viewer types
    /// </summary>
    public void Build();

    /// <summary>
    ///   Handle all the data the rig needs to run a view study
    /// </summary>
    /// <param name="parameters"></param>
    public void Initialize(List<RigParameters> parameters);
  }

}
