using System.Collections.Generic;

namespace ViewObjects
{

	public interface IStreamReference
	{
		/// <summary>
		/// A list of reference ids to use for connection speckle data into view to
		/// </summary>
		public List<string> References { get; set; }
	}

	public interface IViewStudy_v2<TObject> : INameable, IId
	{
		/// <summary>
		/// A list of <typeparamref name="TObject"/> objects to group in a study
		/// </summary>
		public List<TObject> Objects { get; set; }
	}

	#region Viewer Objects

	public interface IViewerLayout_v2
	{
		/// <summary>
		/// Setup of viewers to use with each layout type
		/// </summary>
		public List<ViewerDirection> Viewers { get; }
	}

	public interface IViewerSystem_v2<TLayout> where TLayout : IViewerLayout_v2
	{
		/// <summary>
		/// The group of <typeparamref name="TLayout"/> targeted to be used during the analysis 
		/// </summary>
		public List<TLayout> Layouts { get; set; }

		/// <summary>
		/// A list of cloud ids that can be used with this bundle
		/// </summary>
		public List<string> Clouds { get; set; }
	}

	#endregion

	#region View Cloud Objects

	public interface IViewCloudData_v2 : IViewCloud_v2
	{
		/// <summary>
		/// The cloud of points to use
		/// </summary>
		public CloudPoint[] points { get; set; }

		/// <summary>
		/// The view analysis data gathered
		/// </summary>
		public List<IResultCloudData> data { get; set; }
	}

	public interface IViewCloud_v2 : IId, IStreamReference
	{ }

	public interface IResultCloudData
	{
		/// <summary>
		/// The <see cref="IContent"/> id associated with these results
		/// </summary>
		string ContentId { get; }

		/// <summary>
		/// The result stage used during analysis 
		/// </summary>
		ResultStage Stage { get; }

		/// <summary>
		/// The <see cref="IViewerLayout_v2"/> used to gather the data
		/// </summary>
		string Layout { get; }

		/// <summary>
		/// the raw values gathered 
		/// </summary>
		List<int> Values { get; }

	}

	#endregion

	#region View Content Objects

	/// <summary>
	/// Grouping object for bundling the objects
	/// </summary>
	public interface IContents<TContent> where TContent : IContent
	{
		public List<TContent> Contents { get; set; }
	}

	/// <summary>
	/// basic view content type object
	/// </summary>
	public interface IContent : INameable, IId
	{
		/// <summary>
		/// A list of references to be used for this view content
		/// </summary>
		public List<string> References { get; }

		/// <summary>
		/// The style of view content
		/// </summary>
		public ContentType ContentType { get; }

	}

	#endregion

}