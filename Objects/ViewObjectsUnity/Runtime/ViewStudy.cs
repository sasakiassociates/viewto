using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ViewObjects.Common;
using ViewObjects.Studies;

namespace ViewObjects.Unity
{

	public class ViewStudy : ViewObjectMono, IViewStudy
	{

		[SerializeField] string _viewId;

		[SerializeField] List<ViewObjectMono> loadedObjs;

		public string ViewId
		{
			get => _viewId;
			set => _viewId = value;
		}

		public ResultCloud TrySetResults(ResultsForCloud results)
		{
			if (results == null || !results.id.Valid())
			{
				Debug.Log("Cannot set results to cloud");
				return null;
			}

			Debug.Log($"Trying to set new results to cloud {results.id}");
			ResultCloud rc = null;

			for (var i = 0; i < loadedObjs.Count; i++)
			{
				if (loadedObjs[i] is not ViewCloud cloud || !cloud.ViewId.Equals(results.id))
					continue;

				// reference game object with view cloud attached
				var go = cloud.gameObject;

				rc = go.AddComponent<ResultCloud>();
				rc.name = "Result Cloud";
				rc.ViewId = cloud.ViewId;
				rc.Points = cloud.Points;
				rc.Data = results.data;

				// Remove the view cloud component
				Destroy(go.GetComponent<ViewCloud>());

				// replace the view cloud with the result cloud
				loadedObjs[i] = rc;
			}

			return rc;
		}

		public string ViewName
		{
			get => gameObject.name;
			set => name = value;
		}

		public bool IsValid
		{
			get => Objects.Valid() && ViewName.Valid();
		}

		public List<IViewObject> Objects
		{
			get
			{
				var res = new List<IViewObject>();

				foreach (var obj in loadedObjs)
				{
					if (obj != null && obj is IViewObject casted)
						res.Add(casted);
				}

				return res;
			}
			set
			{
				loadedObjs = new List<ViewObjectMono>();

				foreach (var obj in value)
					if (obj is ViewObjectMono mono)
					{
						mono.transform.SetParent(transform);
						loadedObjs.Add(mono);
					}
					else
					{
						Debug.Log(obj.TypeName() + "- is not valid for mono");
					}
			}
		}
		
	}
}