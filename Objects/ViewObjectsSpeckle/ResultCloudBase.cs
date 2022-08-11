using System.Collections.Generic;
using Speckle.Newtonsoft.Json;

namespace ViewObjects.Speckle
{

  public class ResultCloudBase : ViewCloudBase
  {

    public ResultCloudBase()
    { }

    [JsonIgnore]
    public override bool isValid => base.isValid && data.Valid();

    public List<IResultData> data { get; set; }
  }
}
