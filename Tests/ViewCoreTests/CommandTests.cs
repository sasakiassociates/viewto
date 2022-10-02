using NUnit.Framework;
using ViewObjects;
using ViewObjects.Explorer;
using ViewTo;
using Cat = ViewTests.ViewTestCategories;

[TestFixture, Category(Cat.INT)]
public class CommandTests
{

	[Test]
	public void Explorer_GetNormalizedValues()
	{
		var study = ViewObjectFaker.Study();
		var obj = new Explorer();
		obj.Load(study);

		Assert.IsTrue(obj.TryGetValues(ExplorerValueType.ExistingOverPotential, out var values));
	}
}