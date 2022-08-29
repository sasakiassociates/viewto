// using UnityEngine;
// using ViewObjects.Viewer;
//
// namespace ViewObjects.Unity
// {
// 	public class ViewerMono : MonoBehaviour, IViewer
// 	{
// 		[SerializeField] ViewerDirection viewerDir;
//
// 		public ViewerDirection Direction
// 		{
// 			get => viewerDir;
// 			set
// 			{
// 				viewerDir = value;
// 				Align();
// 			}
// 		}
//
// 		public void Setup(IViewer viewer)
// 		{
// 			gameObject.name = "Viewer: " + viewer.Direction;
// 			Direction = viewer.Direction;
// 		}
//
// 		void Align()
// 		{
// 			var camDirection = Direction switch
// 			{
// 				ViewerDirection.Front => new Vector3(0, 0, 0),
// 				ViewerDirection.Left => new Vector3(0, 90, 0),
// 				ViewerDirection.Back => new Vector3(0, 180, 0),
// 				ViewerDirection.Right => new Vector3(0, -90, 0),
// 				ViewerDirection.Up => new Vector3(90, 0, 0),
// 				ViewerDirection.Down => new Vector3(-90, 0, 0),
// 				_ => new Vector3(0, 0, 0)
// 			};
// 			transform.localRotation = Quaternion.Euler(camDirection);
// 		}
// 	}
// }