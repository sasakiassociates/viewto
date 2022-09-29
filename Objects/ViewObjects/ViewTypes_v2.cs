using System.Collections.Generic;

namespace ViewObjects
{

	public interface IStreamReference
	{
		/// <summary>
		///   A list of reference ids to use for connection speckle data into view to
		/// </summary>
		public List<string> References { get; set; }
	}

	public interface IViewStudy_v2 : IViewStudy_v2<IViewObj>
	{ }

	public interface IViewStudy_v2<TObject> : INameable, IId
	{
		/// <summary>
		///   A list of <typeparamref name="TObject" /> objects to group in a study
		/// </summary>
		public List<TObject> Objects { get; set; }
	}

	#region Viewer Objects

	public interface IViewerLayout_v2
	{
		/// <summary>
		///   Setup of viewers to use with each layout type
		/// </summary>
		public List<ViewerDirection> Viewers { get; }
	}

	public interface IViewerSystem_v2<TLayout> where TLayout : IViewerLayout_v2
	{
		/// <summary>
		///   The group of <typeparamref name="TLayout" /> targeted to be used during the analysis
		/// </summary>
		public List<TLayout> Layouts { get; set; }

		/// <summary>
		///   A list of cloud ids that can be used with this bundle
		/// </summary>
		public List<string> Clouds { get; set; }
	}

	#endregion

	#region View Cloud Objects

	public interface IViewCloudRef_v2 : IId, IStreamReference
	{ }

	public interface IViewCloud_v2 : IId
	{
		/// <summary>
		///   The cloud of points to use
		/// </summary>
		public CloudPoint[] Points { get; }
	}

	public interface IResultCloud_v2 : IResultCloud_v2<IResultCloudData>
	{ }

	public interface IResultCloud_v2<TData> : IViewCloud_v2 where TData : IResultCloudData
	{
		/// <summary>
		///   The view analysis data gathered
		/// </summary>
		public List<TData> Data { get; }
	}

	public interface IResultCloudData : IResultCloudMetaData
	{

		/// <summary>
		///   the raw values gathered
		/// </summary>
		List<int> Values { get; }
	}

	public interface IResultCloudMetaData
	{
		/// <summary>
		///   The <see cref="IContent" /> associated with these results. Includes the name, id, and stage
		/// </summary>
		public IContentOption Option { get; }

		/// <summary>
		///   The <see cref="IViewerLayout_v2" /> used to gather the data
		/// </summary>
		string Layout { get; }
	}

	#endregion

	#region View Content Objects

	/// <summary>
	///   Grouping object for bundling the objects
	/// </summary>
	public interface IContents<TContent> where TContent : IContent
	{
		public List<TContent> Contents { get; set; }
	}

	/// <summary>
	///   basic view content type object
	/// </summary>
	public interface IContent : INameable, IId
	{
		/// <summary>
		///   A list of references to be used for this view content
		/// </summary>
		public List<string> References { get; }

		/// <summary>
		///   The style of view content
		/// </summary>
		public ContentType ContentType { get; }
	}

	#endregion

}