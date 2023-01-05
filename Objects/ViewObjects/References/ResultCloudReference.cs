using System;
using System.Collections.Generic;
using ViewObjects.Clouds;

namespace ViewObjects.References;

[Serializable]
public class ResultCloudReference : ViewObjectReference<ResultCloud>
{
  /// <inheritdoc />
  public ResultCloudReference(ResultCloud obj, List<string> references) : base(obj, references)
  { }

  /// <inheritdoc />
  public ResultCloudReference(List<string> references, string viewId, string viewName = null) : base(references, viewId, viewName)
  { }
}
