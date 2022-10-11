using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Speckle.Core.Kits;
using Speckle.Core.Models;

namespace ViewObjects.Speckle
{
	/// <summary>
	///   View Content Object adapted to <see cref="Base" />
	/// </summary>
	public class ContentReference : ViewObjectReference<ViewObjects.ContentReference>, IContent
	{

		const string TYPE = "Content_Type";

		/// <summary>
		/// 
		/// </summary>
		public ContentReference()
		{ }

		/// <inheritdoc />
		[SchemaInfo("View Content", "Simple Object type for structuring geometry for a view study", ViewObject.Schema.Category, "Objects")]
		public ContentReference(ViewObjects.ContentReference obj) : base(obj)
		{
			Type = obj.Type;
			References = obj.References;
			ContentType = obj.ContentType;
		}

		/// <inheritdoc />
		public ContentReference(ContentType type, List<string> references, string viewId, string viewName = null) : base(references, viewId, viewName)
		{
			ContentType = type;
		}

		/// <inheritdoc />
		[JsonIgnore] public ContentType ContentType
		{
			get { return(ContentType)Enum.Parse(typeof(ContentType), (string)this[TYPE]); }
			set { this[TYPE] = value.ToString(); }
		}

		/// <inheritdoc />
		public ViewColor Color { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public bool Equals(IContent obj)
		{
			return obj != default && ViewId.Equals(obj.ViewId) && ContentType == obj.ContentType;
		}

	}
}