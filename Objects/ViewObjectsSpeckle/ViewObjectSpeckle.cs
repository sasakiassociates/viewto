using System;

namespace ViewObjects.Speckle
{
	/// <summary>
	///   Static values for view objects in speckle
	/// </summary>
	public static class ViewObjectSpeckle
	{
		internal static string CheckIfValidId(this string valueId) =>
			!string.IsNullOrEmpty(valueId) && Guid.TryParse(valueId, out _) ? valueId : Guid.NewGuid().ToString();

		internal static class Schema
		{

			internal const string Category = "ViewObjects";
		}
	}
}