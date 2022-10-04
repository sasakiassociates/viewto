using System;
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
		Assert.IsTrue(study.CanExplore());

		var obj = new Explorer();
		obj.Load(study);

		Assert.IsTrue(obj.TryGetValues(ExplorerValueType.ExistingOverPotential, out var values));
	}

	[Test]
	public void Study_CreateRig()
	{
		var study = ViewObjectFaker.Study();
		IRig rig = new Rig();

		Assert.IsTrue(study.CanRun());
		var reports = study.LoadStudyToRig(ref rig);

		reports.ForEach(Console.WriteLine);
	}
}