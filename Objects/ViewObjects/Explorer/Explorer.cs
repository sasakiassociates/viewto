namespace ViewObjects.Explorer
{
	public interface IExplorer
	{
		/// <summary>
		/// The active view study being used with the source cloud. 
		/// </summary>
		public IViewStudy study { get; }

		/// <summary>
		/// The heart and soul of the data being explored 
		/// </summary>
		public IResultCloud source { get; }

		/// <summary>
		/// Set of data settings for the explorer to use 
		/// </summary>
		public ExplorerSettings settings { get; }

		/// <summary>
		/// Load in a new cloud for the explorer to explore!
		/// </summary>
		/// <param name="viewObj"></param>
		public void Load(IResultCloud viewObj);

		/// <summary>
		/// Load in a new view study for the explorer to explore!
		/// </summary>
		/// <param name="viewObj">The view study to load in</param>
		/// <param name="loadFirstResultCloud">Optional toggle to set if any result clouds are found in the study</param>
		public void Load(IViewStudy viewObj, bool loadFirstResultCloud = true);

		/// <summary>
		/// Pass in a new set of explorer settings for the explorer to use
		/// </summary>
		/// <param name="settings"></param>
		public void Set(ExplorerSettings settings);

		/// <summary>
		/// Set the active point index of the cloud being explored
		/// </summary>
		/// <param name="index"></param>
		public void Set(int index);

		/// <summary>
		/// Retrieves the active point result data
		/// </summary>
		/// <returns></returns>
		public ResultPoint GetResultPoint();
	}

}