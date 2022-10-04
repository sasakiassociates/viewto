using System.Collections.Generic;
using ViewObjects;
using ViewTo.Receivers;

namespace ViewTo.Cmd
{
	internal class CanStudyRun : ICmdWithArgs<SimpleResultArgs>
	{
		IReadOnlyList<IContent> _contents;
		IReadOnlyList<IViewCloud> _clouds;
		IReadOnlyList<IViewerSystem> _viewers;

		StudyObjectValidator _receiver;

		public SimpleResultArgs args { get; private set; }

		public CanStudyRun(IReadOnlyList<IContent> contents, IReadOnlyList<IViewCloud> clouds, IReadOnlyList<IViewerSystem> viewers)
		{
			_contents = contents;
			_clouds = clouds;
			_viewers = viewers;
			_receiver = new StudyObjectValidator();
		}

		public void Execute()
		{
			// check all lists are populated correctly
			if (!_receiver.DataIsValid(_contents, out var message)
			    || !_receiver.DataIsValid(_clouds, out message)
			    || !_receiver.DataIsValid(_viewers, out message))
			{
				args = new SimpleResultArgs(false, message);
			}

			// check if all the correct clouds are there
			if (_receiver.CompareClouds(_viewers, _clouds, out message))
			{
				args = new SimpleResultArgs(false, message);
			}

			// check that we right objects to run a study
			args = new SimpleResultArgs(_receiver.CheckData(_contents, _clouds, _viewers, out message), message);
		}

	}
}