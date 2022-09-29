using System.Collections.Generic;
using ViewTo.Events.Args;

namespace ViewTo.Primers
{
	internal interface IPrimedData
	{
		public List<PrimeProcessArgs> args { get; }
	}
}