using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Speckle.Core.Transports;
using ViewObjects;
using ViewObjects.Converter.Script;
using ViewObjects.Speckle;
using ViewTo;

namespace ViewTests.Objects
{
	[TestFixture]
	[Category(ViewCat.SPECKLE)]
	public class ConverterTests
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

		[OneTimeTearDown]
		public void BreakDown()
		{
			myServerTransport?.Dispose();
			myClient?.Dispose();
		}

		[Test]
		public async Task Convert_ResultCloud()
		{
			var commit = await myClient.CommitGet(_stream.id, _stream.commit);
			
			Assert.NotNull(commit);
			myServerTransport = new ServerTransport(myClient.Account, _stream.id);

			var @base = await Operations.Receive(commit.referencedObject, myServerTransport);

			Assert.IsNotNull(@base);
			Console.WriteLine(@base.totalChildrenCount);

			var converter = new ViewObjectConverter();
			var obj = converter.ConvertToNative(@base);

			Assert.IsNotNull(obj);
		}

		[Test]
		public void Convert_ViewStudy()
		{
			var study = Mil.Fabricate.Object.ViewStudy("Test_For_Conversion", true, false);

			var converter = new ViewObjectConverter();

			Assert.IsTrue(converter.CanConvertToSpeckle(study));

			var res = (ViewStudyBase)converter.ConvertToSpeckle(study);

			Assert.IsNotNull(study);

			Assert.IsTrue(res.objs.Count == study.objs.Count);
			Assert.IsTrue(res.viewName.Equals(study.viewName));

			foreach (var obj in res.objs)
			{
				switch (obj)
				{
					case ResultCloudBase o:
						var rc = (IResultCloud)study.objs.First(x => x is IResultCloud);
						Assert.IsTrue(rc != default && rc.viewID.Equals(o.viewID));
						Assert.IsTrue(rc.points.Length == o.points.Count);
						Assert.IsTrue(rc.data.Count == o.data.Count);
						break;
					case ViewCloudBase o:
						var vc = (IViewCloud)study.objs.First(x => x is IViewCloud);
						Assert.IsTrue(vc != default && vc.viewID.Equals(o.viewID));
						Assert.IsTrue(vc.points.Length == o.points.Count);
						break;
					case ContentBundleBase o:
						var cb = (IViewContentBundle)study.objs.First(x => x is IViewContentBundle);
						Assert.IsTrue(cb != default && cb.contents.Count == o.targets.Count + o.blockers.Count + o.designs.Count);
						break;
					case ViewerBundleBase o:
						var vb = (IViewerBundle)study.objs.First(x => x is IViewerBundle);
						Assert.IsTrue(vb != default && vb.layouts.Count == o.layouts.Count);
						break;
				}
			}
		}

	}
}