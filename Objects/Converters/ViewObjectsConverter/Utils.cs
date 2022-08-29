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
		public static bool IsList(this object @object) => @object != null && @object.GetType().IsList();

		public static bool IsList(this Type type)
		{
			if (type == null) return false;

			return typeof(IEnumerable).IsAssignableFrom(type) && !typeof(IDictionary).IsAssignableFrom(type) && type != typeof(string);
		}

		public static bool IsBase(this object value, out Base @base)
		{
			@base = null;

			if (!value.GetType().IsSimpleType() && value is Base o)
				@base = o;

			return @base != null;
		}

		public static IResultData Convert(this IResultData @object) => new ResultPixelBase
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

		public static List<CloudShell> ToView(this List<ViewCloudBase> @base)
		{
			return!@base.Valid() ? new List<CloudShell>() :
				@base.Select(c => c.ToView()).ToList();
		}

		public static CloudShell ToView(this ViewCloudBase @base) => new CloudShell(@base, @base.id, @base.count);

		public static CloudPoint ToView(this Point p) => new CloudPoint(p.x, p.y, p.z);
	}
}