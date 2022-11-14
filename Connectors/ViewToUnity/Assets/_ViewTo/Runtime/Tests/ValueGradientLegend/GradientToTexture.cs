#region

using System;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace ViewTo.Connector.Unity
{
	[ExecuteAlways]
	public class GradientToTexture : MonoBehaviour
	{
		[SerializeField]
		Gradient gradient;

		[SerializeField]
		Image image;

		[SerializeField]
		[HideInInspector]
		Texture2D texture;

		/// <summary>
		///   Sets the mode of the gradient and repaints the texture
		/// </summary>
		public GradientMode mode
		{
			set
			{
				if (gradient == null)
				{
					Debug.Log("No gradient available to modify");
				}
				else
				{
					gradient.mode = value;
					PaintTexture();
				}
			}
		}

		void OnGUI()
		{
			if (GUI.Button(new Rect(0, 0, 50, 20), "Paint"))
				PaintTexture();
			if (GUI.Button(new Rect(0, 20, 50, 20), "Create"))
				SetupTexture();
		}

		public void SetupTexture(int width = 1, int height = 50)
		{
			texture = new Texture2D(width, height)
			{
				wrapMode = TextureWrapMode.Clamp,
				name = "Gradient Texture"
			};

			var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
			sprite.name = "Gradient Sprite";

			if (image == null)
				Debug.Log($"No image available for {name} to set gradient to");
			else
				image.sprite = sprite;
			// PaintTexture();
		}

		/// <summary>
		///   Applies the current gradient to a texture
		///   If no texture is assigned a default 50x1 blank texture will be created
		/// </summary>
		public void PaintTexture()
		{
			if (texture == null)
				SetupTexture();

			var longSide = Math.Max(texture.height, texture.width);
			var shortSide = Math.Min(texture.height, texture.width);

			var pixels = new Color[longSide * shortSide];
			for (int i = 0, index = 0; i < longSide; i++)
			{
				var color = gradient.Evaluate((float)i / (longSide - 1));

				for (var j = 0; j < shortSide; j++, index++)
					pixels[index] = color;
			}

			texture.SetPixels(pixels);
			texture.Apply(false);
		}
	}
}