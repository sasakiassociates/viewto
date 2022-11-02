namespace ViewObjects
{

  public class ViewCloud : IViewCloud, IViewObject
  {

    /// <summary>
    /// </summary>
    public ViewCloud()
    {
      ViewId = ObjUtils.InitGuid;
    }

    /// <inheritdoc />
    public string ViewId { get; set; }

    /// <inheritdoc />
    public CloudPoint[] Points { get; set; }
  }
}
