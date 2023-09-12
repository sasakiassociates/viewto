using Sasaki.ViewObjects;
using System;
using System.Collections.Generic;

namespace Sasaki.ViewObjects;

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

/// <summary>
///   Simple Rig Object to use for constructing a study for analysis
/// </summary>
public class Rig : ViewObject, IRig 
{

  public Rig()
  { }

  public List<RigParameters> StoredObjs {get;protected set;}

  /// <inheritdoc />
  public void Build()
  {
    Console.WriteLine("Building Rig");
  }

  /// <inheritdoc />
  public void Initialize(List<RigParameters> parameters)
  {
    StoredObjs = parameters;
  }
}

[Serializable]
public class RigParameters
{
  public RigParameters(List<string> clouds, List<ViewColor> colors, List<ILayout> viewer)
  {
    Clouds = clouds;
    Colors = colors;
    Viewer = viewer;
  }

  public List<ILayout> Viewer {get;set;}

  /// <summary>
  ///   The lists of <see cref="ICloud" /> by <see cref="IntIHaveIdid" /> associated with the args
  /// </summary>
  public List<string> Clouds {get;}

  /// <summary>
  ///   List of colors to use for run time analysis
  /// </summary>
  public List<ViewColor> Colors {get;}
}
