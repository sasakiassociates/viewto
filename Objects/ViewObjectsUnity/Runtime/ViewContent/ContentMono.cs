using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ViewObjects.Unity
{
	public class ContentMono : ViewObjMono, IContent
	{

		[SerializeField] List<ContentObj> contentObjs;

		[SerializeField] Color32 color;

		[SerializeField] bool _combine = true;

		[SerializeField] string _viewId;

		public string ViewId
		{
			get => _viewId;
			set => _viewId = value;
		}

		[SerializeField] List<string> _references;

		public List<string> References
		{
			get => _references;
			set => _references = value;
		}

		[SerializeField] ContentType _contentType;

		public ContentType ContentType
		{
			get => _contentType;
			set => _contentType = value;
		}

		[SerializeField] [HideInInspector] int _layerMask;

		public int ContentLayerMask
		{
			get => _layerMask;
			set => _layerMask = value;
		}

		[SerializeField] string viewName;

		public string ViewName
		{
			get => viewName;
			set
			{
				viewName = value;
				gameObject.name = FullName;
			}
		}

		public string FullName => $"Content {ContentType.ToString().Split('.').LastOrDefault()} - {ViewName}";

		static int DiffuseColor => Shader.PropertyToID("_diffuseColor");

		public bool show
		{
			set
			{
				if (!contentObjs.Valid())
					return;

				foreach (var obj in contentObjs)
					obj.gameObject.SetActive(value);
			}
		}

		public List<object> objects
		{
			get => contentObjs.Valid() ? contentObjs.Cast<object>().ToList() : new List<object>();
			set
			{
				contentObjs = new List<ContentObj>();

				foreach (var o in value)
					if (o is Object obj)
					{
						ContentObj content;

						switch (obj)
						{
							case Component comp:
								comp.transform.SetParent(transform);
								content = comp.GetComponent<ContentObj>();
								if (content == null)
									content = comp.gameObject.AddComponent<ContentObj>();
								break;
							case GameObject go:
								go.transform.SetParent(transform);
								content = go.GetComponent<ContentObj>();
								if (content == null)
									content = go.gameObject.AddComponent<ContentObj>();
								break;
							default:
								Debug.LogWarning($"I don't know how to handle {obj.TypeName()}");
								content = null;
								break;
						}

						contentObjs.Add(content);
					}
			}
		}

		public ViewColor viewColor
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

		void ApplyColor()
		{
			if (contentObjs.Valid())
				foreach (var contentObj in contentObjs)
					contentObj.SetColor = color;
		}

		/// <summary>
		///   references the objects converted to the view content list and imports them
		/// </summary>
		public void PrimeMeshData(Material material, Action<ContentObj> onAfterPrime = null)
		{
			if (!objects.Valid())
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
				Debug.Log($"Combinding {contentObjs.Count} Mesh(es)");
				contentObjs = new List<ContentObj>() { gameObject.CombineMeshes(material) };
			}

			gameObject.ApplyAll(material);
			gameObject.SetLayerRecursively(ContentLayerMask);

			// this little loop is taking care of all the filtering of what speckle might send back. ideally it will be just components
			foreach (var obj in contentObjs)
			{
				onAfterPrime?.Invoke(obj);
			}

			Debug.Log($"{ViewName} is primed!\nview color {viewColor.ToUnity()}");
		}

	}
}