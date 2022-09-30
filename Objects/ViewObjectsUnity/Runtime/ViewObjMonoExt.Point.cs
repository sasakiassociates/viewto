using System.Linq;
using UnityEngine;
using ViewObjects.Cloud;

namespace ViewObjects.Unity
{
	public static partial class ViewObjMonoExt
	{
		public static CloudShell GetShell(this IViewCloud obj) => new(obj, obj.ViewId, obj.count);

		public static Vector3[] GetPointsAsVectors(this IViewCloud obj) => obj.points.Valid() ? obj.points.ToUnity() : null;

		public static CloudPoint[] ToView(Vector3[] value, string[] meta)
		{
			var items = new CloudPoint[value.Length];

			for (var i = 0; i < value.Length; i++)
				items[i] = value[i].ToView(meta[i]);

			return items;
		}

		public static CloudPoint ToView(this Vector3 value) => new(value.x, value.z, value.y);

		public static CloudPoint ToView(this Vector3 value, string meta) => new(value.x, value.z, value.y) { meta = meta };

		public static Vector3[] ToUnity(this CloudPoint[] value, out string[] meta)
		{
			meta = null;
			meta = new string[value.Length];
			var points = new Vector3[value.Length];
			for (var i = 0; i < value.Length; i++)
			{
				points[i] = value[i].ToUnity();
				meta[i] = value[i].meta;
			}

			return points;
		}

		public static Vector3[] ToUnity(this CloudPoint[] value)
		{
			var points = new Vector3[value.Length];
			for (var i = 0; i < value.Length; i++)
				points[i] = value[i].ToUnity();

			return points;
		}

		public static Vector3 GetCenter(this Vector3[] points)
		{
			if (!points.Valid())
				return Vector3.zero;

			return new Vector3(
				points.Average(x => x.x),
				points.Average(x => x.y),
				points.Average(x => x.z)
			);
		}

		public static Bounds GetBounds(this IViewCloud cloud)
		{
			// rest center 
			Bounds bounds = default;
			var v3 = cloud.points.ToUnity();

			if (v3.Valid())
			{
				bounds = new Bounds(v3.GetCenter(), Vector3.zero);

				foreach (var point in v3)
					bounds.Encapsulate(point);
			}

			return bounds;
		}

		public static Bounds GetBounds(this IViewCloud cloud, Vector3 center)
		{
			// rest center 
			Bounds bounds = default;
			var v3 = cloud.points.ToUnity();

			if (v3.Valid())
			{
				bounds = new Bounds(center, Vector3.zero);

				foreach (var point in v3)
					bounds.Encapsulate(point);
			}

			return bounds;
		}

    /// <summary>
    ///   flips value
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Vector3 ToUnity(this CloudPoint value) => new((float)value.x, (float)value.z, (float)value.y);
	}
}