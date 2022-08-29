using System;
using UnityEngine;
using ViewObjects.Viewer;

namespace ViewObjects.Unity
{

	public class SoViewerLayout : ScriptableObject
	{
		[SerializeField] ClassTypeReference objType;

		public IViewerLayout GetRef
		{
			get => objType != null ? (ViewerLayout)Activator.CreateInstance(objType.Type) : null;
		}

		public string GetName
		{
			get => GetRef?.TypeName();
		}

		public void SetRef(IViewerLayout obj)
		{
			objType = new ClassTypeReference(obj.GetType());
		}
		
	}

}