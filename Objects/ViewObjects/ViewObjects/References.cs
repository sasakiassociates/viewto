using System;
using System.Collections.Generic;

namespace ViewObjects
{

	[Serializable]
	public class ViewObjectReference : IReferenceObject, IViewObject
	{
		/// <summary>
		/// </summary>
		public ViewObjectReference()
		{ }

		/// <summary>
		/// </summary>
		public ViewObjectReference(List<string> references)
		{
			References = references;
		}

		/// <summary>
		/// </summary>
		/// <param name="references"></param>
		/// <param name="viewId"></param>
		/// <param name="viewName"></param>
		public ViewObjectReference(List<string> references, string viewId, string viewName = null)
		{
			ViewName = viewName;
			References = references;
			ViewId = ObjUtils.CheckIfValidId(viewId);
		}

		/// <summary>
		/// </summary>
		/// <param name="references"></param>
		/// <param name="type"></param>
		/// <param name="viewId"></param>
		/// <param name="viewName"></param>
		public ViewObjectReference(List<string> references, Type type, string viewId, string viewName = null)
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
		/// <param name="references"></param>
		/// <param name="viewId"></param>
		/// <param name="viewName"></param>
		protected ViewObjectReference(List<string> references, string viewId, string viewName = null) :
			base(references, viewId, viewName)
		{
			Type = typeof(TObj);
		}

		/// <summary>
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="references"></param>
		protected ViewObjectReference(TObj obj, List<string> references) : base(references)
		{
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
				ViewName = n.ViewName;
			}
		}

	}

	[Serializable]
	public class ViewCloudReference : ViewObjectReference<ViewCloud>
	{
		/// <inheritdoc />
		public ViewCloudReference(ViewCloud obj, List<string> references) : base(obj, references)
		{ }

		/// <inheritdoc />
		public ViewCloudReference(List<string> references, string viewId, string viewName = null) : base(references, viewId, viewName)
		{ }
	}

	[Serializable]
	public class ResultCloudReference : ViewObjectReference<ResultCloud>
	{
		/// <inheritdoc />
		public ResultCloudReference(ResultCloud obj, List<string> references) : base(obj, references)
		{ }

		/// <inheritdoc />
		public ResultCloudReference(List<string> references, string viewId, string viewName = null) : base(references, viewId, viewName)
		{ }
	}

	[Serializable]
	public class ContentReference : ViewObjectReference<Content>, IContentInfo
	{

		/// <inheritdoc />
		public ContentReference(Content obj, List<string> references) : base(obj, references)
		{
			ContentType = obj.ContentType;
		}

		/// <inheritdoc />
		public ContentReference(List<string> references, ContentType type, string viewId, string viewName = null) : base(references, viewId, viewName) =>
			ContentType = type;

		/// <summary>
		/// </summary>
		public ContentType ContentType { get; }
	}
}