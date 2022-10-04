using System.Collections.Generic;
using System.Linq;
using ViewObjects;
using ViewTo.Cmd;

namespace ViewTo
{
	public static partial class ViewCoreExtensions
	{
		public static List<string> LoadStudyToRig(this IViewStudy study, ref IRig rig)
		{
			var contents = study.GetAll<IContent>();
			var clouds = study.GetAll<IViewCloud>();
			var viewers = study.GetAll<IViewer>();

			var reports = new List<string>();

			var sequence = new List<ICmd>()
			{
				new CanStudyRun(contents, clouds, viewers),
				new AssignViewColors(contents),
				new InitializeAndBuildRig(rig, contents, clouds, viewers),
			};

			foreach (var s in sequence)
			{
				s.Execute();

				if (s is ICmdWithArgs<CommandArgs> cmdWithArgs)
				{
					reports.Add(cmdWithArgs.args.Message);
				}
			}

			return reports;
		}

		/// <summary>
		/// <para>Checks if a view study has the correct data necessary to run a view study.
		/// The correct data is at least one <see cref="IViewCloud"/>, one <see cref="IViewer"/>,
		/// one <see cref="Content"/> marked as <see cref="ContentType.Target"/> and one marked as <see cref="ContentType.Existing"/>
		/// </para> 
		/// </summary>
		/// <param name="study">object to check</param>
		/// <returns></returns>
		public static bool CanRun(this IViewStudy study)
		{
			return study.Has<IViewCloud>() && study.Has<IViewer>() && study.Has(ContentType.Target) && study.Has(ContentType.Existing);
		}

		/// <summary>
		/// <para>Checks if a view study has the correct object types needed to visualize data.
		/// The one object needed is a <see cref="IResultCloud"/></para>
		/// </summary>
		/// <param name="study">object to check</param>
		/// <returns></returns>
		public static bool CanExplore(this IViewStudy study)
		{
			return study.Has<IResultCloud>();
		}

		/// <summary>
		/// <para>Checks for a specific type of <see cref="IContent"/> object that fits the <see cref="ContentType"/></para> 
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
		/// <para>Checks for a specific type of object to search for</para> 
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
		/// <para>Gets a list of objects of a specific type</para> 
		/// </summary>
		/// <param name="study">study to check</param>
		public static List<TObjType> GetAll<TObjType>(this IViewStudy study)
		{
			return study.Objects.OfType<TObjType>().ToList();
		}

		/// <summary>
		/// <para>Gets a specific type of object to search for</para> 
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

	}
}