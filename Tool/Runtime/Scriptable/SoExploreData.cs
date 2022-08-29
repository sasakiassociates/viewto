#region

using System.Collections.Generic;
using UnityEngine;
using ViewObjects;

#endregion

namespace ViewTo.Connector.Unity
{
	public class SoExploreData : ScriptableObject, IResultData
	{
		[SerializeField]
		List<double> _values;
		[SerializeField]
		string _content, _stage, _meta, _layout;
		[SerializeField]
		int _color;

		public double max, min;

		public List<double> values
		{
			get => _values;
		}

		public string content
		{
			get => _content;
		}

		public string stage
		{
			get => _stage;
		}

		public string meta
		{
			get => _meta;
		}

		public int color
		{
			get => _color;
		}

		public string layout
		{
			get => _layout;
		}

		public void Init(IResultData item)
		{
			_values = item.values;
			_color = item.color;

			_stage = item.stage;
			_content = item.content;
			_meta = item.meta;
			_layout = item.layout;

			foreach (var value in _values)
			{
				if (min == 0)
					min = value;
				else if (min > value)
					min = value;
				if (max < value)
					max = value;
			}
		}
	}
}