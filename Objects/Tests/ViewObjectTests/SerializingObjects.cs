using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Speckle.Core.Api;
using Speckle.Core.Models;
using ViewObjects;
using ViewObjects.Speckle;
using Cat = ViewTests.ViewTestCategories;

namespace ViewTests.Objects
{
	[TestFixture]
	[Category(Cat.UNITS)]
	public class SerializingObjects
	{
		[Test]
		public void Serialize_ViewObjectType()
		{
			foreach (var vtype in GetSubclassTypes(typeof(ViewObjectBase)))
			{
				var vo = Activator.CreateInstance(vtype) as ViewObjectBase;

				Assert.IsNotNull(vo);
				Assert.IsTrue(vo.speckle_type.Contains(vo.GetType().ToString()));

				Console.WriteLine(vo.speckle_type);
			}
		}

		[Test]
		public void Serialize_Content()
		{
			var obj_v2 = Serialize_Process(new ContentBase());
		}

		[Test]
		public void Serialize_Study()
		{
			var obj_v2 = Serialize_Process(new ViewStudyBase());
		}

		[Test]
		public void Serialize_ResultCloud()
		{
			var obj_v2 = Serialize_Process(new ResultCloudBase
			{
				Data = new List<IResultCloudData>
				{
					new ResultCloudDataBase
					{
						Layout = "ViewerLayout",
						Values = new List<int>
							{ 1, 2, 3, 4 },
						Option = new ContentOption
							{ Id = Guid.NewGuid().ToString(), Stage = ResultStage.Existing, Name = "Test" }
					}
				}
			});
		}

		[Test]
		public void Serialize_ResultCloudData()
		{
			var obj = new ResultCloudDataBase
			{
				Layout = "ViewerLayout",
				Values = new List<int>
					{ 1, 2, 3, 4 },
				Option = new ContentOption
					{ Id = Guid.NewGuid().ToString(), Stage = ResultStage.Existing, Name = "Test" }
			};
			var obj_v2 = Serialize_Process(obj);

			Assert.IsTrue(obj.Layout.Equals(obj_v2.Layout));
			Assert.IsTrue(obj.Values.Count == obj_v2.Values.Count);
			Assert.IsTrue(obj.Option.Stage.Equals(obj_v2.Option.Stage)
			              && obj.Option.Id.Equals(obj_v2.Option.Id)
			              && obj.Option.Name.Equals(obj_v2.Option.Name)
			);
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

		static List<Type> GetSubclassTypes(Type parentType) => Assembly.GetAssembly(parentType).GetTypes()
			.Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(parentType)).ToList();
	}
}