using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ViewObjects.Common;
using ViewObjects.Contents;

namespace ViewObjects.Unity
{
	public class Content : ViewObjectMono, IContent, IContentObjects<GameObject>
	{

		[HideInInspector] [SerializeField] int _layerMask;

		[SerializeField] List<GameObject> _objects;

		[SerializeField] ContentType _contentType;

		[SerializeField] Color32 _color;

		[SerializeField] string _viewId;

		[SerializeField] string _viewName;

		[SerializeField, HideInInspector] List<string> references;

		public List<string> References
		{
			get => references;
			set => references = value;
		}

		public ViewColor Color
		{
			get => new(_color.r, _color.g, _color.b, _color.a);
			set
			{
				if (value == null)
					return;

				Debug.Log($"new assigned to {_viewName}:" + value.ToUnity());

				_color = value.ToUnity();

				ApplyColor();
			}
		}

		public string ViewId
		{
			get => _viewId;
			set => _viewId = value;
		}

		public ContentType ContentType
		{
			get => _contentType;
			set
			{
				_contentType = value;
				ContentLayerMask = value.GetLayerMask();
			}
		}

		public int ContentLayerMask
		{
			get => _layerMask;
			set => _layerMask = value;
		}

		public string ViewName
		{
			get => _viewName;
			set
			{
				_viewName = value;
				gameObject.name = FullName;
			}
		}

		public List<GameObject> Objects
		{
			get => _objects;
			set { _objects = value; }
		}

		public string FullName => $"Content {ContentType.ToString().Split('.').LastOrDefault()} - {ViewName}";

		static int DiffuseColor => Shader.PropertyToID("_diffuseColor");

		public bool Show
		{
			set
			{
				if (!_objects.Valid())
					return;

				foreach (var obj in _objects)
					obj.gameObject.SetActive(value);
			}
		}

		void ApplyColor()
		{
			if (!_objects.Valid())
			{
				Debug.Log("No Objects to apply");
				return;
			}

			foreach (var contentObj in _objects)
			{
				var meshRend = contentObj.GetComponent<MeshRenderer>();
				if (meshRend != null)
				{
					if (Application.isPlaying)
					{
						meshRend.material.SetColor(DiffuseColor, _color);
					}
					else
					{
						meshRend.sharedMaterial.SetColor(DiffuseColor, _color);
					}
				}
			}
		}

		/// <summary>
		///   references the objects converted to the view content list and imports them
		/// </summary>
		public void PrimeMeshData(Material material, Action<GameObject> onAfterPrime = null)
		{
			if (!Objects.Valid())
			{
				Debug.Log($"No objects for {name} are ready to be primed ");
				return;
			}

			if (material == null)
			{
				Debug.LogError($"Material is needed to prime mesh data on {name}");
				return;
			}

			var c = _color;

			if (material.HasProperty(DiffuseColor))
			{
				material.SetColor(DiffuseColor, c);
			}
			else
			{
				Debug.Log($"No property {DiffuseColor} on shader");
			}

			Debug.Log($"Combinding {_objects.Count} Mesh(es)");
			_objects = new List<GameObject>() { gameObject.CombineMeshes(material) };

			gameObject.ApplyAll(material);
			gameObject.SetLayerRecursively(ContentLayerMask);

			Debug.Log($"{ViewName} is primed!\nview color {Color.ToUnity()}");
		}

	}
}