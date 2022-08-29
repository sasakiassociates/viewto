#region

using Sasaki.Unity;
using UnityEngine;

#endregion

namespace ViewTo
{
	public class TestHub : MonoBehaviour
	{
		// Start is called before the first frame update
		void Start()
		{
			var pf = new GameObject().AddComponent<PixelFinderCube>();
			pf.Init(1, Color.white);
		}

	}
}