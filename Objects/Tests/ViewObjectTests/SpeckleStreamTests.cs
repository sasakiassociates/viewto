using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Speckle.Core.Kits;
using Speckle.Core.Transports;
using ViewObjects.Converter;
using Cat = ViewTests.ViewTestCategories;
using VS = ViewObjects.Speckle;
using VO = ViewObjects;

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
		public async Task SendTestResultCloud()
		{
			_client = new Client(AccountManager.GetDefaultAccount());

			// test stream for view to 
			_stream.id = "1da7b18b31";
			_stream.branch = "conversions";

			var cloud = VO.ViewObjectFaker.ResultCloud<VS.ResultCloud, VS.ResultCloudData>(1000, 2);

			_transport = new ServerTransport(_client.Account, _stream.id);

			var id = await Operations.Send(cloud, new List<ITransport>
				                               { _transport });

			Assert.IsNotNull(id);

			var commit = await _client.CommitCreate(new CommitCreateInput
			{
				streamId = _stream.id,
				branchName = _stream.branch,
				objectId = id,
				message = "Test commit from Script",
				sourceApplication = VersionedHostApplications.Script
			});

			Assert.IsNotNull(commit);
		}

		[Test]
		[Ignore("Not working with current object type")]
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
			if (obj is VS.ResultCloud viewObj)
			{
				Assert.IsNotEmpty(viewObj.Points);
				Assert.IsNotEmpty(viewObj.Data);
			}
			else
			{
				Assert.Fail($"{obj.GetType()} received was not converted to {typeof(VS.ResultCloud)}");
			}
		}
	}
}