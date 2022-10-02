using System.Collections.Generic;
using System.Linq;

namespace ViewTo.Cmd
{
	internal interface ICommandArgs
	{
		public bool IsValid();
	}

	internal readonly struct ValuesForExplorerArgs : ICommandArgs
	{

		public readonly double[] values;

		public ValuesForExplorerArgs(double[] values)
		{
			this.values = values;
		}

		public bool IsValid() => values != null && values.Any();
	}

	internal readonly struct ValuesRawForExplorerArgs : ICommandArgs
	{

		public readonly int[] values;

		public ValuesRawForExplorerArgs(IEnumerable<int> values)
		{
			this.values = values.ToArray();
		}

		public bool IsValid() => values != null && values.Any();
	}

	internal readonly struct PointWithValueArgs : ICommandArgs
	{
		public readonly int index;

		public PointWithValueArgs(int index)
		{
			this.index = index;
		}

		public bool IsValid() => index >= 0;
	}

}