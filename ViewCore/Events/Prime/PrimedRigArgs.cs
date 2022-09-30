using ViewTo.Events.Args;

namespace ViewTo.Events.Prime
{
	internal sealed class PrimedRigArgs : PrimeProcessArgs
	{
		public readonly RigV1 RigV1;

		public PrimedRigArgs(RigV1 rigV1) => RigV1 = rigV1;
	}
}