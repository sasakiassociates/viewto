using System;
using UnityEngine;

namespace ViewObjects.Unity
{
	[ExecuteAlways]
	public class ViewCloudMono : ViewObjMono, IViewCloud
	{

		[SerializeField] string id;

		[SerializeField] CloudPoint[] cloudPoints;

		void Awake()
		{
			id ??= Guid.NewGuid().ToString();
		}

		public string viewID
		{
			get => id;
			set => id = value;
		}

		public CloudPoint[] points
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