using System;
using System.Collections.Generic;
using ViewObjects.Cloud;

namespace ViewObjects.References
{

	public class ViewObjectReference : IReferenceObject, IViewObject
	{
		/// <summary>
		/// </summary>
		public ViewObjectReference()
		{ }

		/// <summary>
		/// </summary>
		/// <param name="references"></param>
		/// <param name="type"></param>
		/// <param name="viewId"></param>
		/// <param name="viewName"></param>
		public ViewObjectReference(List<string> references, Type type, string viewId = null, string viewName = null)
		{
			Type = type;
			ViewName = viewName;
			References = references;
			ViewId = ObjUtils.CheckIfValidId(viewId);
		}

		/// <inheritdoc />
		public string ViewId { get; protected set; }

		/// <inheritdoc />
		public string ViewName { get; set; }

		/// <inheritdoc />
		public Type Type { get; protected set; }

		/// <inheritdoc />
		public List<string> References { get; protected set; } = new();
	}

	public abstract class ViewObjectReference<TObj> : ViewObjectReference where TObj : IViewObject
	{
		/// <summary>
		/// </summary>
		public ViewObjectReference()
		{ }

		/// <summary>
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="references"></param>
		public ViewObjectReference(TObj obj, List<string> references)
		{
			References = references;

			if (obj == null)
			{
				return;
			}

			Type = obj.GetType();

			if (typeof(IId).IsAssignableFrom(Type) && obj is IId i)
			{
				ViewId = i.ViewId;
			}

			if (typeof(INameable).IsAssignableFrom(Type) && obj is INameable n)
			{
				ViewId = n.ViewName;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="references"></param>
		/// <param name="viewId"></param>
		/// <param name="viewName"></param>
		public ViewObjectReference(List<string> references, string viewId = null, string viewName = null)
		{
			Type = typeof(TObj);
			ViewName = viewName;
			References = references;
			ViewId = ObjUtils.CheckIfValidId(viewId);
		}
	}

	public class CloudReference : ViewObjectReference<ViewCloud>
	{
		/// <inheritdoc />
		public CloudReference(ViewCloud obj, List<string> references) : base(obj, references)
		{ }

		/// <inheritdoc />
		public CloudReference(List<string> references, string viewId = null, string viewName = null) : base(references, viewId, viewName)
		{ }
	}

	public class ResultCloudReference : ViewObjectReference<ResultCloud>
	{
		/// <inheritdoc />
		public ResultCloudReference(ResultCloud obj, List<string> references) : base(obj, references)
		{ }

		/// <inheritdoc />
		public ResultCloudReference(List<string> references, string viewId = null, string viewName = null) : base(references, viewId, viewName)
		{ }
	}

	public class ContentReference : ViewObjectReference<Content>
	{

		/// <inheritdoc />
		public ContentReference(Content obj, List<string> references) : base(obj, references) => ContentType = obj.ContentType;

		/// <inheritdoc />
		public ContentReference(List<string> references, ContentType type, string viewId = null, string viewName = null) : base(references, viewId, viewName) =>
			ContentType = type;

		/// <summary>
		/// </summary>
		public ContentType ContentType { get; }
	}
}