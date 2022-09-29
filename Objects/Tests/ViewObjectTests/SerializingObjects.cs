using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Speckle.Core.Api;
using Speckle.Core.Models;
using ViewObjects.Speckle;
using Cat = ViewTests.ViewTestCategories;

namespace ViewTests.Objects
{
	[TestFixture, Category(Cat.UNITS)]
	public class SerializingObjects
	{
		[Test]
		public void Serialize_ViewObjectType()
		{
			foreach (var vtype in GetSubclassTypes(typeof(ViewObjectBase_v2)))
			{
				var vo = Activator.CreateInstance(vtype) as ViewObjectBase_v2;

				Assert.IsNotNull(vo);
				Assert.IsTrue(vo.speckle_type.Contains(vo.GetType().ToString()));

				Console.WriteLine(vo.speckle_type);
			}
		}

		[Test]
		public void Serialize_Content()
		{
			var obj_v1 = Serialize_Process(new TargetContentBaseV1());
			var obj_v2 = Serialize_Process(new ContentBase_v2());
		}

		[Test]
		public void Serialize_Study()
		{
			var obj_v1 = Serialize_Process(new ViewStudyBaseV1());
			var obj_v2 = Serialize_Process(new ViewStudyBase_v2());
		}

		[Test]
		public void Serialize_ResultCloud()
		{
			var obj_v1 = Serialize_Process(new ResultCloudBaseV1());
			var obj_v2 = Serialize_Process(new ResultCloudBase_v2());
		}

		static TObj Serialize_Process<TObj>(TObj obj) where TObj : Base
		{
			var json = Operations.Serialize(obj);
			var res = Operations.Deserialize(json);

			Assert.IsNotNull(res);
			Assert.IsTrue(res is TObj);

			Console.WriteLine(res.speckle_type);

			return res as TObj;
		}

		static List<Type> GetSubclassTypes(Type parentType) =>
			Assembly.GetAssembly(parentType).GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(parentType)).ToList();

	}
}