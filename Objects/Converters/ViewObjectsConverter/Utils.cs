using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Objects.Geometry;
using Speckle.Core.Models;
using ViewObjects.Cloud;
using ViewObjects.Speckle;

namespace ViewObjects.Converter
{
	public static class Utils
	{
		public static IResultData Convert(this IResultData @object) => new ResultPixelBaseV1
		{
			values = @object.values,
			content = @object.content,
			color = @object.color,
			meta = @object.meta,
			stage = @object.stage,
			layout = @object.layout
		};

		public static List<IResultData> Convert(this List<IResultData> @object)
		{
			return @object.Valid() ? @object.Select(data => data.Convert()).ToList() : new List<IResultData>();
		}

		public static List<CloudPointBase> ToSpeckle(this List<CloudPoint> @object)
		{
			return!@object.Valid() ?
				new List<CloudPointBase>() :
				@object.Select(p => new CloudPointBase
				{
					x = p.x, y = p.y, z = p.z, meta = p.meta
				}).ToList();
		}

		public static List<CloudPoint> ToView(this List<CloudPointBase> points)
		{
			return!points.Valid() ? new List<CloudPoint>() :
				points.Select(p => new CloudPoint
				{
					x = p.x, y = p.y, z = p.z, meta = p.meta
				}).ToList();
		}

		public static List<CloudShell> ToView(this List<ViewCloudBaseV1> @base)
		{
			return!@base.Valid() ? new List<CloudShell>() :
				@base.Select(c => c.ToView()).ToList();
		}

		public static CloudShell ToView(this ViewCloudBaseV1 baseV1) => new CloudShell(baseV1, baseV1.id, baseV1.count);

		public static CloudPoint ToView(this Point p) => new CloudPoint(p.x, p.y, p.z);

		public static TBase SearchForType<TBase>(this Base obj, bool recursive) where TBase : Base
		{
			if (obj is TBase simpleCast) return simpleCast;

			foreach (var member in obj.GetMemberNames())
			{
				var nestedObj = obj[member];

				// 1. Direct cast for object type 
				if (nestedObj.IsBase(out TBase memberCast))
				{
					return memberCast;
				}

				// 2. Check if member is base type
				if (nestedObj.IsBase(out var nestedBase))
				{
					var objectToFind = nestedBase.SearchForType<TBase>(recursive);

					if (objectToFind != default)
					{
						return objectToFind;
					}
				}
				else if (nestedObj.IsList(out List<object> nestedList))
				{
					foreach (var listObj in nestedList)
					{
						if (listObj.IsBase(out TBase castedListObjectType))
						{
							return castedListObjectType;
						}

						// if not set to recursive we dont look through any other objects
						if (!recursive) continue;

						// if its not a base object we turn around
						if (!listObj.IsBase(out Base nestedListBase)) continue;

						var objectToFind = nestedListBase.SearchForType<TBase>(true);
						if (objectToFind != default)
						{
							return objectToFind;
						}
					}
				}
			}

			return null;
		}

		public static bool IsList(this object obj, out List<object> list)
		{
			list = new List<object>();

			if (obj.IsList())
			{
				list = ((IEnumerable)obj).Cast<object>().ToList();
			}

			return list.Any();
		}

		public static bool IsList(this object obj) => obj != null && obj.GetType().IsList();

		public static bool IsList(this Type type)
		{
			if (type == null) return false;

			return typeof(IEnumerable).IsAssignableFrom(type) && !typeof(IDictionary).IsAssignableFrom(type) && type != typeof(string);
		}

		public static bool IsBase<TBase>(this object value, out TBase @base) where TBase : Base
		{
			@base = default;

			if (value != null && !value.GetType().IsSimpleType() && value is TBase o)
			{
				@base = o;
			}

			return @base != null;
		}

		public static bool IsBase(this object value, out Base @base)
		{
			@base = null;

			if (value != null && !value.GetType().IsSimpleType() && value is Base o)
			{
				@base = o;
			}

			return @base != null;
		}

	}
}