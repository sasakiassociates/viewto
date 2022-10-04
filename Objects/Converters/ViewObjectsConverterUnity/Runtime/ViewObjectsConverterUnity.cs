using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Speckle.ConnectorUnity.Converter;
using Speckle.Core.Models;
using UnityEngine;
using ViewObjects.Converter.Script;
using ViewObjects.Unity;

namespace ViewObjects.Converter.Unity
{

	public class ViewObjectsConverterUnity : ScriptableSpeckleConverter
	{

		/// <summary>
		/// this does not work correctly or should not be returning the serialized list. 
		/// </summary>
		/// <returns></returns>
		public override List<ComponentConverter> StandardConverters()
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