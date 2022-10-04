using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ViewObjects.Unity
{
	public class Viewer : ViewObjectMono, IViewer
	{

		[SerializeField] List<Layout> _layouts;

		public List<IViewerLayout> Layouts
		{
			get => _layouts.Valid() ? _layouts.Cast<IViewerLayout>().ToList() : new List<IViewerLayout>();
			set
			{
				_layouts = new List<Layout>();
				if (value.Valid())
				{
					foreach (var v in value)
					{
						if (v is Layout vl)
							_layouts.Add(vl);
					}
				}
			}
		}

	}
}