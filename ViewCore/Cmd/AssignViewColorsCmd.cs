using ViewTo.Commands;

namespace ViewTo.Cmd
{
	public class AssignViewColorsCmd : ICmd
	{

		public void Run()
		{
			throw new System.NotImplementedException();
		}

		// public static List<TContent> GetContents<TContent>(this IViewContentBundle_v1 obj) where TContent : IViewContent_v1
		// {
		// 	var res = new List<TContent>();
		//
		// 	foreach (var item in obj.contents)
		// 		if (item is TContent casted)
		// 			res.Add(casted);
		//
		// 	return res;
		// }
		//
		// public static int GetContentCount<TContent>(this IViewContentBundle_v1 obj) where TContent : IViewContent_v1
		// {
		// 	var res = 0;
		//
		// 	foreach (var item in obj.contents)
		// 		if (item is TContent)
		// 			res++;
		//
		// 	return res;
		// }
		//
		// public static void AssignColors(this IViewContentBundle_v1 obj)
		// {
		// 	var colors = obj.contents.CreateBundledColors();
		//
		// 	var colorIndex = 0;
		// 	foreach (var c in obj.contents)
		// 		c.viewColor = colors[colorIndex++];
		// }
		//
		// public static List<ViewColor> CreateBundledColors(this ICollection content)
		// {
		// 	var colorSet = new HashSet<ViewColor>();
		// 	var r = new Random();
		//
		// 	while (colorSet.Count < content.Count)
		// 	{
		// 		var b = new byte[3];
		// 		r.NextBytes(b);
		// 		var tempColor = new ViewColor(b[0], b[1], b[2], 255);
		// 		colorSet.Add(tempColor);
		// 	}
		//
		// 	return colorSet.ToList();
		// }
	}

}