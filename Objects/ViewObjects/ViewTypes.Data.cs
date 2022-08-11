using System.Collections.Generic;

namespace ViewObjects
{
	public interface IResultData : IPixelResults, IContentResults, ILayoutResults
	{ }

	public interface IPixelResults
	{
		List<double> values { get; }
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