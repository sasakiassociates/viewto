namespace ViewTo.Cmd
{

	internal interface ILoadCommand<TObject>
	{
		public TObject Obj { get; }
	}
}