using Speckle.Core.Kits;
using Speckle.Core.Models;
using Speckle.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using ViewObjects.Clouds;
using ViewObjects.Contents;

namespace ViewObjects.Speckle
{

  /// <summary>
  /// </summary>
  public class ResultLayer : ViewObjectBase, IResultLayer
  {

    const string TARGET_NAME = "Target_Name";
    const string TARGET_ID = "Target_Id";
    const string CONTENT_ID = "Content_Id";
    const string CONTENT_NAME = "Content_Name";
    const string STAGE = nameof(ViewContentType);

    /// <summary>
    /// </summary>
    public ResultLayer()
    { }

    /// <summary>
    ///   Schema constructor
    /// </summary>
    /// <param name="values">Pixel values connected to each point of a cloud</param>
    /// <param name="targetId">GUID from the Content</param>
    /// <param name="targetName"></param>
    /// <param name="contentId"></param>
    /// <param name="contentName"></param>
    /// <param name="stage">Result stage flag</param>
    /// <param name="name"></param>
    /// <param name="count">Viewer count meta data</param>
    [SchemaInfo("View Result Data", "Container of data for a view cloud", ViewObject.Schema.Category, "Objects")]
    public ResultLayer(List<int> values, string targetId, string targetName, string contentId, string contentName, ViewContentType stage, int count)
    {
      this.values = values;
      this.count = count;
      this.info = new ContentOption(new ContentInfo(targetId, targetName), new ContentInfo(contentId, contentName), stage);
    }

    /// <summary>
    ///   Schema constructor
    /// </summary>
    /// <param name="values">Pixel values connected to each point of a cloud</param>
    /// <param name="option"></param>
    /// <param name="count">Viewer count meta data</param>
    [SchemaInfo("View Result Data", "Container of data for a view cloud", ViewObject.Schema.Category, "Objects")]
    public ResultLayer(List<int> values, ContentOption option, int count )
    {
      this.values = values;
      this.count = count;
      this.info = option;
    }

    /// <inheritdoc />
    public int count { get; set; }

    /// <inheritdoc />
    [Chunkable] public List<int> values { get; set; } = new List<int>();

    /// <inheritdoc />
    [JsonIgnore] public ContentOption info
    {
      get => new ContentOption
      (
        new ContentInfo((string)this[TARGET_ID], (string)this[TARGET_NAME]),
        new ContentInfo((string)this[CONTENT_ID], (string)this[CONTENT_NAME]),
        (ViewContentType)Enum.Parse(typeof(ViewContentType), (string)this[STAGE])
      );
      set
      {
        this[STAGE] = value.stage.ToString();
        this[TARGET_ID] = value.target.appId;
        this[TARGET_NAME] = value.target.name;
        this[CONTENT_ID] = value.content.appId;
        this[CONTENT_NAME] = value.content.name;
      }
    }
  }

}
