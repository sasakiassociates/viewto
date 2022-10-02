using System.Collections.Generic;
using System.Linq;
using ViewObjects;

namespace ViewTo.Cmd
{
	/// <summary>
	/// <para>Searches through a list of <see cref="IResultCloudData"/> to find a given object with a matching id and stage</para>
	/// </summary>
	internal class TryGetValuesCmd : ICmdWithArgs<ValuesRawForExplorerArgs>
	{
		/// <summary>
		/// id from the content
		/// </summary>
		readonly string contentId;

		/// <summary>
		/// stage the data is linked with
		/// </summary>
		readonly ResultStage stage;

		/// <summary>
		/// the list of data to search through
		/// </summary>
		readonly IReadOnlyCollection<IResultCloudData> data;

		public ValuesRawForExplorerArgs args { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data">data to search through</param>
		/// <param name="contentId">id of the content to find</param>
		/// <param name="stage">the analysis stage to find</param>
		public TryGetValuesCmd(List<IResultCloudData> data, string contentId, ResultStage stage)
		{
			this.data = data;
			this.stage = stage;
			this.contentId = contentId;

			args = new ValuesRawForExplorerArgs();
		}

		public void Run()
		{
			if (data != null && data.Any() && !string.IsNullOrEmpty(contentId))
			{
				foreach (var d in data)
				{
					if (d.Option.Id.Equals(contentId) && d.Option.Stage == stage)
					{
						args = new ValuesRawForExplorerArgs(d.Values);
						break;
					}
				}
			}
		}

	}
}