using System;
using System.Collections.Generic;
using UnityEngine;
using ViewObjects.Clouds;

namespace ViewObjects.Unity
{
	[ExecuteAlways]
	public class ViewCloud : ViewObjectMono, IViewCloud
	{

		[SerializeField] string id;

		[SerializeField] CloudPoint[] cloudPoints;

		[SerializeField, HideInInspector] List<string> _reference;

		public List<string> Reference
		{
			get => _reference;
			set => _reference = value;
		}

		void Awake()
		{
			id ??= Guid.NewGuid().ToString();
		}

		public string ViewId
		{
			get => id;
			set => id = value;
		}

		public CloudPoint[] Points
		{
			get => cloudPoints;
			set => cloudPoints = value;
		}

		public int count
		{
			get => this.GetCount();
		}
	}

}