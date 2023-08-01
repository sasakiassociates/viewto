using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using ViewObjects;
using ViewObjects.Clouds;
using ViewObjects.Contents;
using ViewObjects.References;
using ViewObjects.Studies;

namespace ViewTo.RhinoGh.Results
{

  public class ExtractContentOptions : ViewToComponentBase
  {
    // private GH_ValueList _activeList;



    public ExtractContentOptions() : base(
      "Extract Options",
      "EO",
      "Extract all Content Options from a result cloud",
      ConnectorInfo.Nodes.RESULTS)
    { }

    public override Guid ComponentGuid => new Guid("601505A6-A108-4DB4-AEFA-E15722C008A6");

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("View Study", "V", "View Study To get data from", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Targets", "T", "List of View Targets", GH_ParamAccess.list);
      // pManager.AddGenericParameter("Proposals", "P", "List of propsal content", GH_ParamAccess.list);
    }

    // public static List<GH_ValueListItem> CreateValueListItems(List<string> values, List<string> expressions = null)
    // {
    //   var items = new List<GH_ValueListItem>();
    //   if(values != null && values.Any())
    //   {
    //     for(var i = 0; i < values.Count; i++)
    //     {
    //       items.Add(new GH_ValueListItem(values[i], expressions?[i] != null ? $"\"{expressions[i]}\"" : $"\"{values[i]}\""));
    //     }
    //   }
    //
    //   return items;
    // }
    //
    // public static GH_ValueList PopulateValueList(List<GH_ValueListItem> values, string name, string nickName, string description)
    // {
    //   GH_ValueList valueList = null;
    //   //make dropdown box
    //   if(values != null && values.Any())
    //   {
    //     valueList = new GH_ValueList();
    //     valueList.CreateAttributes();
    //     valueList.Name = name;
    //     valueList.NickName = nickName;
    //     valueList.Description = description;
    //     valueList.ListMode = GH_ValueListMode.DropDown;
    //
    //     valueList.ListItems.Clear();
    //
    //     foreach(var t in values)
    //     {
    //       valueList.ListItems.Add(t);
    //     }
    //   }
    //
    //   return valueList;
    // }
    //
    // protected override void AfterSolveInstance()
    // {
    //   base.AfterSolveInstance();
    //
    //   if(!_refresh || _storedValues == null || !_storedValues.Any())
    //   {
    //     return;
    //   }
    //
    //   var items = CreateValueListItems(_storedValues.Select(x => x.ViewName).ToList(), _storedValues.Select(x => x.ViewId).ToList());
    //   if(_activeList == null)
    //   {
    //     _activeList = PopulateValueList(
    //       items,
    //       "Targets",
    //       "T",
    //       "A list of view targets"
    //     );
    //     var point = new PointF(Attributes.Bounds.Location.X - _activeList.Attributes.Bounds.Width,
    //       Attributes.Bounds.Location.Y + (1 * 2 + 1) * Attributes.Bounds.Height / (Params.Input.Count * 2) - 12);
    //     _activeList.Attributes.Pivot = point;
    //     _activeList.SelectItem(0);
    //
    //     var doc = OnPingDocument();
    //     doc?.AddObject(_activeList, true);
    //   }
    //   else
    //   {
    //     _activeList.ListItems.Clear();
    //     foreach(var i in items)
    //     {
    //       _activeList.ListItems.Add(i);
    //     }
    //
    //     _activeList.SelectItem(0);
    //   }
    //
    //   _refresh = false;
    // }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GH_ObjectWrapper wrapper = null;
      DA.GetData(0, ref wrapper);

      if(wrapper?.Value is ViewStudy obj && obj.Has<ResultCloud>())
      {
        var options = obj.Get<ResultCloud>().GetAllOpts()
          .Where(x => x != null)
          .Select(x => new ContentOption(x.target, x.content, x.stage))
          .ToList();

        DA.SetDataList(0,options );

      }
    }
  }

}
