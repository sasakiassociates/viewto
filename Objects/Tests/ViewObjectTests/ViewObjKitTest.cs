using System;
using System.Linq;
using NUnit.Framework;
using Speckle.Core.Kits;
using ViewObjects.Speckle;
using Cat = ViewTests.ViewTestCategories;

namespace ViewTests.Objects
{
	[TestFixture]
	[Category(ViewTestCategories.UNITS)]
	public class ViewObjKitTest
	{

		[Test]
		public void Kit_Types()
		{
			var kit = KitManager.GetKit(ViewObjectKit.AssemblyFullName);
			Assert.IsNotNull(kit);
			Assert.IsTrue(kit.Name.Equals(nameof(ViewObjectKit)));

			var types = kit.Types.ToList();

			Assert.IsNotEmpty(types);
			foreach (var t in types)
				Console.WriteLine(t.Name);
		}
	}
}