using ViewObjects;
using ViewTo.Events.Args;

namespace ViewTo.Events.Prime
{
	internal sealed class CsvDataArgs : CommandDataArgs
	{
		public readonly string data;

		public CsvDataArgs(string data, string message = null)
		{
			this.data = data;
			success = data.Valid();
			this.message = message.Valid() ? message : success ? "CSV was primed" : "CSV failed in priming";
		}
	}
}