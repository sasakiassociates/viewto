using NUnit.Framework;
using Speckle.Core.Api;
using Speckle.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ViewObjects.Clouds;
using VS = ViewObjects.Speckle;
using VO = ViewObjects;

namespace ViewTo.Tests.Objects
{

  [TestFixture]
  [Category(Categories.UNITS)]
  public class SerializingObjects
  {
    [Test]
    public void Serialize_ViewObjectType()
    {
      foreach(var vtype in GetSubclassTypes(typeof(VS.ViewObjectBase)))
      {
        var vo = Activator.CreateInstance(vtype) as VS.ViewObjectBase;

        Assert.IsNotNull(vo);
        Assert.IsTrue(vo.speckle_type.Contains(vo.GetType().ToString()));

        Console.WriteLine(vo.speckle_type);
      }
    }

    [Test]
    public void Serialize_Content()
    {
      var obj_v2 = Serialize_Process(new VS.ContentReference());
    }

    [Test]
    public void Serialize_Study()
    {
      var obj_v2 = Serialize_Process(new VS.ViewStudy());
    }

    [Test]
    public void Serialize_ResultCloud()
    {
      var obj = ViewObjectFaker.ResultCloud<VS.ResultCloud, VS.ResultCloudData>(100, 2);
      var result = Serialize_Process(obj);
      Assert.IsTrue(obj != default(object) && result != default(object));
      Assert.IsTrue(obj.MetaData.Count == result.MetaData.Count);
      Assert.IsTrue(obj.Data.Count == result.Data.Count);

      for(var i = 0; i < obj.Data.Count; i++)
      {
        Check(obj.Data[i], result.Data[i]);
      }
    }

    [Test]
    public void Serialize_ResultCloudData()
    {
      var obj = ViewObjectFaker.Result<VS.ResultCloudData>(100, VO.ViewContentType.Existing);
      Check(obj, Serialize_Process(obj));
    }

    private static TObj Serialize_Process<TObj>(TObj obj) where TObj : Base
    {
      var json = Operations.Serialize(obj);
      var res = Operations.Deserialize(json);

      Assert.IsNotNull(res);
      Assert.IsTrue(res is TObj);

      Console.WriteLine(res.speckle_type);

      return res as TObj;
    }

    private static void Check(IResultCloudData dataA, IResultCloudData dataB)
    {
      Assert.IsTrue(dataA != default(object) && dataB != default(object));
      Assert.IsTrue(dataA.count.Equals(dataB.count));
      Assert.IsTrue(dataA.values.Count == dataB.values.Count);
      Assert.IsTrue(dataA.info.stage.Equals(dataB.info.stage)
                    && dataA.info.target.ViewId.Equals(dataB.info.target.ViewId)
                    && dataA.info.content.ViewId.Equals(dataB.info.content.ViewId)
      );
    }

    private static List<Type> GetSubclassTypes(Type parentType)
    {
      return Assembly.GetAssembly(parentType).GetTypes()
        .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(parentType)).ToList();
    }
  }

}
