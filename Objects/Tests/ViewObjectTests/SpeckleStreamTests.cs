using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Speckle.Core.Transports;
using ViewObjects.Converter;
using ViewObjects.Speckle;
using Cat = ViewTests.ViewTestCategories;

namespace ViewTests.Objects
{
	[TestFixture]
	[Category(Cat.SPECKLE)]
	public class SpeckleStreamTests
	{

		[TearDown]
		public void BreakDown()
		{
			_transport?.Dispose();
			_client?.Dispose();
		}

		Client _client;
		ServerTransport _transport;
		(string id, string branch, string commit) _stream;

		[Test]
		public async Task ReceiveResultCloudFromCommit()
		{
			_client = new Client(AccountManager.GetDefaultAccount());

			// example from uph stream
			_stream.id = "a823053e07";
			_stream.branch = "viewstudies/road-way";
			_stream.commit = "41000075d1";

			var commit = await _client.CommitGet(_stream.id, _stream.commit);

			Assert.NotNull(commit);
			_transport = new ServerTransport(_client.Account, _stream.id);

			var @base = await Operations.Receive(commit.referencedObject, _transport);

			Assert.IsNotNull(@base);

			Console.WriteLine($"{@base.speckle_type} Total Child Cout={@base.totalChildrenCount}");

			var converter = new ViewObjectsConverter();
			var obj = converter.ConvertToNative(@base);

			Assert.IsNotNull(obj);
			if (obj is ResultCloudBase viewObj)
			{
				Assert.IsNotEmpty(viewObj.Points);
				Assert.IsNotEmpty(viewObj.Data);
			}
			else
			{
				Assert.Fail($"{obj.GetType()} received was not converted to {typeof(ResultCloudBase)}");
			}
		}
	}
}