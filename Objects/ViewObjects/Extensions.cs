using System.Collections.Generic;
using System.Linq;

namespace ViewObjects
{
	public static class Extensions
	{
		public static List<IContentOption> FindObject(this IResultCloud<IResultCloudData> obj) => new();

		public static TObj FindObject<TObj>(this IViewStudy obj) where TObj : IViewObject
		{
			TObj viewObj = default;
			if (!obj.Objects.Any())
			{
				return viewObj;
			}

			foreach (var o in obj.Objects)
			{
				if (o is TObj casted)
				{
					viewObj = casted;
					break;
				}
			}

			return viewObj;
		}

		public static List<TObj> FindObjects<TObj>(this IViewStudy obj) where TObj : IViewObject
		{
			List<TObj> viewObjs = new();

			if (obj.Objects.Any())
			{
				foreach (var o in obj.Objects)
				{
					if (o is TObj casted)
					{
						viewObjs.Add(casted);
					}
				}
			}

			return viewObjs;
		}

		/// <summary>
		///   <para>
		///     Checks if a view study has the correct data necessary to run a view study.
		///     The correct data is at least one <see cref="IViewCloud" />, one <see cref="IViewer" />,
		///     one <see cref="Content" /> marked as <see cref="ContentType.Target" /> and one marked as
		///     <see cref="ContentType.Existing" />
		///   </para>
		/// </summary>
		/// <param name="study">object to check</param>
		/// <returns></returns>
		public static bool CanRun(this IViewStudy study) =>
			study.Has<IViewCloud>() && study.Has<IViewer>() && study.Has(ContentType.Target) && study.Has(ContentType.Existing);

		/// <summary>
		///   <para>
		///     Checks if a view study has the correct object types needed to visualize data.
		///     The one object needed is a <see cref="IResultCloud" />
		///   </para>
		/// </summary>
		/// <param name="study">object to check</param>
		/// <returns></returns>
		public static bool CanExplore(this IViewStudy study) => study.Has<IResultCloud>();

		/// <summary>
		///   <para>Checks for a specific type of <see cref="IContent" /> object that fits the <see cref="ContentType" /></para>
		/// </summary>
		/// <param name="study">study to check</param>
		/// <param name="type">type of content to find</param>
		/// <param name="id">optional id to use</param>
		/// <returns></returns>
		public static bool Has(this IViewStudy study, ContentType type, string id = "")
		{
			if (study?.Objects == null || !study.Objects.Any())
			{
				return false;
			}

			foreach (var obj in study.GetAll<IContent>())
			{
				if (obj.ContentType != type)
				{
					continue;
				}

				// found at least one but no name was passed in 
				if (!id.Valid())
				{
					return true;
				}

				if (obj is IId objWithID && objWithID.ViewId.Equals(id))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		///   <para>Checks for a specific type of object to search for</para>
		/// </summary>
		/// <param name="study">study to check</param>
		/// <param name="id">optional id to use</param>
		public static bool Has<TObjType>(this IViewStudy study, string id = "")
		{
			if (study?.Objects == null || !study.Objects.Any())
			{
				return false;
			}

			foreach (var obj in study.Objects.OfType<TObjType>())
			{
				// found at least one but no name was passed in 
				if (!id.Valid())
				{
					return true;
				}

				if (obj is IId objWithID && objWithID.ViewId.Equals(id))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		///   <para>Gets a list of objects of a specific type</para>
		/// </summary>
		/// <param name="study">study to check</param>
		public static List<TObjType> GetAll<TObjType>(this IViewStudy study) => study.Objects.OfType<TObjType>().ToList();

		/// <summary>
		///   <para>Gets a specific type of object to search for</para>
		/// </summary>
		/// <param name="study">study to check</param>
		/// <param name="id">optional id to use</param>
		public static TObjType Get<TObjType>(this IViewStudy study, string id = "")
		{
			foreach (var obj in study.Objects.OfType<TObjType>())
			{
				// found at least one but no name was passed in 
				if (!id.Valid())
				{
					return obj;
				}

				if (obj is IId objWithID && objWithID.ViewId.Equals(id))
				{
					return obj;
				}
			}

			return default;
		}

		/// <summary>
		/// Shortcut for getting all available content types in a study
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="targetCount"></param>
		/// <param name="existingCount"></param>
		/// <param name="proposedCount"></param>
		public static void ContentCount(this IViewStudy obj, out int targetCount, out int existingCount, out int proposedCount)
		{
			targetCount = 0;
			existingCount = 0;
			proposedCount = 0;

			if (obj != default)
			{
				var contents = obj.GetAll<IContent>();

				targetCount = contents.Count(x => x.ContentType == ContentType.Target);
				existingCount = contents.Count(x => x.ContentType == ContentType.Existing);
				proposedCount = contents.Count(x => x.ContentType == ContentType.Proposed);
			}
		}

		/// <summary>
		/// Shortcut for getting all available content types 
		/// </summary>
		/// <param name="contents"></param>
		/// <param name="targetCount"></param>
		/// <param name="existingCount"></param>
		/// <param name="proposedCount"></param>
		public static void ContentCount(this List<IContent> contents, out int targetCount, out int existingCount, out int proposedCount)
		{
			targetCount = contents.Count(x => x.ContentType == ContentType.Target);
			existingCount = contents.Count(x => x.ContentType == ContentType.Existing);
			proposedCount = contents.Count(x => x.ContentType == ContentType.Proposed);
		}
	}

}