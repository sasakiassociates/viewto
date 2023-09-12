using System.Collections.Generic;
using ViewObjects.Clouds;
using ViewObjects.Contents;
using ViewObjects.Systems;
using ViewTo.Receivers;

namespace ViewTo.Cmd
{

  internal class CanStudyRun : ICmdWithArgs<SimpleResultArgs>
  {
    private IReadOnlyList<ICloud> _clouds;
    private IReadOnlyList<IContext> _contents;

    private StudyObjectValidator _receiver;
    private IReadOnlyList<IViewer> _viewers;

    public CanStudyRun(IReadOnlyList<IContext> contents, IReadOnlyList<ICloud> clouds, IReadOnlyList<IViewer> viewers)
    {
      _contents = contents;
      _clouds = clouds;
      _viewers = viewers;
      _receiver = new StudyObjectValidator();
    }

    public SimpleResultArgs args { get; private set; }

    public void Execute()
    {
      // check all lists are populated correctly
      if(!_receiver.DataIsValid(_contents, out var message)
         || !_receiver.DataIsValid(_clouds, out message)
         || !_receiver.DataIsValid(_viewers, out message))
      {
        args = new SimpleResultArgs(false, message);
      }

      // check if all the correct clouds are there
      if(_receiver.CompareClouds(_viewers, _clouds, out message))
      {
        args = new SimpleResultArgs(false, message);
      }

      // check that we right objects to run a study
      args = new SimpleResultArgs(_receiver.CheckData(_contents, _clouds, _viewers, out message), message);
    }
  }

}
