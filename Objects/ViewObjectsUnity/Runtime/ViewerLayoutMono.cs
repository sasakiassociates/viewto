using System.Collections.Generic;
using UnityEngine;
using ViewObjects.Viewer;

namespace ViewObjects.Unity
{
	public class ViewerLayoutMono : ViewObjMono, IViewerLayout
	{
		// [SerializeField] List<ViewerMono> sceneViewers;

		[SerializeField] SoViewerLayout data;

		public List<IViewer> viewers
		{
			get
			{
				var res = new List<IViewer>();
				if (data != null)
				{
					var layout = data.GetRef;
					if (layout != default)
						foreach (var viewer in layout.viewers)
							res.Add(viewer);
				}

				return res;
			}
		}

		public void SetData(ViewerLayout obj)
		{
			Clear();

			data = ScriptableObject.CreateInstance<SoViewerLayout>();
			data.SetRef(obj);

			gameObject.name = data.GetName;
		}

		public void SetData(SoViewerLayout obj)
		{
			Clear();

			data = obj;
			gameObject.name = data.GetName;
		}

		public void Clear()
		{
			// if (viewers.Valid())
			// 	ViewObjMonoExt.ClearList(sceneViewers);
			//
			// sceneViewers = new List<ViewerMono>();
		}

		// public void Build()
		// {
		// 	Debug.Log($"Build Process called for {name}");
		//
		// 	if (data == null)
		// 	{
		// 		Debug.LogWarning($"{name} does not have valid viewer layout data to build");
		// 		return;
		// 	}
		//
		// 	Clear();
		//
		// 	var prefab = new GameObject().AddComponent<ViewerMono>();
		// 	
		// 	foreach (var v in viewers)
		// 	{
		// 		var mono = Instantiate(prefab, transform);
		// 		mono.Setup(v);
		// 		sceneViewers.Add(mono);
		//
		// 		// onBuildComplete?.Invoke(mono);
		// 	}
		//
		// 	ViewObjMonoExt.SafeDestroy(prefab.gameObject);
		// }
	}
}