using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ViewObjects.Viewer;

namespace ViewObjects.Unity
{

	public class ViewerBundleMono : ViewObjMono, IViewerBundle
	{

		[SerializeField] private List<SoViewerLayout> _layouts;

		List<IViewerLayout> _tempLayouts;

		public List<IViewerLayout> layouts
		{
			get => _tempLayouts.Valid() ? _tempLayouts : new List<IViewerLayout>();
			// get => _layouts.Valid() ? _layouts.Select(x => x.GetRef).ToList() : new List<IViewerLayout>();
			set
			{
				_tempLayouts = value;
				return;

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
			_tempLayouts = new List<IViewerLayout>();
			_layouts = new List<SoViewerLayout>();
		}

	}
}