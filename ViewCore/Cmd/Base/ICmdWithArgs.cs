namespace ViewTo.Cmd
{
	internal interface ICmdWithArgs<out TArgs> : ICmd where TArgs : ICommandArgs
	{
		public TArgs args { get; }
	}
}