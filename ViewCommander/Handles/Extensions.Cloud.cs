using System;
using System.Linq;
using ViewObjects;

namespace ViewTo
{
	public static partial class ViewCoreExtensions
	{

		public static ContentInfo GetTarget(this IResultCloud obj, string targetByIdOrName, ResultStage stage)
		{
			if (obj == default || !obj.Data.Valid() || !targetByIdOrName.Valid())
			{
				return null;
			}

			var guid = Guid.Parse(targetByIdOrName);

			foreach (var data in obj.Data)
			{
				if (data?.Option == null)
				{
					continue;
				}

				if (data.Option.Stage == stage && guid != default ? data.Option.Id.Equals(targetByIdOrName) : data.Option.Name.Equals(targetByIdOrName))
				{
					return new ContentInfo(data.Option);
				}
			}

			return null;
		}

		public static ContentInfo GetTarget(this IResultCloud obj, string targetByIdOrName)
		{
			if (obj == default || !obj.Data.Valid() || !targetByIdOrName.Valid())
			{
				return null;
			}

			var guid = Guid.Parse(targetByIdOrName);

			foreach (var data in obj.Data)
			{
				if (data?.Option == null)
				{
					continue;
				}

				if (guid != default ? data.Option.Id.Equals(targetByIdOrName) : data.Option.Name.Equals(targetByIdOrName))
				{
					return new ContentInfo(data.Option);
				}
			}

			return null;
		}

		public static bool HasTarget(this IResultCloud obj, string targetByIdOrName, ResultStage stage)
		{
			if (obj == default || !obj.Data.Valid() || !targetByIdOrName.Valid())
			{
				return false;
			}

			// is an id so search for that
			return Guid.TryParse(targetByIdOrName, out _) ?
				obj.Data.Any(x => x.Option.Id.Equals(targetByIdOrName) && x.Option.Stage == stage)
				: obj.Data.Any(x => x.Option.Name.Equals(targetByIdOrName) && x.Option.Stage == stage);
		}

		public static bool HasTarget(this IResultCloud obj, string targetByIdOrName)
		{
			if (obj == default || !obj.Data.Valid() || !targetByIdOrName.Valid())
			{
				return false;
			}

			// is an id so search for that
			if (Guid.TryParse(targetByIdOrName, out _))
			{
				return obj.Data.Any(x => x.Option.Id.Equals(targetByIdOrName));
			}
			else
			{
				return obj.Data.Any(x => x.Option.Name.Equals(targetByIdOrName));
			}
		}
	}
}