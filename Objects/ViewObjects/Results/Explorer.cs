using System.Collections.Generic;
using System.Linq;
using ViewObjects.Clouds;
using ViewObjects.Contents;
using ViewObjects.Studies;

namespace ViewObjects.Results;

public class Explorer : IExplorer
{

  /// <summary>
  /// </summary>
  public Explorer()
  { }

  /// <inheritdoc />
  public IViewStudy Source { get; internal set; }

  /// <inheritdoc />
  public IResultCloud Cloud { get; internal set; }

  /// <inheritdoc />
  public ExplorerSettings Settings { get; set; } = new();

  /// <inheritdoc />
  public ContentInfo ActiveContent { get; set; }

  // /// <inheritdoc />
  public List<ContentOption> Options { get; internal set; }

  /// <inheritdoc />
  public List<IResultCloudData> Data => Cloud?.Data ?? new List<IResultCloudData>();

  /// <inheritdoc />
  public void Load(IViewStudy viewObj)
  {
    if(viewObj == default(object) || !viewObj.CanExplore())
    {
      return;
    }

    Source = viewObj;
    Cloud = viewObj.FindObject<ResultCloud>();

    if(Cloud == null)
    {
      return;
    }

    Settings ??= new ExplorerSettings();

    Options = Cloud.Data.Where(x => x != null).Select(x => x.Option).Cast<ContentOption>().ToList();
    var opt = Options.FirstOrDefault();
    ActiveContent = new ContentInfo(opt);
  }

  public bool IsValid => Source != null && Cloud != null && ActiveContent != null;
}
