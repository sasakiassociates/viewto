using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace ViewObjects.Unity
{

	public class ViewStudyMono : ViewObjMono, IViewStudy
	{

		[SerializeField] List<ViewObjMono> loadedObjs;

		public List<TContent> GetContent<TContent>() where TContent : ContentMono
		{
			var res = new List<TContent>();

			foreach (var obj in loadedObjs)
			{
				if (obj == null || obj is not ContentBundleMono mono)
					continue;

				res.AddRange(mono.GetContent<TContent>());
			}

			return res;
		}

		public bool TrySetResults(ResultsForCloud results)
		{
			if (results == null || !results.id.Valid())
			{
				Debug.Log("Cannot set results to cloud");
				return false;
			}

			Debug.Log($"Trying to set new results to cloud {results.id}");

			for (var i = 0; i < loadedObjs.Count; i++)
			{
				if (loadedObjs[i] is not ViewCloudMono cloud || !cloud.viewID.Equals(results.id))
					continue;

				// reference game object with view cloud attached
				var go = cloud.gameObject;

				var resultCloud = go.AddComponent<ResultCloudMono>();
				resultCloud.name = "Result Cloud";
				resultCloud.viewID = cloud.viewID;
				resultCloud.points = cloud.points;
				resultCloud.data = results.data;

				// Remove the view cloud component
				Destroy(go.GetComponent<ViewCloudMono>());

				// replace the view cloud with the result cloud
				loadedObjs[i] = resultCloud;

				OnResultsSet?.Invoke(resultCloud);
				return true;
			}

			Debug.Log($"No Clouds were found with {results.id}");
			return false;
		}

		public string viewName
		{
			get => gameObject.name;
			set => name = value;
		}

		public bool isValid
		{
			get => objs.Valid() && viewName.Valid();
		}

		public List<IViewObj> objs
		{
			get
			{
				var res = new List<IViewObj>();

				foreach (var obj in loadedObjs)
					if (obj != null && obj is IViewObj casted)
						res.Add(casted);

				return res;
			}
			set
			{
				loadedObjs = new List<ViewObjMono>();

				foreach (var obj in value)
					if (obj is ViewObjMono mono)
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

		public event UnityAction<ResultCloudMono> OnResultsSet;

	}
}