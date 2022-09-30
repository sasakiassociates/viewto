using System;
using System.Collections.Generic;

namespace ViewObjects
{
	public abstract class Container
	{ }

	[Serializable]
	public class ContentOption : Container, IContentOption
	{

		/// <inheritdoc />
		public string Id { get; set; }

		/// <inheritdoc />
		public string Name { get; set; }

		/// <inheritdoc />
		public ResultStage Stage { get; set; }
	}

	[Serializable]
	public class ResultCloudData : Container, IResultCloudData
	{

		/// <summary>
		/// </summary>
		public ResultCloudData()
		{ }

		public ResultCloudData(List<int> values, IContentOption option, string layout)
		{
			this.Values = values;
			this.Option = option;
			this.Layout = layout;
		}

		/// <inheritdoc />
		public IContentOption Option { get; set; }

		/// <inheritdoc />
		public string Layout { get; set; }

		/// <inheritdoc />
		public List<int> Values { get; set; }

	}

}