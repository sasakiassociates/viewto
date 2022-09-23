using System.Collections.Generic;

namespace ViewObjects
{
	public abstract class Container
	{ }

	public class PixelDataContainer : Container, IResultCloudData
	{

		/// <inheritdoc />
		public string ContentId { get; set; }

		/// <inheritdoc />
		public ResultStage Stage { get; set; }

		/// <inheritdoc />
		public string Layout { get; set; }

		/// <inheritdoc />
		public List<int> Values { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public PixelDataContainer()
		{ }
	}
}