using NUnit.Framework;
using Speckle.Core.Kits;
using System;
using System.Linq;
using ViewObjects.Speckle;

namespace ViewTo.Tests.Objects
{

  [TestFixture]
  [Category(Categories.UNITS)]
  public class ViewObjKitTest
  {

    [Test]
    public void Kit_Types()
    {

      foreach(var k in KitManager.Kits)
      {
        if(k == null) continue;

        Console.WriteLine(k.Name);
      }
      return;


      var kit = KitManager.GetKit(ViewObject.AssemblyFullName);
      Assert.IsNotNull(kit);
      Assert.IsTrue(kit.Name.Equals(nameof(ViewObject)));

      var types = kit.Types.ToList();

      Assert.IsNotEmpty(types);
      foreach(var t in types)
      {
        Console.WriteLine(t.Name);
      }
    }
  }

}
