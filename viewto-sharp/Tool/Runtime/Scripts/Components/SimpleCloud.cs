#region

using System.Collections.Generic;
using Pcx;
using UnityEngine;

#endregion

namespace ViewTo.Connector.Unity
{
	public class SimpleCloud : MonoBehaviour
	{

		[SerializeField]
		PointCloudRenderer _pcRenderer;

		[SerializeField]
		[HideInInspector]
		PointCloudData _pcData;

		public float pointSize
		{
			get => _pcRenderer != null ? _pcRenderer.pointSize : 0;
			set
			{
				if (_pcRenderer != null)
					_pcRenderer.pointSize = value;
			}
		}

		public void Render(List<Vector3> vector3, List<Color32> colors)
		{
			// if (_pcData == null)
			// {
			// 	#if UNITY_EDITOR
			//   _pcData = ScriptableObject.CreateInstance<PointCloudData>();
			// 	#endif
			// }

			_pcData = ScriptableObject.CreateInstance<PointCloudData>();

			#if UNITY_EDITOR
			_pcData.Initialize(vector3, colors);
			#endif

			if (_pcRenderer == null)
			{
				_pcRenderer = gameObject.GetComponent<PointCloudRenderer>();
				if (_pcRenderer == null)
					_pcRenderer = gameObject.AddComponent<PointCloudRenderer>();
			}

			_pcRenderer.sourceData = _pcData;
		}
	}
}