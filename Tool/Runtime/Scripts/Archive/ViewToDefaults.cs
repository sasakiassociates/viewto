#region

using UnityEngine;
using ViewObjects;

#endregion

namespace ViewTo.Connector.Unity
{

	[CreateAssetMenu(menuName = "ViewTo/Create Defualts", fileName = "ViewToDefaults", order = 0)]
	public class ViewToDefaults : ScriptableObject
	{
		public ComputeShader pixelShader;
		public Gradient gradient;

		public Material analysisMaterial;
		public Material targetMaterial, blockerMaterial, designMaterial;

		public static ViewToDefaults Instance { get; set; }

		public void OnEnable()
		{
			Instance = this;
			Debug.Log(this.TypeName() + "Instance set");
		}
	}
}