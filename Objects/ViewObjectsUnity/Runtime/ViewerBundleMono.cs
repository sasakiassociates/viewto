using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ViewObjects.Unity
{
	
	public class ViewerBundleMono : ViewObjMono, IViewerBundle
	{

		[SerializeField] private List<SoViewerLayout> _layouts;

		public List<IViewerLayout> layouts
		{
			get => _layouts.Valid() ? _layouts.Select(x => x.GetRef).ToList() : new List<IViewerLayout>();
			set
			{
				_layouts = new List<SoViewerLayout>();
				
				foreach (var item in value)
				{
					var data = ScriptableObject.CreateInstance<SoViewerLayout>();
					data.SetRef(item);
					_layouts.Add(data);
				}
			}
		}

		public void Clear()
		{
			_layouts = new List<SoViewerLayout>();
		}
		
	}
}