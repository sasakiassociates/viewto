using System.IO;

namespace ViewTo.Commands
{
	internal class WriteCsvCommand : ICommand
	{

		readonly string blob;
		readonly string path;

		public WriteCsvCommand(string blob, string path)
		{
			this.blob = blob;
			this.path = path;
		}

		public void Run()
		{
			using var writer = new StreamWriter(path);

			writer.Write(blob);
			writer.Flush();
			writer.Close();
		}
	}
}