using System;

namespace ViewObjects
{
	public interface ICloud
	{
		CloudPoint[] points { get; set; }
	}

	public interface IValidate
	{
		bool isValid { get; }
	}

	public interface IId
	{
		string viewID { get; set; }
	}

	public interface INameable
	{
		string viewName { get; set; }
	}

	public interface IMetaShell
	{
		string objId { get; }
		Type objType { get; }
	}
}