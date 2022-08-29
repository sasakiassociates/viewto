namespace ViewObjects.Unity
{
	public static class ViewMonoExt
	{
		public const int TargetLayer = 7;

		public const int BlockerLayer = 8;

		public const int DesignLayer = 6;

		public const int CloudLayer = 9;

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
	}
}