using Speckle.Newtonsoft.Json;

namespace ViewObjects.Speckle
{
	public class DesignContentBaseV1 : ViewContentBaseV1
	{

		public DesignContentBaseV1()
		{ }

		[JsonIgnore] public override bool IsValid => base.IsValid && ViewName.Valid();
	}
}