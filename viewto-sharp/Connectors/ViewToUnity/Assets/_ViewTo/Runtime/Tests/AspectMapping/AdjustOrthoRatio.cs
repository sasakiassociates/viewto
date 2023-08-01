#region

using System;
using UnityEngine;

#endregion

namespace ViewTo.Connector.Unity
{
	public class AdjustOrthoRatio : MonoBehaviour
	{

		[SerializeField]
		Camera ortho;

		[SerializeField]
		Collider mesh;

		[SerializeField]
		Bounds bounds;

		// Start is called before the first frame update
		void Start()
		{
			bounds = mesh.bounds;
		}

		void OnGUI()
		{
			if (GUI.Button(new Rect(25, 25, 100, 25), "fit"))
				Fit();
		}

		void Fit()
		{
			var screenRatio = Screen.width / Screen.height;
			var targetRatio = bounds.size.x / bounds.size.z;

			// if the screen ration can fit our bounds
			var difference = 1f;
			if (screenRatio < targetRatio)
				difference = targetRatio / screenRatio;

			// center camera  
			ortho.transform.position = new Vector3(bounds.center.x, 100, bounds.center.z);
			// fit camera bounds
			ortho.orthographicSize = Math.Max(bounds.size.x, bounds.size.z) / 2 * difference;
		}
	}
}