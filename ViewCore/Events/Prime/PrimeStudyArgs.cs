using System;
using System.Collections.Generic;
using ViewTo.Events.Args;
using ViewTo.Primers;

namespace ViewTo.Events.Prime
{

	internal sealed class PrimeStudyArgs : PrimeProcessArgs
	{
		public readonly List<PrimeViewerBundleArgs> bundleArgs;
		public readonly List<PrimeCloudArgs> cloudArgs;
		public readonly List<PrimeContentCaseArgs> contentArgs;

		public PrimeStudyArgs(IPrimedData data)
		{
			cloudArgs = new List<PrimeCloudArgs>();
			bundleArgs = new List<PrimeViewerBundleArgs>();
			contentArgs = new List<PrimeContentCaseArgs>();

			foreach (var arg in data.args)
				switch (arg)
				{
					case PrimeCloudArgs a:
						cloudArgs.Add(a);
						break;
					case PrimeContentCaseArgs a:
						contentArgs.Add(a);
						break;
					case PrimeViewerBundleArgs a:
						bundleArgs.Add(a);
						break;
					default:
						Console.WriteLine("Args type is not supported");
						break;
				}
		}
	}
}