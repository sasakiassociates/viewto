using System;
using System.Collections.Generic;
using ViewObjects;
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

				static TObj CreateByType_WithName<TObj>(string name) where TObj : INameable
				{
					var obj = Instance<TObj>();
					obj.ViewName = name;
					return obj;
				}

				public static ResultCloudV1V1 ResultCloud()
				{
					var c = new ResultCloudV1V1
					{
						points = Data.CloudPoints(100), data = Data.PixelCollection(100)
					};
					return c;
				}

				public static ViewCloudV1V1 ViewCloud() => new ViewCloudV1V1
				{
					points = Data.CloudPoints(100)
				};

				public static ContentBundleV1 ContentCase()
				{
					var cb = new ContentBundleV1();
					cb.contents.AddRange(Data.GroupContentTargets);
					cb.contents.AddRange(Data.GroupContentBlockers);
					cb.contents.AddRange(Data.GroupContentDesigns);
					return cb;
				}

				public static ViewStudy_v1 ViewStudy(string name, bool withComponents = false, bool withResults = false)
				{
					var c = CreateByType_WithName<ViewStudy_v1>(name);
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

				public static RigParameters RigParams() => new RigParameters
				{
					bundles = new List<IViewerBundle_v1>
					{
						ViewerParam()
					}
				};

				public static ViewerBundleV1 ViewerParam() => new ViewerBundleV1
				{
					layouts = new List<IViewerLayout_v1>
					{
						Bundle<ViewerLayoutV1Horizontal>(),
						Bundle<ViewerLayoutV1Cube>(),
						Bundle<ViewerLayoutV1Normal>(),
						Bundle<ViewerLayoutV1Ortho>(),
						Bundle<ViewerLayoutV1Focus>()
					}
				};

				public static ViewerLayoutV1 Bundle<TBundle>() where TBundle : ViewerLayoutV1, new()
				{
					var b = Instance<TBundle>();
					// b.cloudInfo = Data.CloudShellInfo();
					return b;
				}
			}
		}
	}
}