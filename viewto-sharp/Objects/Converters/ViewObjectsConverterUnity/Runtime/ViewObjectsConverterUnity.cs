using Speckle.ConnectorUnity.Converter;
using System.Collections.Generic;

namespace ViewObjects.Converter.Unity
{

  public class ViewObjectsConverterUnity : ScriptableConverter
  {

    /// <summary>
    /// this does not work correctly or should not be returning the serialized list. 
    /// </summary>
    /// <returns></returns>
    protected override List<ComponentConverter> GetDefaultConverters()
    {
      var items = new List<ComponentConverter>
      {
        CreateInstance<MeshComponentConverter>(),
        CreateInstance<PolylineComponentConverter>(),
        CreateInstance<PointComponentConverter>(),
        CreateInstance<PointCloudComponentConverter>(),
        CreateInstance<View3DComponentConverter>(),
        CreateInstance<BrepComponentConverter>()
      };
      return items;
    }
  }

}
