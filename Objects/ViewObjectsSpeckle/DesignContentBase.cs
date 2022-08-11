using Speckle.Newtonsoft.Json;

namespace ViewObjects.Speckle
{
	public class DesignContentBase : ViewContentBase
	{

		public DesignContentBase()
		{ }
		
		[JsonIgnore]
		public override bool isValid => base.isValid && viewName.Valid();
	}
}