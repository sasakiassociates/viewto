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
  public void Study_CreateRig()
  {
    var study = ViewObjectFaker.Study();
    IRig rig = new Rig();

    Assert.IsTrue(study.CanRun());
    var reports = study.LoadStudyToRig(ref rig);

    reports.ForEach(Console.WriteLine);
  }


}
