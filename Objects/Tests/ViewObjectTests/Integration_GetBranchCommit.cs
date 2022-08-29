using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Speckle.Core.Transports;
using ViewObjects.Converter.Script;

namespace ViewTests.Objects
{
	[TestFixture]
	[Category(ViewCat.SPECKLE)]
	public class Integration_GetBranchCommit
	{

		Client myClient;
		ServerTransport myServerTransport;

		(string id, string branch, string commit) _stream;

		[OneTimeSetUp]
		public void Setup()
		{
			_stream.id = "9b692137ca";
			_stream.branch = "viewstudy/all-targets";
			_stream.commit = "e4cc0c5ce9";

			myClient = new Client(AccountManager.GetDefaultAccount());
		}

		[Test]
		public async Task GetCommit()
		{
			var commit = await myClient.CommitGet(_stream.id, _stream.commit);
			Assert.NotNull(commit);
			myServerTransport = new ServerTransport(myClient.Account, _stream.id);

			var @base = await Operations.Receive(commit.referencedObject, myServerTransport);

			Assert.IsNotNull(@base);
			Console.WriteLine(@base.totalChildrenCount);

			var converter = new ViewObjectsConverterScript();
			var obj = converter.ConvertToNative(@base);

			Assert.IsNotNull(obj);
		}

	}
}