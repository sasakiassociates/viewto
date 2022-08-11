namespace ViewObjects.Explorer
{
	public struct ResultPoint : IExploreContent
	{

		public int Index;
		public double Value;
		public System.Drawing.Color Color;
		public double X, Y, Z;

		public string target { get; set; }

		public ResultType type { get; set; }
	}

}