namespace ViewObjects
{

  public interface IValidate
  {
    bool IsValid { get; }
  }

  public interface IId
  {
    /// <summary>
    ///   View id in the format of <see cref="System.Guid" />
    /// </summary>
    string ViewId { get; }
  }

  public interface INameable
  {
    /// <summary>
    ///   Simple name for view objects
    /// </summary>
    string ViewName { get; set; }
  }

}
