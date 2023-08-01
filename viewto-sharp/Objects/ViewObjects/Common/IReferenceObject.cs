using System;

namespace ViewObjects.Common
{

  public interface IReferenceObject : IId, INameable, IStreamReference
  {
    public Type Type { get; }
  }

}
