using System.Collections.Generic;

namespace ViewObjects
{
	public interface IResultData_v1 : IPixelResults, IContentResults, ILayoutResults
	{ }

	public interface IPixelResults
	{
		List<int> values { get; }
	}

	public interface ILayoutResults
	{
		string layout { get; }
	}

	public interface IContentResults
	{
		string content { get; }

		string stage { get; }

		string meta { get; }

		int color { get; }
	}
}