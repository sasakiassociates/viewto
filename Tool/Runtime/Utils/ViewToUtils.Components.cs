#region

using UnityEngine;
using ViewObjects;

#endregion

namespace ViewTo.Connector.Unity
{
	public static partial class ViewToUtils
	{

		// private static TObj Set<TObj>(this ViewerMono mono) where TObj : ViewerComponent
		// {
		//
		//   TObj obj = (TObj)mono.gameObject.GetComponent(typeof(TObj));
		//   if (obj != null)
		//     MonoHelper.SafeDestroy(obj);
		//
		//   obj = (TObj)mono.gameObject.AddComponent(typeof(TObj));
		//   obj.Attach(mono);
		//
		//   return obj;
		// }

		public const int TargetLayer = 7;
		public const int BlockerLayer = 8;
		public const int DesignLayer = 6;
		public const int CloudLayer = 9;

		public static bool Compare(this ViewColor a, Color32 b) => a.R == b.r && a.G == b.g && a.B == b.b;

		public static int GetLayerMask(this IViewContent value)
		{
			return value switch
			{
				ITargetContent _ => TargetLayer,
				IBlockerContent _ => BlockerLayer,
				IDesignContent _ => DesignLayer,
				_ => 0
			};
		}

		public static int GetCullingMask(this ResultStage value)
		{
			return value switch
			{
				ResultStage.Potential => (1 << TargetLayer) | (1 << CloudLayer),
				ResultStage.Existing => (1 << TargetLayer) | (1 << BlockerLayer) | (1 << CloudLayer),
				ResultStage.Proposed => (1 << TargetLayer) | (1 << BlockerLayer) | (1 << DesignLayer) | (1 << CloudLayer),
				_ => -1
			};
		}

		public static int GetCullingMask(this RigStage value)
		{
			return value switch
			{
				RigStage.Target => 1 << TargetLayer,
				RigStage.Blocker => (1 << TargetLayer) | (1 << BlockerLayer),
				RigStage.Design => (1 << TargetLayer) | (1 << BlockerLayer) | (1 << DesignLayer),
				_ => -1
			};
		}
	}
}