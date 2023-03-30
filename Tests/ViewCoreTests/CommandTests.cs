using NUnit.Framework;
using System;
using ViewObjects;
using ViewObjects.Clouds;
using ViewObjects.Systems;
using ViewTo;
using ViewTo.Tests;

[TestFixture]
[Category(Categories.INT)]
public partial class CommandTests
{

  [Test]
  public void Explorer_GetNormalizedValues()
  {
    var study = ViewObjectFaker.Study();
    Assert.IsTrue(study.CanExplore());

    var obj = new Explorer();
    obj.Load(study.Get<IResultCloud>());

    Assert.IsTrue(obj.GetSols(ExplorerValueType.ExistingOverPotential, out var values));
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
