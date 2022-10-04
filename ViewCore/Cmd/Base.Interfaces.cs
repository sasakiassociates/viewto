namespace ViewTo.Cmd
{

	internal interface ICmd
	{
		public void Execute();
	}

	internal interface ICmdWithArgs<out TArgs> : ICmd where TArgs : CommandArgs
	{
		public TArgs args { get; }
	}

	internal interface ICommandArgs
	{
		public bool IsValid();

		public string Message { get; }
	}

}