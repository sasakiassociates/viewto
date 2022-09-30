using System.Collections.Generic;
using ViewTo.Events.Args;

namespace ViewTo.Primers
{

	internal abstract class PrimedData : IPrimedData
	{
		public List<PrimeProcessArgs> args { get; } = new List<PrimeProcessArgs>();
	}
}