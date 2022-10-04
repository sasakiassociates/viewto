using UnityEngine;

namespace ViewObjects.Unity
{
	public class ContentObj : MonoBehaviour
	{
		static int DiffuseColor
		{
			get => Shader.PropertyToID("_diffuseColor");
		}

		public Color32 SetColor
		{
			set
			{
				var meshRend = gameObject.GetComponent<MeshRenderer>();
				if (meshRend != null)
				{
					if (Application.isPlaying)
						meshRend.material.SetColor(DiffuseColor, value);
					else
						meshRend.sharedMaterial.SetColor(DiffuseColor, value);
				}
			}
		}

		public Material SetMat
		{
			set
			{
				var meshRend = gameObject.GetComponent<MeshRenderer>();
				if (meshRend == null)
					meshRend = gameObject.AddComponent<MeshRenderer>();

				if (Application.isPlaying)
					meshRend.material = value;
				else
					meshRend.sharedMaterial = value;
			}

		}
	}
}