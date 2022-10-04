using System.Collections.Generic;
using ViewObjects;
using ViewTo.Receivers;

namespace ViewTo.Cmd
{
	internal class InitializeRig : ICmd
	{
		IReadOnlyList<IContent> _contents;
		IReadOnlyList<IViewCloud> _clouds;
		IReadOnlyList<IViewerSystem> _viewers;

		StudyObjectPrimer _receiver;

		public InitializeRig(IRig rig, IReadOnlyList<IContent> contents, IReadOnlyList<IViewCloud> clouds, IReadOnlyList<IViewerSystem> viewers)
		{
			_contents = contents;
			_clouds = clouds;
			_viewers = viewers;
			_receiver = new StudyObjectPrimer();
		}

		public void Execute()
		{
			// check rig is real
			// create all params 
			// store them in rig
			// build rig
		}
	}
}