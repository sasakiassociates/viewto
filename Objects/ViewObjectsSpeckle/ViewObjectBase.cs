using System;
using System.Collections.Generic;
using Speckle.Core.Models;

namespace ViewObjects.Speckle
{

	/// <summary>
	/// </summary>
	public abstract class ViewObjectBase : Base, IViewObject
	{

		/// <summary>
		/// </summary>
		public ViewObjectBase()
		{ }

		/// <summary>
		///   Returns <see cref="ViewObjectBase" />
		/// </summary>
		public override string speckle_type
		{
			get => GetType().ToString();
		}
	}

	/// <summary>
	/// </summary>
	public class ViewObjectReferenceBase : ViewObjectBase, IReferenceObject
	{
		/// <summary>
		/// </summary>
		public ViewObjectReferenceBase()
		{ }

		/// <summary>
		/// </summary>
		/// <param name="references"></param>
		/// <param name="type"></param>
		/// <param name="viewId"></param>
		/// <param name="viewName"></param>
		public ViewObjectReferenceBase(List<string> references, Type type, string viewId = null, string viewName = null)
		{
			Type = type;
			ViewName = viewName;
			References = references;
			ViewId = ObjUtils.CheckIfValidId(viewId);
		}

		/// <inheritdoc />
		public List<string> References { get; set; }

		/// <inheritdoc />
		public string ViewName { set; get; }

		/// <inheritdoc />
		public string ViewId { get; set; }

		/// <inheritdoc />
		public Type Type { get; set; }
	}

	/// <summary>
	/// </summary>
	/// <typeparam name="TObj"></typeparam>
	public abstract class ViewObjectReferenceBase<TObj> : ViewObjectReferenceBase where TObj : IViewObject
	{
		/// <summary>
		/// </summary>
		public ViewObjectReferenceBase()
		{ }

		/// <summary>
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="references"></param>
		public ViewObjectReferenceBase(TObj obj, List<string> references)
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
		public ViewObjectReferenceBase(List<string> references, string viewId = null, string viewName = null)
		{
			Type = typeof(TObj);
			ViewName = viewName;
			References = references;
			ViewId = ObjUtils.CheckIfValidId(viewId);
		}
	}

}