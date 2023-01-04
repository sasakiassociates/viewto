#region

using UnityEngine;
using ViewTo.Connector.Unity.Commands;

#endregion

namespace ViewTo.Connector.Unity
{
	[RequireComponent(typeof(Camera))]
	public class FirstPersonViewer : MonoBehaviour
	{
		public Camera viewer;

		public float lookSpeed = 2.0f;
		public float lookXLimit = 45.0f;

		[SerializeField]
		GameObject icon;
		float rotationX;
		float rotationY;

		Vector3 savedIconRotation;
		float saveIconEyeLevel;

		/// <summary>
		///   Toggle for allowing camera to move
		/// </summary>
		public bool Lock { get; set; }

		void Start()
		{
			viewer = gameObject.GetComponent<Camera>();
			savedIconRotation = icon.transform.localRotation.eulerAngles;
			saveIconEyeLevel = icon.transform.localPosition.y;
			// Lock cursor
			// Cursor.lockState = CursorLockMode.Locked;
			// Cursor.visible = false;
		}

		public void SetViewerPos(ActivePointArgs args)
		{
			viewer.transform.position = args.position;
			viewer.transform.LookAt(args.center);

			// reset rotation param
			rotationX = viewer.transform.localRotation.eulerAngles.x;
			rotationY = viewer.transform.localRotation.eulerAngles.y;

			icon.transform.rotation = Quaternion.Euler(
				new Vector3(
					savedIconRotation.x,
					savedIconRotation.y + rotationY,
					savedIconRotation.z)
			);
		}

		public void Move()
		{
			var yAxis = Input.GetAxis("Mouse Y");
			var xAxis = Input.GetAxis("Mouse X");

			rotationX += yAxis * lookSpeed;
			rotationY += xAxis * lookSpeed;

			// Debug.Log($"xAxis={xAxis}  yAxis={yAxis}");
			// Debug.Log($"rotationX={rotationX}  rotationY={rotationY}");

			viewer.transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0);

			icon.transform.position = new Vector3(
				viewer.transform.position.x,
				viewer.transform.position.y + saveIconEyeLevel,
				viewer.transform.position.z
			);

			icon.transform.rotation = Quaternion.Euler(
				new Vector3(
					savedIconRotation.x,
					savedIconRotation.y + viewer.transform.localRotation.eulerAngles.y,
					savedIconRotation.z)
			);
		}
	}
}