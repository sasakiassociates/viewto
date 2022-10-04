using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ViewObjects.Unity
{
	public class Content : ViewObjectMono, IContent, IContentObjects<ContentObj>
	{

		[HideInInspector] [SerializeField] int _layerMask;

		[SerializeField] List<ContentObj> _contentObjs;

		[SerializeField] ContentType _contentType;

		[SerializeField] Color32 color;

		[SerializeField] bool _combine = true;

		[SerializeField] string _viewId;

		[SerializeField] string viewName;
		
		[SerializeField, HideInInspector] List<string> _reference;

		public List<string> Reference
		{
			get => _reference;
			set => _reference = value;
		}

		public ViewColor Color
		{
			get => new(color.r, color.g, color.b, color.a);
			set
			{
				if (value == null)
					return;

				Debug.Log($"new assigned to {viewName}:" + value.ToUnity());

				color = value.ToUnity();

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
			set => _contentType = value;
		}

		public int ContentLayerMask
		{
			get => _layerMask;
			set => _layerMask = value;
		}

		public string ViewName
		{
			get => viewName;
			set
			{
				viewName = value;
				gameObject.name = FullName;
			}
		}

		public List<ContentObj> Objects
		{
			get => _contentObjs;
			set => _contentObjs = value;
		}

		public string FullName => $"Content {ContentType.ToString().Split('.').LastOrDefault()} - {ViewName}";

		static int DiffuseColor => Shader.PropertyToID("_diffuseColor");

		public bool show
		{
			set
			{
				if (!_contentObjs.Valid())
					return;

				foreach (var obj in _contentObjs)
					obj.gameObject.SetActive(value);
			}
		}

		void ApplyColor()
		{
			if (_contentObjs.Valid())
				foreach (var contentObj in _contentObjs)
					contentObj.SetColor = color;
		}

		/// <summary>
		///   references the objects converted to the view content list and imports them
		/// </summary>
		public void PrimeMeshData(Material material, Action<ContentObj> onAfterPrime = null)
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

			var c = color;

			if (material.HasProperty(DiffuseColor))
				material.SetColor(DiffuseColor, c);
			else
				Debug.Log($"No property {DiffuseColor} on shader");

			if (_combine)
			{
				Debug.Log($"Combinding {_contentObjs.Count} Mesh(es)");
				_contentObjs = new List<ContentObj>() { gameObject.CombineMeshes(material) };
			}

			gameObject.ApplyAll(material);
			gameObject.SetLayerRecursively(ContentLayerMask);

			// this little loop is taking care of all the filtering of what speckle might send back. ideally it will be just components
			foreach (var obj in _contentObjs)
			{
				onAfterPrime?.Invoke(obj);
			}

			Debug.Log($"{ViewName} is primed!\nview color {Color.ToUnity()}");
		}

	}
}