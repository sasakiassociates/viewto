using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ViewObjects;
using ViewObjects.Contents;
using ViewObjects.References;
using ViewObjects.Studies;

namespace ViewTo.RhinoGh.Results
{

  public class ExtractTargetNames : ViewToComponentBase
  {
    GH_ValueList _activeList;

    (int Object, int Values ) _input;

    bool _refresh;
    List<ContentInfo> _storedValues;

    public ExtractTargetNames() : base(
      "Extract Targets",
      "ET",
      "Extract Targets from a result cloud into a value list",
      ConnectorInfo.Nodes.RESULTS)
    { }

    public override Guid ComponentGuid => new Guid("601505A6-A108-4DB4-AEFA-E15722C008A6");

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      var index = 0;
      pManager.AddGenericParameter("View Study", "V", "View Study To get data from", GH_ParamAccess.item);
      _input.Object = index++;
      pManager.AddTextParameter("Results", "R", "Results of data", GH_ParamAccess.list);
      _input.Values = index;

      pManager[_input.Values].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Targets", "T", "List of View Targets", GH_ParamAccess.list);
    }

    public static List<GH_ValueListItem> CreateValueListItems(List<string> values, List<string> expressions = null)
    {
      var items = new List<GH_ValueListItem>();
      if(values != null && values.Any())
      {
        for(var i = 0; i < values.Count; i++)
        {
          items.Add(new GH_ValueListItem(values[i], expressions?[i] != null ? $"\"{expressions[i]}\"" : $"\"{values[i]}\""));
        }
      }

      return items;
    }

    public static GH_ValueList PopulateValueList(List<GH_ValueListItem> values, string name, string nickName, string description)
    {
      GH_ValueList valueList = null;
      //make dropdown box
      if(values != null && values.Any())
      {
        valueList = new GH_ValueList();
        valueList.CreateAttributes();
        valueList.Name = name;
        valueList.NickName = nickName;
        valueList.Description = description;
        valueList.ListMode = GH_ValueListMode.DropDown;

        valueList.ListItems.Clear();

        foreach(var t in values)
        {
          valueList.ListItems.Add(t);
        }
      }

      return valueList;
    }

    protected override void AfterSolveInstance()
    {
      base.AfterSolveInstance();

      if(!_refresh || _storedValues == null || !_storedValues.Any())
      {
        return;
      }

      var items = CreateValueListItems(_storedValues.Select(x => x.ViewName).ToList(), _storedValues.Select(x => x.ViewId).ToList());
      if(_activeList == null)
      {
        _activeList = PopulateValueList(
          items,
          "Targets",
          "T",
          "A list of view targets"
        );
        var point = new PointF(Attributes.Bounds.Location.X - _activeList.Attributes.Bounds.Width,
          Attributes.Bounds.Location.Y + (1 * 2 + 1) * Attributes.Bounds.Height / (Params.Input.Count * 2) - 12);
        _activeList.Attributes.Pivot = point;
        _activeList.SelectItem(0);

        var doc = OnPingDocument();
        doc?.AddObject(_activeList, true);
        Params.Input[_input.Values].AddSource(_activeList);
      }
      else
      {
        _activeList.ListItems.Clear();
        foreach(var i in items)
        {
          _activeList.ListItems.Add(i);
        }

        _activeList.SelectItem(0);
      }

      _refresh = false;
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GH_ObjectWrapper wrapper = null;
      DA.GetData(_input.Object, ref wrapper);

      var inputTargets = new List<string>();
      DA.GetDataList(_input.Values, inputTargets);

      if(wrapper?.Value is ViewStudy obj && obj.Has<IContentInfo>())
      {
        _storedValues = obj.FindObjects<ContentReference>()
          .Where(x => x != null && x.ContentType == ContentType.Potential)
          .Select(x => new ContentInfo(x.ViewId, x.ViewName))
          .ToList();

        if(!inputTargets.Any())
        {
          _refresh = true;
        }
        else
        {
          if(_storedValues.Count != inputTargets.Count)
          {
            _refresh = true;
          }
        }
      }

      DA.SetDataList(0, _storedValues);
    }
  }

}
