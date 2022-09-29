namespace ViewTo.Primers
{

	public class ComparedCloud
	{
		public ComparedCloud(string cloudID, string linkedContent)
		{
			this.cloudID = cloudID;
			this.linkedContent = linkedContent;
		}

		public string cloudID { get; }

		public string linkedContent { get; }
	}
}