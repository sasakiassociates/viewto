using System.Collections.Generic;
using NUnit.Framework;
using ViewObjects;
using ViewObjects.Converter;
using VS = ViewObjects.Speckle;
using Cat = ViewTests.ViewTestCategories;

namespace ViewTests.Objects
{
	[TestFixture]
	[Category(Cat.UNITS)]
	public class ConversionTests
	{

		ViewObjectsConverter _converter;

		[OneTimeSetUp]
		public void Setup()
		{
			_converter = new ViewObjectsConverter();
		}

		[OneTimeTearDown]
		public void BreakDown()
		{ }

		[Test]
		public void Convert_Content()
		{
			var obj = new ViewObjects.ContentReference(new Content(ContentType.Target), new List<string>() { "123443q312" });
			var res = _converter.ConvertToSpeckle(obj) as VS.ContentReference;
			Assert.IsTrue(res.ViewId.Equals(obj.ViewId));
		}

		[Test]
		public void Convert_Study()
		{
			var objs = new List<IViewObject>
			{
				new ViewObjects.ContentReference(new Content(ContentType.Target), new List<string>() { "123443q312" }),
				new ViewCloudReference(new List<string> { "256ff84cf7" }, ObjUtils.InitGuid),
				new Viewer()
			};

			var obj = new ViewStudy(objs, "Test View Study");

			var converter = new ViewObjectsConverter();
			var res = converter.ConvertToSpeckle(obj);

			Assert.IsNotNull(res);
			Assert.IsTrue(res is ViewObjects.Speckle.ViewStudy);

			var studyBase = res as ViewObjects.Speckle.ViewStudy;
			Assert.IsTrue(obj.ViewId.Equals(studyBase.ViewId));
			Assert.IsTrue(obj.ViewName.Equals(studyBase.ViewName));
			Assert.IsTrue(obj.Objects.Count.Equals(studyBase.Objects.Count));
		}
	}
}