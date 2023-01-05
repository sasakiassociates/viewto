using System.Collections.Generic;

namespace ViewObjects.Contents;

public interface IContentObjects<TObj>
{
  /// <summary>
  ///   Group of <typeparamref name="TObj" /> objects
  /// </summary>
  public List<TObj> Objects { get; }
}
