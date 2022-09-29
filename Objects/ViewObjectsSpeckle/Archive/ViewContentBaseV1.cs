using System.Collections.Generic;
using Speckle.Core.Models;
using Speckle.Newtonsoft.Json;

namespace ViewObjects.Speckle
{

	/// <summary>
	/// 
	/// </summary>
	public abstract class ViewContentBaseV1 : ViewObjectBase_v1, IValidate, INameable
	{

		public ViewContentBaseV1()
		{ }

		[DetachProperty] public List<Base> objects { get; set; }

		[JsonIgnore] public virtual bool IsValid => objects != null;

		public string ViewName { get; set; }

		public ContentType contentType { get; set; }
	}
}