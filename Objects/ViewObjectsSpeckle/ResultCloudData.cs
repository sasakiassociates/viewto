using System;
using System.Collections.Generic;
using Speckle.Core.Kits;
using Speckle.Core.Models;
using Speckle.Newtonsoft.Json;

namespace ViewObjects.Speckle
{
	/// <summary>
	/// </summary>
	public class ResultCloudData : ViewObjectBase, IResultCloudData
	{

		const string NAME = nameof(IContent);
		const string ID = nameof(IId);
		const string STAGE = nameof(ResultStage);

		/// <summary>
		/// </summary>
		public ResultCloudData()
		{ }

		/// <summary>
		///   Schema constructor
		/// </summary>
		/// <param name="values">Pixel values connected to each point of a cloud</param>
		/// <param name="contentId">GUID from the Content</param>
		/// <param name="stage">Result stage flag</param>
		/// <param name="name"></param>
		/// <param name="layout">Viewer layout meta data</param>
		[SchemaInfo("View Result Data", "Container of data for a view cloud", ViewObject.Schema.Category, "Objects")]
		public ResultCloudData(List<int> values, string contentId, ResultStage stage, string name = null, string layout = null)
		{
			Values = values;
			Layout = layout;
			Option = new ContentOption
				{ Id = contentId, Stage = stage, Name = name };
		}

		/// <summary>
		///   Schema constructor
		/// </summary>
		/// <param name="values">Pixel values connected to each point of a cloud</param>
		/// <param name="option"></param>
		/// <param name="layout">Viewer layout meta data</param>
		[SchemaInfo("View Result Data", "Container of data for a view cloud", ViewObject.Schema.Category, "Objects")]
		public ResultCloudData(List<int> values, IContentOption option, string layout = null)
		{
			Values = values;
			Layout = layout;
			Option = option;
		}

		/// <inheritdoc />
		public string Layout { get; set; }

		/// <inheritdoc />
		[DetachProperty] [Chunkable] public List<int> Values { get; set; } = new List<int>();

		/// <inheritdoc />
		[JsonIgnore] public IContentOption Option
		{
			get =>
				new ContentOption
				{
					Id = (string)this[ID],
					Name = (string)this[NAME],
					Stage = (ResultStage)Enum.Parse(typeof(ResultStage), (string)this[STAGE])
				};
			set
			{
				this[NAME] = value.Name;
				this[STAGE] = value.Stage.ToString();
				this[ID] = value.Id;
			}
		}
	}
}