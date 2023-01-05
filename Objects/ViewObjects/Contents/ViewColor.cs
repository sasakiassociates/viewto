using System;
using System.Drawing;

namespace ViewObjects.Contents;

[Serializable]
public class ViewColor
{
  public byte A;
  public byte B;
  public byte G;
  public byte R;

  public ViewColor()
  { }

  public ViewColor(byte r, byte g, byte b, byte a)
  {
    R = r;
    G = g;
    B = b;
    A = a;
  }

  public static Color[] Ramp()
  {
    return new[]
    {
      Color.FromArgb(255, 54, 2, 89),
      Color.FromArgb(255, 92, 0, 91),
      Color.FromArgb(255, 127, 0, 86),
      Color.FromArgb(255, 158, 0, 75),
      Color.FromArgb(255, 183, 0, 59),
      Color.FromArgb(255, 202, 0, 38),
      Color.FromArgb(255, 213, 0, 0)
    };
  }
}
