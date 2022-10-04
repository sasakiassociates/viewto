using System;
using System.Collections.Generic;
using System.Linq;
using ViewObjects;

namespace ViewTo.Receivers
{
	public class StudyObjectPrimer
	{

		public void AssignContentColors(ref List<IContent> contents)
		{
			var colorSet = new HashSet<ViewColor>();
			var r = new Random();

			while (colorSet.Count < contents.Count)
			{
				var b = new byte[3];
				r.NextBytes(b);
				var tempColor = new ViewColor(b[0], b[1], b[2], 255);
				colorSet.Add(tempColor);
			}

			var index = 0;
			foreach (var c in colorSet)
			{
				contents[index++].Color = c;
			}
		}

		/// <summary>
		/// Goes through all the viewers and places
		/// <para>this is where the view colors that are not meant to be shared globally would be separated, but for now we use all the colors</para>
		/// </summary>
		/// <param name="study"></param>
		/// <param name="isGlobal"></param>
		public RigParameters CreateRigParams(IViewStudy study, bool isGlobal)
		{
			return CreateRigParams(study.GetAll<IViewerSystem>(),
			                       study.GetAll<IContent>(),
			                       study.GetAll<IViewCloud>(),
			                       isGlobal
			);
		}

		/// <summary>
		/// Goes through all the viewers and places
		/// <para>this is where the view colors that are not meant to be shared globally would be separated, but for now we use all the colors</para>
		/// </summary>
		/// <param name="viewers"></param>
		/// <param name="contents"></param>
		/// <param name="clouds"></param>
		/// <param name="isGlobal"></param>
		public RigParameters CreateRigParams(
			IEnumerable<IViewerSystem> viewers,
			IEnumerable<IContent> contents,
			IEnumerable<IId> clouds,
			bool isGlobal
		)
		{
			return new RigParameters(
				clouds.Where(x => x != default).Select(x => x.ViewId).ToList(),
				contents.Where(x => x != null && x.ContentType == ContentType.Target).Select(x => x.Color).ToList(),
				viewers.Where(x => x != null && x.IsGlobal == isGlobal).ToList()
			);
		}

	}
}