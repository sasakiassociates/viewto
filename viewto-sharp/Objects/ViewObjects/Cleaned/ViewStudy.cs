using Sasaki.Analysis;
using Sasaki.Common;
using System.Collections.Generic;

namespace Sasaki.ViewObjects;

/// <summary>
/// I tend to like to extend the study interface so I can pass around all of the high level study commands that way
/// </summary>
public interface IViewStudy : IStudy<IViewObject>
{ }

public class ViewStudy : IViewStudy, IViewObject, IHaveName
{

  public ViewStudy()
  { }

  /// <inheritdoc />
  public string name {get;set;} = "";

  /// <inheritdoc />
  public string appId {get;set;} = "";

  /// <inheritdoc />
  public List<IViewObject> objects {get;set;} = new List<IViewObject>();
}