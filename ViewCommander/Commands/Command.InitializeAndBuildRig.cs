using System.Collections.Generic;
using System.Linq;
using ViewObjects;
using ViewObjects.Clouds;
using ViewObjects.Common;
using ViewObjects.Contents;
using ViewObjects.Systems;
using ViewObjects.Systems.Layouts;

namespace ViewTo.Cmd
{

  internal class InitializeAndBuildRig : ICmdWithArgs<SimpleResultArgs>
  {
    private IReadOnlyList<IViewCloud> _clouds;
    private IReadOnlyList<IContent> _contents;
    private IRig _rig;
    private IReadOnlyList<IViewer> _viewers;

    public InitializeAndBuildRig(IRig rig, IReadOnlyList<IContent> contents, IReadOnlyList<IViewCloud> clouds, IReadOnlyList<IViewer> viewers)
    {
      _rig = rig;
      _contents = contents;
      _clouds = clouds;
      _viewers = viewers;
    }

    public SimpleResultArgs args { get; private set; }

    public void Execute()
    {
      if(_rig == default(object))
      {
        args = new SimpleResultArgs(false, "Rig is not valid for initializing");
        return;
      }

      var rigParams = new List<RigParameters>();

      var layouts = new List<ILayout>();
      foreach(var v in _viewers)
      {
        // if its not a global object
        if(v is IViewerLinked vl)
        {
          rigParams.Add(CreateRigParams(vl.Layouts, _contents, vl.Clouds));
        }
        else
        {
          layouts.AddRange(v.Layouts);
        }
      }

      rigParams.Insert(0, CreateRigParams(layouts, _contents, _clouds));
      _rig.Initialize(rigParams);
      _rig.Build();
      args = new SimpleResultArgs(true, $"Rig Built! {rigParams.Count}");
    }

    /// <summary>
    ///   Goes through all the viewers and places
    ///   <para>
    ///     this is where the view colors that are not meant to be shared globally would be separated, but for now we use
    ///     all the colors
    ///   </para>
    /// </summary>
    /// <param name="viewers"></param>
    /// <param name="contents"></param>
    /// <param name="clouds"></param>
    public RigParameters CreateRigParams(
      IEnumerable<ILayout> viewers,
      IEnumerable<IContent> contents,
      IEnumerable<IId> clouds
    )
    {
      return CreateRigParams(viewers, contents, clouds.Where(x => x != default(object)).Select(x => x.ViewId).ToList());
    }

    /// <summary>
    ///   Goes through all the viewers and places
    ///   <para>
    ///     this is where the view colors that are not meant to be shared globally would be separated, but for now we use
    ///     all the colors
    ///   </para>
    /// </summary>
    /// <param name="viewers"></param>
    /// <param name="contents"></param>
    /// <param name="clouds"></param>
    public RigParameters CreateRigParams(
      IEnumerable<ILayout> viewers,
      IEnumerable<IContent> contents,
      IEnumerable<string> clouds
    )
    {
      return new RigParameters(
        clouds.ToList(),
        contents.Where(x => x != null && x.ContentType == ContentType.Potential).Select(x => x.Color).ToList(),
        viewers.ToList()
      );
    }
  }

}
