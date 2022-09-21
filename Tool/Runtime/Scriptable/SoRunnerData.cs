#region

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ViewObjects;
using ViewObjects.Unity;

#endregion

namespace ViewTo.Connector.Unity
{
	[Serializable]
	public class ViewColorWithName : ViewColor
	{

		public string content;

		public ViewColorWithName()
		{ }

		public ViewColorWithName(ViewColor color, string name)
		{
			content = name;

			R = color.R;
			G = color.G;
			B = color.B;
			A = color.A;
		}

		public ViewColorWithName(IViewContent viewContent)
		{
			content = viewContent.ViewName;

			R = viewContent.viewColor.R;
			G = viewContent.viewColor.G;
			B = viewContent.viewColor.B;
			A = viewContent.viewColor.A;
		}
	}

	public class SoRunnerData : ScriptableObject
	{
		[SerializeField]
		Vector3[] _points;
		[SerializeField]
		ViewColor[] _viewColors;
		[SerializeField]
		Color32[] _colors;
		[SerializeField]
		List<ViewColorWithName> colorWithNames;

		List<string> GetTargetNames
		{
			get
			{
				var items = new List<string>();
				if (!colorWithNames.Valid())
					return items;

				foreach (var cn in colorWithNames)
					items.Add(cn.content);

				return items;
			}
		}

		public int count
		{
			get => points.Valid() ? points.Length : 0;
		}

		public Color32[] colors
		{
			get => _colors;
		}

		public Vector3[] points
		{
			get => _points;
			set => _points = value;
		}

		public List<ViewColorWithName> nameAndColor
		{
			get => colorWithNames;
			set => colorWithNames = value;
		}

		public ViewColor[] viewColors
		{
			get => _viewColors;
			set
			{
				_viewColors = value;
				_colors = value.Select(x => x.ToUnity()).ToArray();
			}
		}
	}

}