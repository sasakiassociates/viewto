#region

using UnityEngine;
using ViewObjects;

#endregion

namespace ViewTo.Connector.Unity
{
	public class ViewPointClicker : MonoBehaviour
	{
		[SerializeField]
		ResultExplorerMono _resultExplorer;
		Vector3[] _points;
		Vector3 lastPos;

		void Start()
		{
			if (_resultExplorer != null)
				_resultExplorer.OnNewCloudSet += () =>
				{
					_points = _resultExplorer.points;
					transform.position = _points[_resultExplorer.activePoint];
					lastPos = transform.position;
				};
		}

		public void Update()
		{
			if (!_points.Valid() || !transform.hasChanged || lastPos.Equals(transform.position))
				return;

			var minDist = -1f;
			var minPoint = -1;

			for (var i = 0; i < _points.Length; i++)
			{
				var pt = _points[i];
				var dist = Vector3.Distance(pt, gameObject.transform.position);

				if (minDist < 0 || dist < minDist)
				{
					minDist = dist;
					minPoint = i;
				}
			}

			if (minPoint >= 0)
			{
				_resultExplorer.activePoint = minPoint;
				lastPos = transform.position;
			}
		}
	}
}