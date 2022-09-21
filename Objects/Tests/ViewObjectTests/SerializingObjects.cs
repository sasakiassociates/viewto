using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Speckle.Core.Api;
using Speckle.Core.Kits;
using ViewObjects;
using ViewObjects.Cloud;
using ViewObjects.Content;
using ViewObjects.Converter.Script;
using ViewObjects.Speckle;
using ViewObjects.Study;
using ViewObjects.Viewer;

namespace ViewTests.Objects
{
	[TestFixture]
	public class SerializingObjects
	{
		[Test]
		public void Convert_Study()
		{
			var objs = new List<IViewObj>()
			{
				new ViewContent_v2(ContentType.Proposed),
				new ViewCloudReference(new List<string>() { "256ff84cf7" }),
				new ViewerSystem_v2()
			};

			var obj = new ViewStudy_v2(objs, "Test View Study");

			var converter = new ViewObjectsConverterScript();
			var res = converter.ConvertToSpeckle(obj);

			Assert.IsNotNull(res);
			Assert.IsTrue(res is ViewStudyBase_v2);

			ViewStudyBase_v2 studyBase = res as ViewStudyBase_v2;
			Assert.IsTrue(obj.ViewId.Equals(studyBase.ViewId));
			Assert.IsTrue(obj.ViewName.Equals(studyBase.ViewName));
			Assert.IsTrue(obj.Objects.Count.Equals(studyBase.Objects.Count));
		}

		[Test]
		public void Serialize_Content()
		{
			var obj_v1 = new TargetContentBaseV1();

			var json = Operations.Serialize(obj_v1);
			var res = Operations.Deserialize(json);

			Assert.IsNotNull(res);
			Assert.IsTrue(res is TargetContentBaseV1);

			var obj_v2 = new ContentBase_v2();

			json = Operations.Serialize(obj_v2);
			res = Operations.Deserialize(json);

			Assert.IsNotNull(res);
			Assert.IsTrue(res is ContentBase_v2);
		}

		[Test]
		public void Kit_Types()
		{
			var kit = KitManager.GetKit(ViewToKit.AssemblyFullName);
			Assert.IsNotNull(kit);
			Assert.IsTrue(kit.Name.Equals(nameof(ViewToKit)));

			var types = kit.Types.ToList();
			Assert.IsNotEmpty(types);
			foreach (var t in types)
				Console.WriteLine(t.Name);
		}

		[Test, Ignore("Data Container types are only available in View Objects")]
		public void Serialize_Containers()
		{
			var obj_v1 = new ResultCloudBase_v2()
			{
				Data = new List<PixelDataContainer>()
				{
					new PixelDataContainer()
					{
						ContentId = "123",
						Layout = "Horizontal",
						Stage = ResultStage.Potential,
						Values = Array.Empty<int>()
					}
				}
			};

			var json = Operations.Serialize(obj_v1);
			var res = Operations.Deserialize(json);

			Assert.IsNotNull(res);
			Assert.IsTrue(res is ResultCloudBase_v2);
			var obj_v2 = res as ResultCloudBase_v2;

			Assert.IsNotNull(obj_v2);
			Assert.IsTrue(obj_v2.Data.Count == obj_v1.Data.Count);
			Assert.IsTrue(obj_v2.Data.FirstOrDefault().Stage.Equals(obj_v1.Data.FirstOrDefault().Stage));
			Assert.IsTrue(obj_v2.Data.FirstOrDefault().Layout.Equals(obj_v1.Data.FirstOrDefault().Layout));
			Assert.IsTrue(obj_v2.Data.FirstOrDefault().ContentId.Equals(obj_v1.Data.FirstOrDefault().ContentId));
			Assert.IsTrue(obj_v2.Data.FirstOrDefault().Values.Length.Equals(obj_v1.Data.FirstOrDefault().Values.Length));
		}

		[Test]
		public void Serialize_Study()
		{
			var obj_v1 = new ViewStudyBaseV1();

			var json = Operations.Serialize(obj_v1);
			var res = Operations.Deserialize(json);

			Assert.IsNotNull(res);
			Assert.IsTrue(res is ViewStudyBaseV1);

			var obj_v2 = new ViewStudyBase_v2();

			json = Operations.Serialize(obj_v2);
			res = Operations.Deserialize(json);

			Assert.IsNotNull(res);
			Assert.IsTrue(res is ViewStudyBase_v2);

			// var objs = new List<ViewObjectBase>()
			// {
			// 	new ContentBase_v2(ContentType.Proposed, new List<string>() { "256ff84cf7" }),
			// 	new ViewCloudBase_v2(new List<string>() { "256ff84cf7" }),
			// 	new ViewerSystemBase_v2()
			// };

			// ViewStudyBase_v2 studyBase = res as ViewStudyBase_v2;
			// Assert.IsTrue(obj.ViewId.Equals(studyBase.ViewId));
			// Assert.IsTrue(obj.ViewName.Equals(studyBase.ViewName));
			// Assert.IsTrue(obj.Objects.Count.Equals(studyBase.Objects.Count));
		}

	}
}