using System.Collections.Generic;
using NUnit.Framework;
using ViewObjects;
using ViewObjects.Cloud;
using ViewObjects.Content;
using ViewObjects.Converter;
using ViewObjects.Study;
using ViewObjects.Viewer;
using Cat = ViewTests.ViewTestCategories;
using ViewStudy = ViewObjects.Speckle.ViewStudy;

namespace ViewTests.Objects
{
	[TestFixture]
	[Category(Cat.UNITS)]
	public class ConversionTests
	{

		[OneTimeSetUp]
		public void Setup()
		{
			_converter = new ViewObjectsConverter();
		}

		[OneTimeTearDown]
		public void BreakDown()
		{ }

		ViewObjectsConverter _converter;

		[Test]
		public void Convert_Study()
		{
			var objs = new List<IViewObj>
			{
				new ViewContent_v2(ContentType.Proposed),
				new ViewCloudReference(new List<string>
					                       { "256ff84cf7" }),
				new ViewerSystem_v2()
			};

			var obj = new ViewStudy_v2(objs, "Test View Study");

			var converter = new ViewObjectsConverter();
			var res = converter.ConvertToSpeckle(obj);

			Assert.IsNotNull(res);
			Assert.IsTrue(res is ViewStudy);

			var studyBase = res as ViewStudy;
			Assert.IsTrue(obj.ViewId.Equals(studyBase.ViewId));
			Assert.IsTrue(obj.ViewName.Equals(studyBase.ViewName));
			Assert.IsTrue(obj.Objects.Count.Equals(studyBase.Objects.Count));
		}
	}
}