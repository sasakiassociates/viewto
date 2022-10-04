using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

		public List<TContent> GetContent<TContent>() where TContent : Content
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
				if (loadedObjs[i] is not ViewCloud cloud || !cloud.ViewId.Equals(results.id))
					continue;

				// reference game object with view cloud attached
				var go = cloud.gameObject;

				var resultCloud = go.AddComponent<ResultCloud>();
				resultCloud.name = "Result Cloud";
				resultCloud.ViewId = cloud.ViewId;
				resultCloud.Points = cloud.Points;
				resultCloud.Data = results.data;

				// Remove the view cloud component
				Destroy(go.GetComponent<ViewCloud>());

				// replace the view cloud with the result cloud
				loadedObjs[i] = resultCloud;

				OnResultsSet?.Invoke(resultCloud);
				return true;
			}

			Debug.Log($"No Clouds were found with {results.id}");
			return false;
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

		public event UnityAction<ResultCloud> OnResultsSet;

	}
}