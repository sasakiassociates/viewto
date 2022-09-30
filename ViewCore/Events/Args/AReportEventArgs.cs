namespace ViewTo.Events.Args
{
	public abstract class AReportEventArgs : AEventArgs
	{
		public string message { get; protected set; }
	}
}