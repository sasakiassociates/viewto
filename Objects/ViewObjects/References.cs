using System;
using System.Collections.Generic;
using ViewObjects.Cloud;

namespace ViewObjects.References
{

	public class ViewObjectReference : IReferenceObject, IViewObject
	{
		/// <summary>
		/// 
		/// </summary>
		public ViewObjectReference()
		{ }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="references"></param>
		/// <param name="type"></param>
		/// <param name="viewId"></param>
		/// <param name="viewName"></param>
		public ViewObjectReference(List<string> references, Type type, string viewId = null, string viewName = null)
		{
			this.Type = type;
			this.ViewName = viewName;
			this.References = references;
			this.ViewId = ObjUtils.CheckIfValidId(viewId);
		}

		/// <inheritdoc />
		public string ViewId { get; protected set; }

		/// <inheritdoc />
		public string ViewName { get; set; }

		/// <inheritdoc />
		public Type Type { get; protected set; }

		/// <inheritdoc />
		public List<string> References { get; protected set; } = new List<string>();

	}

	public abstract class ViewObjectReference<TObj> : ViewObjectReference where TObj : IViewObject
	{
		/// <summary>
		/// 
		/// </summary>
		public ViewObjectReference()
		{ }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="references"></param>
		public ViewObjectReference(TObj obj, List<string> references)
		{
			this.References = references;

			if (obj == null)
				return;

			this.Type = obj.GetType();

			if (typeof(IId).IsAssignableFrom(Type) && obj is IId i)
				this.ViewId = i.ViewId;
			if (typeof(INameable).IsAssignableFrom(Type) && obj is INameable n)
				this.ViewId = n.ViewName;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="references"></param>
		/// <param name="viewId"></param>
		/// <param name="viewName"></param>
		public ViewObjectReference(List<string> references, string viewId = null, string viewName = null)
		{
			this.Type = typeof(TObj);
			this.ViewName = viewName;
			this.References = references;
			this.ViewId = ObjUtils.CheckIfValidId(viewId);
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
		/// <summary>
		/// 
		/// </summary>
		public ContentType ContentType { get; }

		/// <inheritdoc />
		public ContentReference(Content obj, List<string> references) : base(obj, references)
		{
			this.ContentType = obj.ContentType;
		}

		/// <inheritdoc />
		public ContentReference(List<string> references, ContentType type, string viewId = null, string viewName = null) : base(references, viewId, viewName)
		{
			this.ContentType = type;
		}

	}
}