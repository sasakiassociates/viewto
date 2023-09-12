using System;
using System.Collections.Generic;
using ViewObjects.Contents;

namespace ViewTo.Cmd
{

  /// <summary>
  ///   <para>Goes through all the valid view colors and assigns them a value through <see cref="IContext.Color" /></para>
  /// </summary>
  public class AssignViewColors : ICmd
  {
    private List<IContext> _contents;

    public AssignViewColors(List<IContext> contents)
    {
      _contents = contents;
    }

    public void Execute()
    {
      if(_contents == null || _contents.Count == 0)
      {
        return;
      }

      var colorSet = new HashSet<ViewColor>();
      var r = new Random();

      while(colorSet.Count < _contents.Count)
      {
        var b = new byte[3];
        r.NextBytes(b);
        var tempColor = new ViewColor(b[0], b[1], b[2], 255);
        colorSet.Add(tempColor);
      }

      var index = 0;
      foreach(var c in colorSet)
      {
        _contents[index++].Color = c;
      }
    }
  }

}
