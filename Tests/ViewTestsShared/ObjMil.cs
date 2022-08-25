using System.Collections.Generic;
using ViewObjects;
using ViewObjects.Cloud;
using ViewObjects.Content;
using ViewObjects.Study;
using ViewObjects.Viewer;
using ViewTo;

namespace ViewTo.Tests.Integration
{
  internal static class ObjMil
  {
    public static IViewStudy InitStudy(bool validSetup = true)
    {

      var s = new ViewStudy
      {
        viewName = "StudyCloudTest"
      };

      var cloud1 = Mil.Fabricate.Object.ViewCloud();
      var cloud2 = Mil.Fabricate.Object.ViewCloud();

      var bundle1 = new ViewerBundle
      {
        layouts = new List<IViewerLayout>
        {
          new ViewerLayout(), new ViewerLayoutCube()
        }
      };

      var bundle2 = new ViewerBundleLinked
      {
        layouts = new List<IViewerLayout>
        {
          new ViewerLayoutCube(), new ViewerLayoutFocus()
        },
        linkedClouds = new List<CloudShell>
        {
          cloud1.Build()
        }
      };

      var target1 = new TargetContent
      {
        viewName = "GlobalFunSpot"
      };

      var target2 = new TargetContent
      {
        viewName = "IsolatedTarget",
        isolate = true,
        bundles = new List<IViewerBundle>
        {
          bundle2
        }
      };


      if (!validSetup)
      {
        bundle1.layouts = new List<IViewerLayout>();
        bundle2.linkedClouds.Add(Mil.Fabricate.Object.ViewCloud().Build());
        target2.bundles = new List<IViewerBundle>
        {
          bundle2
        };
      }

      var content = new ContentBundle
      {
        contents = new List<IViewContent>
        {
          target1,
          target2,
          new BlockerContent
          {
            viewName = "blocker1"
          },
          new BlockerContent
          {
            viewName = "blocker2"
          },
          new DesignContent
          {
            viewName = "design1"
          },
          new DesignContent
          {
            viewName = "design2"
          },
          new DesignContent
          {
            viewName = "design3"
          }
        }
      };

      s.Set(bundle1);
      s.Set(bundle2);
      s.Set(cloud1);
      s.Set(cloud2);
      s.Set(content);
      return s;

    }
  }
}
