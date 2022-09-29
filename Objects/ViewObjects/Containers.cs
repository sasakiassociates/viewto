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

	[Serializable]
	public class ResultCloudData : Container, IResultCloudData
	{

		/// <summary>
		/// </summary>
		public ResultCloudData()
		{ }

		/// <inheritdoc />
		public IContentOption Option { get; set; }

		/// <inheritdoc />
		public string Layout { get; set; }

		/// <inheritdoc />
		public List<int> Values { get; set; }
	}

}