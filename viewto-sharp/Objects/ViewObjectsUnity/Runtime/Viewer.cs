using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ViewObjects.Common;
using ViewObjects.Systems;
using ViewObjects.Systems.Layouts;

namespace ViewObjects.Unity
{
	public class Viewer : ViewObjectMono, IViewer
	{

		[SerializeField] List<Layout> _layouts;

		public List<ILayout> Layouts
		{
			get => _layouts.Valid() ? _layouts.Cast<ILayout>().ToList() : new List<ILayout>();
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