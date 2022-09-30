using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ViewObjects.Content;

namespace ViewObjects.Unity
{

	[ExecuteAlways]
	public class ResultCloudMono : ViewObjMono, IResultCloud
	{
		[SerializeField] CloudPoint[] cloudPoints;

		[SerializeField] string id;

		[SerializeField] [HideInInspector]
		List<string> targetOptions;

		[SerializeField] [HideInInspector]
		Vector3[] pointsV3;

		[SerializeField] [HideInInspector]
		List<ContentResultData> cloudData;

		[SerializeField] [HideInInspector]
		int point;

		/// <summary>
		///   returns the current game object position
		/// </summary>
		public Vector3 center
		{
			get => gameObject.transform.position;
		}

		public Vector3 activePoint
		{
			get => points.Valid(activeIndex) ? points[activeIndex].ToUnity() : Vector3.zero;
		}

		public int activeIndex
		{
			get => point;
			set => point = value;
		}

		public Vector3[] pointsAsVector
		{
			get
			{
				if (pointsV3.Valid())
					return pointsV3;

				if (cloudPoints.Valid())
				{
					pointsV3 = cloudPoints.ToUnity();
					return pointsV3;
				}

				return null;
			}
		}

		public List<string> targets
		{
			get => targetOptions.Valid() ? targetOptions : new List<string>();
		}

		void Awake()
		{
			gameObject.SetLayerRecursively(ViewMonoExt.CloudLayer);
		}
		public CloudPoint[] points
		{
			get => cloudPoints;
			set
			{
				cloudPoints = value;
				gameObject.transform.position = this.GetBounds().center;
				pointsV3 = value.ToUnity();
			}
		}

		public int count
		{
			get => this.GetCount();
		}

		public string ViewId
		{
			get => id;
			set => id = value;
		}

		public List<IResultData> data
		{
			get => cloudData.Valid() ? cloudData.Cast<IResultData>().ToList() : new List<IResultData>();
			set
			{
				if (!value.Valid())
					return;

				cloudData = new List<ContentResultData>();
				var targetNames = new HashSet<string>();
				foreach (var item in value)
				{
					AddResultData(item);
					targetNames.Add(item.content);
				}

				targetOptions = targetNames.ToList();
			}
		}

		public void AddResultData(IResultData value)
		{
			cloudData ??= new List<ContentResultData>();
			cloudData.Add(new ContentResultData(value.values, value.stage, value.content, value.color, value.meta, value.layout));
		}
	}

}