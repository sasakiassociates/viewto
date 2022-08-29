#region

using UnityEngine;
using UnityEngine.Events;
using ViewObjects;

#endregion

namespace ViewTo.Connector.Unity
{
	public class CameraCrew : MonoBehaviour
	{
		[SerializeField]
		FirstPersonViewer firstPerson;
		[SerializeField]
		Camera ui;
		[SerializeField]
		Camera map;

		public UnityEvent<Camera> OnMapCameraSet;
		public UnityEvent<FirstPersonViewer> OnFirstPersonCameraSet;

		void Start()
		{
			SetCams();
		}

		public void ContentMaskUpdated(ResultType res)
		{
			if (firstPerson != null && firstPerson.viewer != null)
				firstPerson.viewer.cullingMask = res.GetCullingMask();
		}

		public void SetCams()
		{
			if (map == null)
				ViewConsole.Log("No Map Camera available. Set this property in order to use it!");
			else
				OnMapCameraSet?.Invoke(map);

			if (firstPerson != null)
				OnFirstPersonCameraSet.Invoke(firstPerson);

			if (ui != null)
				ui.enabled = true;
		}
	}
}