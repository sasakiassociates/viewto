using ViewObjects.Common;

namespace ViewObjects.References;

public interface IViewObjectReference : IViewObject
{
  public IReferenceObject Reference { get; set; }
}
