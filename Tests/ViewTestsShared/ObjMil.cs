using System.Collections.Generic;
using ViewObjects;
using ViewObjects.Cloud;
using ViewObjects.Study;
using ViewObjects.Viewer;

namespace ViewTo.Tests.Integration
{
	internal static class ObjMil
	{
		public static IViewStudy_v1 InitStudy(bool validSetup = true)
		{
			var s = new ViewStudy_v1
			{
				ViewName = "StudyCloudTest"
			};

			var cloud1 = Mil.Fabricate.Object.ViewCloud();
			var cloud2 = Mil.Fabricate.Object.ViewCloud();

			var bundle1 = new ViewerBundleV1
			{
				layouts = new List<IViewerLayout_v1>
				{
					new ViewerLayoutV1(), new ViewerLayoutV1Cube()
				}
			};

			var bundle2 = new ViewerBundleLinkedV1
			{
				layouts = new List<IViewerLayout_v1>
				{
					new ViewerLayoutV1Cube(), new ViewerLayoutV1Focus()
				},
				linkedClouds = new List<CloudShell>
				{
					cloud1.Build()
				}
			};

			var target1 = new TargetContentV1
			{
				ViewName = "GlobalFunSpot"
			};

			var target2 = new TargetContentV1
			{
				ViewName = "IsolatedTarget",
				isolate = true,
				bundles = new List<IViewerBundle_v1>
				{
					bundle2
				}
			};

			if (!validSetup)
			{
				bundle1.layouts = new List<IViewerLayout_v1>();
				bundle2.linkedClouds.Add(Mil.Fabricate.Object.ViewCloud().Build());
				target2.bundles = new List<IViewerBundle_v1>
				{
					bundle2
				};
			}

			var content = new ContentBundleV1
			{
				contents = new List<IViewContent_v1>
				{
					target1,
					target2,
					new BlockerContentV1
					{
						ViewName = "blocker1"
					},
					new BlockerContentV1
					{
						ViewName = "blocker2"
					},
					new DesignContentV1
					{
						ViewName = "design1"
					},
					new DesignContentV1
					{
						ViewName = "design2"
					},
					new DesignContentV1
					{
						ViewName = "design3"
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