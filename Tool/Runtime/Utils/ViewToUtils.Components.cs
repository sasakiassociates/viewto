#region

using UnityEngine;
using ViewObjects;
using ViewObjects.Unity;

#endregion

namespace ViewTo.Connector.Unity
{
	public static partial class ViewToUtils
	{
		public static bool Compare(this ViewColor a, Color32 b) => a.R == b.r && a.G == b.g && a.B == b.b;
		
		public static int GetCullingMask(this ResultStage value)
		{
			return value switch
			{
				ResultStage.Potential => (1 << ViewObject.TargetLayer) | (1 << ViewObject.CloudLayer),
				ResultStage.Existing => (1 << ViewObject.TargetLayer) | (1 << ViewObject.BlockerLayer) | (1 << ViewObject.CloudLayer),
				ResultStage.Proposed => (1 << ViewObject.TargetLayer) | (1 << ViewObject.BlockerLayer) | (1 << ViewObject.ProposedLayer) | (1 << ViewObject.CloudLayer),
				_ => -1
			};
		}

	
	}
}