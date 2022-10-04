using System;
using System.Collections.Generic;

namespace ViewObjects
{

	#region Base Types

	public interface IViewObject
	{ }

	public interface IViewObjectReference : IViewObject
	{
		public IReferenceObject Reference { get; set; }
	}

	#endregion

	#region References

	public interface IStreamReference
	{
		/// <summary>
		///   A list of reference ids to use for connection speckle data into view to
		/// </summary>
		public List<string> References { get; }
	}

	public interface IReferenceObject : IId, INameable, IStreamReference
	{
		public Type Type { get; }
	}

	#endregion

	public interface IRig
	{

		/// <summary>
		///   Handle building the different viewer types
		/// </summary>
		public void Build();

		/// <summary>
		///   Handle all the data the rig needs to run a view study
		/// </summary>
		/// <param name="parameters"></param>
		public void Initialize(List<RigParameters> parameters);
	}

	#region View Objects

	public interface IViewStudy : IViewStudy<IViewObject>
	{ }

	/// <summary>
	///   An interface for organizing <typeparamref name="TObject" /> types processing view studies
	/// </summary>
	/// <typeparam name="TObject"></typeparam>
	public interface IViewStudy<TObject> : INameable, IId where TObject : IViewObject
	{
		/// <summary>
		///   A list of <typeparamref name="TObject" /> objects to group in a study
		/// </summary>
		public List<TObject> Objects { get; set; }
	}

	public interface IViewerLayout
	{
		/// <summary>
		///   Setup of viewers to use with each layout type
		/// </summary>
		public List<ViewDirection> Viewers { get; }
	}

	public interface IViewer : IViewer<IViewerLayout>
	{ }

	/// <summary>
	///   An interface for organizing <typeparamref name="TLayout" /> types for <see cref="IViewStudy" />
	/// </summary>
	/// <typeparam name="TLayout"></typeparam>
	public interface IViewer<TLayout> where TLayout : IViewerLayout
	{
		/// <summary>
		///   The group of <typeparamref name="TLayout" /> targeted to be used during the analysis
		/// </summary>
		public List<TLayout> Layouts { get; set; }
	}

	public interface IViewerLinked<TLayout> : IViewer<TLayout> where TLayout : IViewerLayout
	{
		/// <summary>
		///   A list of cloud ids that can be used with this bundle
		/// </summary>
		public List<string> Clouds { get; set; }
	}

	public interface IViewerLinked : IViewer
	{
		/// <summary>
		///   A list of cloud ids that can be used with this bundle
		/// </summary>
		public List<string> Clouds { get; set; }
	}

	#endregion

	#region Cloud Objects

	public interface IViewCloud : IId
	{
		/// <summary>
		///   The cloud of points to use
		/// </summary>
		public CloudPoint[] Points { get; set; }
	}

	public interface IResultCloud : IResultCloud<IResultCloudData>
	{ }

	public interface IResultCloud<TData> : IViewCloud where TData : IResultCloudData
	{
		/// <summary>
		///   The view analysis data gathered
		/// </summary>
		public List<TData> Data { get; set; }
	}

	#region Cloud Result Data

	/// <summary>
	///   The main structure for organizing result data
	/// </summary>
	public interface IResultCloudData : IResultCloudMetaData
	{

		/// <summary>
		///   the raw values gathered
		/// </summary>
		List<int> Values { get; set; }
	}

	/// <summary>
	///   The meta data associated with the result values
	/// </summary>
	public interface IResultCloudMetaData
	{
		/// <summary>
		///   The <see cref="IContent" /> associated with these results. Includes the name, id, and stage
		/// </summary>
		public IContentOption Option { get; set; }

		/// <summary>
		///   The <see cref="IViewerLayout" /> used to gather the data
		/// </summary>
		string Layout { get; set; }
	}

	public interface IContentOption
	{
		/// <summary>
		///   Id linked to <see cref="IContent" />
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		///   Name of the Target Content
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///   the stage to use for
		/// </summary>
		public ResultStage Stage { get; set; }
	}

	#endregion

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
		///   The style of view content
		/// </summary>
		public ContentType ContentType { get; }

		/// <summary>
		///   The color of the content group
		/// </summary>
		public ViewColor Color { get; set; }
	}

	public interface IContentObjects<TObj>
	{
		/// <summary>
		///   Group of <typeparamref name="TObj" /> objects
		/// </summary>
		public List<TObj> Objects { get; }
	}

	#endregion

}