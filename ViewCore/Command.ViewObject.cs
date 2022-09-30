using ViewObjects;

namespace ViewTo
{
	public static partial class Commander
	{
		public static string GetName(this IViewObject obj)
		{
			if (obj is INameable objName)
				return objName.ViewName;

			return"";
		}

		public static bool HasValidName(this IViewObject obj)
		{
			if (obj is INameable objName)
				return objName.ViewName.Valid();

			return false;
		}
	}
}