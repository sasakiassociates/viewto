using System.Collections.Generic;
using ViewObjects;
using ViewObjects.Clouds;
using ViewObjects.Common;
using ViewObjects.Contents;
using ViewObjects.Studies;
using ViewObjects.Systems;
using ViewObjects.Systems.Layouts;
using ViewTo.Cmd;

namespace ViewTo
{




  public static partial class ViewCoreExtensions
  {

    // public static List<string> LoadStudyToRig<TObj>(this IViewStudy<TObj> study, ref IRig rig)
    //   where TObj : IViewObject
    // {
    //
    //   IViewStudy sss = Activator.CreateInstance<IViewStudy>();
    //   
    //   // look for layouts as well since there could be a layout not attached to a 
    //   study.GatherLooseLayouts();
    //   var viewers = study.GetAll<IViewer>();
    //   var clouds = study.GetAll<IViewCloud>();
    //   var contents = study.GetAll<IContent>();
    //
    //   var reports = new List<string>();
    //
    //   var sequence = new List<ICmd>
    //   {
    //     new CanStudyRun(contents, clouds, viewers),
    //     new AssignViewColors(contents),
    //     new InitializeAndBuildRig(rig, contents, clouds, viewers)
    //   };
    //
    //   foreach(var s in sequence)
    //   {
    //     s.Execute();
    //
    //     if(s is ICmdWithArgs<CommandArgs> cmdWithArgs)
    //     {
    //       reports.Add(cmdWithArgs.args.Message);
    //     }
    //   }
    //
    //   return reports;
    // }

    public static void GatherLooseLayouts<TObj>(this ISasakiStudy<TObj> study)
      where TObj : IViewObject
    {
      // var layouts = study.GetAll<ILayout>();
      // if(layouts.Valid())
      // {
      //   // if layouts are loose we add them to a default viewer since they will run on a global viewer 
      //   study.Objects.Add(new Viewer(layouts));
      // }
    }

    public static List<string> LoadStudyToRig(this IViewStudy study, ref IRig rig)
    {
      // look for layouts as well since there could be a layout not attached to a 
      study.GatherLooseLayouts();
      var viewers = study.GetAll<IViewer>();
      var clouds = study.GetAll<IViewCloud>();
      var contents = study.GetAll<IContent>();

      var reports = new List<string>();

      var sequence = new List<ICmd>
      {
        new CanStudyRun(contents, clouds, viewers),
        new AssignViewColors(contents),
        new InitializeAndBuildRig(rig, contents, clouds, viewers)
      };

      foreach(var s in sequence)
      {
        s.Execute();

        if(s is ICmdWithArgs<CommandArgs> cmdWithArgs)
        {
          reports.Add(cmdWithArgs.args.Message);
        }
      }

      return reports;
    }

    public static void GatherLooseLayouts(this IViewStudy study)
    {
      var layouts = study.GetAll<ILayout>();
      if(layouts.Valid())
      {
        // if layouts are loose we add them to a default viewer since they will run on a global viewer 
        study.objects.Add(new Viewer(layouts));
      }
    }


    // public static void Deconstruct<TObj>(this IViewStudy<TObj> obj) where TObj : IViewObject
    // {
    //   return new DeconstructedStudy()
    //   {
    //     targets = obj.FindObjects<ContentReference>().Where(x => x.ContentType == ContentType.Potential),
    //     existing = obj.FindObjects<ContentReference>().Where(x => x.ContentType == ContentType.Existing),
    //     proposed= obj.FindObjects<ContentReference>().Where(x => x.ContentType == ContentType.Proposed),
    //   };
    //
    // }

  }

}
