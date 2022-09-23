using System.Collections.Generic;
using Speckle.Core.Kits;
using Speckle.Core.Models;

namespace ViewObjects.Speckle
{
	/// <summary>
	/// 
	/// </summary>
	public class ResultPixelBase_v2 : ViewObjectBase_v2, IResultCloudData
	{

		/// <inheritdoc />
		[DetachProperty] [Chunkable] public List<int> Values { get; set; } = new List<int>();

		/// <inheritdoc />
		public string ContentId { get; set; }

		/// <inheritdoc />
		public ResultStage Stage { get; set; }

		/// <inheritdoc />
		public string Layout { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public ResultPixelBase_v2()
		{ }

		/// <summary>
		///  Schema constructor
		/// </summary>
		/// <param name="values">Pixel values connected to each point of a cloud</param>
		/// <param name="contentId">GUID from the Content</param>
		/// <param name="stage">Result stage flag</param>
		/// <param name="layout">Viewer layout meta data</param>
		[SchemaInfo("View Result Data", "Container of data for a view cloud", ViewObjectSpeckle.Schema.Category, "Objects")]
		public ResultPixelBase_v2(List<int> values, string contentId, ResultStage stage, string layout = null)
		{
			this.Values = values;
			this.ContentId = contentId;
			this.Stage = stage;
			this.Layout = layout;
		}

	}
}