namespace ViewTo.Events.Args
{

	public abstract class CommandDataArgs : AEventArgs
	{
		public string message { get; protected set; }

		public virtual bool success { get; protected set; }
	}

	public abstract class PrimeProcessArgs : AEventArgs
	{
		public string message { get; protected set; }

		public virtual bool success { get; protected set; }
	}

}