using System;
using System.Collections.Generic;
using NUnit.Framework;
using ViewTests;

namespace ViewCoreTests
{
	[TestFixture]
	[Category(ViewCat.CORE)]
	public class ValueTests
	{

		[Test]
		public void SimpleListCheck()
		{
			var list = new List<double>(10);

			for (var i = 0; i < 10; i++)
			{
				Assert.IsNotNull(list[i]);
				Console.WriteLine($"Item {i} = {list[i]}");
			}

			Assert.Pass();
		}
	}
}