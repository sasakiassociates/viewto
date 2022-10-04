using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ViewObjects.Unity
{

	[ExecuteAlways]
	public class ResultCloud : ViewObjectMono, IResultCloud
	{
		[SerializeField] CloudPoint[] cloudPoints;

		[SerializeField] string id;

		[SerializeField] [HideInInspector] List<string> targetOptions;

		[SerializeField] [HideInInspector] Vector3[] pointsV3;

		[SerializeField] [HideInInspector] List<ResultCloudData> cloudData;

		[SerializeField] [HideInInspector] int point;

		/// <summary>
		///   returns the current game object position
		/// </summary>
		public Vector3 center
		{
			get => gameObject.transform.position;
		}

		public Vector3 activePoint
		{
			get => Points.Valid(activeIndex) ? Points[activeIndex].ToUnity() : Vector3.zero;
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
			gameObject.SetLayerRecursively(this.GetLayerMask());
		}

		public CloudPoint[] Points
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

		public List<IResultCloudData> Data
		{
			get => cloudData.Valid() ? cloudData.Cast<IResultCloudData>().ToList() : new List<IResultCloudData>();
			set
			{
				if (!value.Valid())
					return;

				cloudData = new List<ResultCloudData>();
				var targetNames = new HashSet<string>();
				foreach (var item in value)
				{
					AddResultData(item);
					targetNames.Add(item.Option.Name);
				}

				targetOptions = targetNames.ToList();
			}
		}

		public void AddResultData(IResultCloudData value)
		{
			cloudData ??= new List<ResultCloudData>();
			cloudData.Add(new ResultCloudData()
			{
				Layout = value.Layout,
				Values = value.Values,
				Option = value.Option
			});
		}

	}

}