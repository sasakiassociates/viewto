﻿using NUnit.Framework;
using Speckle.Core.Api;
using Speckle.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
      foreach (var vtype in GetSubclassTypes(typeof(VS.ViewObjectBase)))
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
      var obj = VO.ViewObjectFaker.ResultCloud<VS.ResultCloud, VS.ResultCloudData>(100, 2);
      var result = Serialize_Process(obj);
      Assert.IsTrue(obj != default && result != default);
      Assert.IsTrue(obj.MetaData.Count == result.MetaData.Count);
      Assert.IsTrue(obj.Data.Count == result.Data.Count);

      for (var i = 0; i < obj.Data.Count; i++)
      {
        Check(obj.Data[i], result.Data[i]);
      }
    }

    [Test]
    public void Serialize_ResultCloudData()
    {
      var obj = VO.ViewObjectFaker.Result<VS.ResultCloudData>(100, VO.ResultStage.Existing);
      Check(obj, Serialize_Process(obj));
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

    static void Check(VO.IResultCloudData dataA, VO.IResultCloudData dataB)
    {
      Assert.IsTrue(dataA != default && dataB != default);
      Assert.IsTrue(dataA.Layout.Equals(dataB.Layout));
      Assert.IsTrue(dataA.Values.Count == dataB.Values.Count);
      Assert.IsTrue(dataA.Option.Stage.Equals(dataB.Option.Stage)
                    && dataA.Option.Id.Equals(dataB.Option.Id)
                    && dataA.Option.Name.Equals(dataB.Option.Name)
      );
    }

    static List<Type> GetSubclassTypes(Type parentType) => Assembly.GetAssembly(parentType).GetTypes()
      .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(parentType)).ToList();
  }
}