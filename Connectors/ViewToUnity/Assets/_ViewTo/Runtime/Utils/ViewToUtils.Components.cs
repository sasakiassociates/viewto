#region

using UnityEngine;
using ViewObjects;
using ViewObjects.Contents;
using ViewObjects.Unity;

#endregion

namespace ViewTo.Connector.Unity
{

  public static partial class ViewToUtils
  {
    public static bool Compare(this ViewColor a, Color32 b) => a.R == b.r && a.G == b.g && a.B == b.b;

    public static int GetCullingMask(this ViewContentType value)
    {
      return value switch
      {
        ViewContentType.Potential => (1 << ViewObject.TargetLayer) | (1 << ViewObject.CloudLayer),
        ViewContentType.Existing => (1 << ViewObject.TargetLayer) | (1 << ViewObject.BlockerLayer) | (1 << ViewObject.CloudLayer),
        ViewContentType.Proposed => (1 << ViewObject.TargetLayer) | (1 << ViewObject.BlockerLayer) | (1 << ViewObject.ProposedLayer) | (1 << ViewObject.CloudLayer),
        _ => -1
      };
    }


    public static Color32 ToUnity(this System.Drawing.Color c)
      => new Color32(c.R, c.G, c.B, c.A);

    public static System.Drawing.Color ToSystem(this Color c) => 
      System.Drawing.Color.FromArgb((int)c.a * 255, (int)c.r * 255, (int)c.g * 255, (int)c.b * 255);


    public static System.Drawing.Color[] GetColors(this Gradient gradient, int steps = 0)
    {
      if(steps == 0) steps = gradient.colorKeys.Length;
      var colors = new System.Drawing.Color[steps];
      for(int i = 0; i < steps; i++)
      {
        var time = 0.0f;
        if(i == steps - 1)
        {
          time = 1.0f;
        }
        else if(i > 0)
        {
          time = i / (float)steps;
        }

        colors[i] = gradient.Evaluate(time).ToSystem();
      }

      return colors;

    }
  }

}
