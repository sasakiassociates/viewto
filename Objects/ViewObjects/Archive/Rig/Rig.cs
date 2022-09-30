using System.Collections.Generic;
using System.Linq;

namespace ViewObjects
{

	public class RigV1 : IRig_v1, IValidate
	{
		public List<IRigParam_v1> globalParams { get; set; }

		public List<ViewColor> globalColors { get; set; }

		public Dictionary<string, CloudPoint[]> clouds { get; set; }

		public bool IsValid
		{
			get => globalParams.Valid() && globalColors.Valid() && clouds != null && clouds.Any();
		}
	}
}