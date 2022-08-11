using System;
using System.Collections.Generic;
using ViewObjects;
using ViewObjects.Content;
using ViewObjects.Cloud;
using ViewObjects.Rig;
using ViewObjects.Study;
using ViewObjects.Viewer;

namespace ViewTo
{
  public static partial class Mil
  {
    /// <summary>
    ///   A space for creating fake ViewTo data
    /// </summary>
    public static partial class Fabricate
    {

      public static T Instance<T>()
      {
        var t = (T)Activator.CreateInstance(typeof(T));
        return t;
      }

      public static class Object
      {

        private static TObj CreateByType_WithName<TObj>(string name) where TObj : INameable
        {
          var obj = Instance<TObj>();
          obj.viewName = name;
          return obj;
        }

        public static ResultCloud ResultCloud()
        {
          var c = new ResultCloud
          {
            points = Data.CloudPoints(100), data = Data.PixelCollection(100)
          };
          return c;
        }

        public static ViewCloud ViewCloud()
        {
          return new ViewCloud
          {
            points = Data.CloudPoints(100)
          };
        }

        public static ContentBundle ContentCase()
        {
          var cb = new ContentBundle();
          cb.contents.AddRange(Data.GroupContentTargets);
          cb.contents.AddRange(Data.GroupContentBlockers);
          cb.contents.AddRange(Data.GroupContentDesigns);
          return cb;
        }

        public static ViewStudy ViewStudy(string name, bool withComponents = false, bool withResults = false)
        {
          var c = CreateByType_WithName<ViewStudy>(name);
          if (withComponents || withResults)
          {
            c.Set(ContentCase());
            c.Set(ViewCloud());
            c.Set(ViewerParam());
            if (withResults)
              c.Set(ResultCloud());
          }

          return c;
        }

        public static RigParameters RigParams()
        {
          return new RigParameters
          {
            bundles = new List<IViewerBundle>
            {
              ViewerParam()
            }
          };
        }

        public static ViewerBundle ViewerParam()
        {
          return new ViewerBundle
          {
            layouts = new List<IViewerLayout>
            {
              Bundle<ViewerLayoutHorizontal>(),
              Bundle<ViewerLayoutCube>(),
              Bundle<ViewerLayoutNormal>(),
              Bundle<ViewerLayoutOrtho>(),
              Bundle<ViewerLayoutFocus>()
            }
          };
        }

        public static ViewerLayout Bundle<TBundle>() where TBundle : ViewerLayout, new()
        {
          var b = Instance<TBundle>();
          // b.cloudInfo = Data.CloudShellInfo();
          return b;
        }
      }
    }
  }
}
