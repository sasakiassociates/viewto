using Grasshopper.Kernel.Types;
using ViewObjects;
namespace ViewTo.RhinoGh.Goo
{

  public class GH_ViewObj : GH_Goo<IViewObject>
  {
    public GH_ViewObj()
    {
      Value = default(IViewObject);
    }

    public GH_ViewObj(IViewObject data)
    {
      Value = data;
    }

    public GH_ViewObj(GH_Goo<IViewObject> other)
    {
      Value = other.Value;
    }

    public override bool IsValid => Value != default(object);

    public override string TypeName => IsValid && !string.IsNullOrEmpty(Value.TypeName()) ? Value.TypeName() : "ViewObj";

    public override string TypeDescription => "A ViewTo Object";

    public override string ToString()
    {
      return IsValid && !string.IsNullOrEmpty(Value.TypeName()) ? Value.TypeName() : "ViewObj";
    }

    // Not sure what this does... 
    public override object ScriptVariable()
    {
      return Value;
    }

    // work out copy logic 
    public override IGH_Goo Duplicate()
    {
      return new GH_ViewObj();
    }

    // REVIEW how this works with GOO
    public override bool CastTo<Q>(ref Q target)
    {
      if (!(target is GH_ViewObj))
      {
        return false;
      }

      target = (Q)(object)new GH_ViewObj
      {
        Value = Value
      };
      return true;
    }

    public override bool CastFrom(object source)
    {
      var obj = source switch
      {
        IViewObject viewObj => viewObj,
        GH_ViewObj viewObjGH => viewObjGH.Value,
        GH_Goo<IViewObject> goo => goo.Value,
        _ => default(IViewObject)
      };

      if (obj != null)
      {
        Value = obj;
      }

      return true;
    }

    // private ViewObj TryCreateEmpty(object source)
    // {
    //   switch (source)
    //   {
    //     case ViewStudy _:
    //       return new ViewStudy();
    //     case ViewCloud _:
    //       return new ViewCloud();
    //     case ContentBundle _:
    //       return new ContentBundle();
    //     case TargetContent _:
    //       return new TargetContent();
    //     case BlockerContent _:
    //       return new BlockerContent();
    //     case DesignContent _:
    //       return new DesignContent();
    //     case ViewerBundle _:
    //       return new ViewerBundle();
    //     default:
    //       return null;
    //   }
    // }
  }
}
