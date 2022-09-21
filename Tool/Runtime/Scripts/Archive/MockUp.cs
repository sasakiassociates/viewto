#region

using System.Collections.Generic;
using UnityEngine;
using ViewObjects;
using ViewObjects.Cloud;
using ViewObjects.Content;
using ViewObjects.Study;
using ViewObjects.Viewer;

#endregion

namespace ViewTo.Connector.Unity
{
	public static class MockUp
	{

		static ViewCloud TestCloud
		{
			get
			{
				var pts = new CloudPoint[100];
				for (var i = 0; i < pts.Length; i++)
					pts[i] = new CloudPoint(Random.Range(0f, 50f), Random.Range(0f, 50f), Random.Range(0f, 50f), 0, 0, 0, "test");

				return new ViewCloud { points = pts };
			}
		}

		static GameObject CreatePrim(PrimitiveType t, Vector3 pos)
		{
			var obj = GameObject.CreatePrimitive(t);
			obj.transform.position = pos;
			obj.transform.localScale = Vector3.one * 4f;
			return obj;
		}

		public static List<TContent> BasicContent<TContent>() where TContent : IViewContent, new()
		{
			var tc = new TContent().TypeName();
			return new List<TContent>
			{
				new()
				{
					ViewName = tc + "-A",
					objects = new List<object>
					{
						CreatePrim(PrimitiveType.Cube, new Vector3(Random.Range(0f, 50f), Random.Range(0f, 50f))),
						CreatePrim(PrimitiveType.Cube, new Vector3(Random.Range(0f, 50f), Random.Range(0f, 50f)))
					}
				},

				new()
				{
					ViewName = tc + "-B",
					objects = new List<object>
					{
						CreatePrim(PrimitiveType.Sphere, new Vector3(Random.Range(0f, 50f), Random.Range(0f, 50f))),
						CreatePrim(PrimitiveType.Sphere, new Vector3(Random.Range(0f, 50f), Random.Range(0f, 50f)))
					}
				}
			};
		}

		public static ViewStudy Create(string studyName)
		{
			var study = new ViewStudy
				{ ViewName = studyName };

			var contentBundle = new ContentBundle();
			contentBundle.contents.AddRange(BasicContent<TargetContent>());
			contentBundle.contents.AddRange(BasicContent<BlockerContent>());
			contentBundle.contents.AddRange(BasicContent<DesignContent>());

			var cloud = TestCloud;

			var viewerBundle = new ViewerBundle
			{
				layouts = new List<IViewerLayout>
					{ new ViewerLayoutCube() }
			};

			study.objs = new List<IViewObj>
				{ contentBundle, cloud, viewerBundle };

			return study;
		}

		public static ViewerBundle CreateFakeBundle(ViewerLayout layout = null)
		{
			layout ??= new ViewerLayoutCube();
			return new ViewerBundle
			{
				layouts = new List<IViewerLayout>
				{
					layout
				}
			};
		}
	}
}