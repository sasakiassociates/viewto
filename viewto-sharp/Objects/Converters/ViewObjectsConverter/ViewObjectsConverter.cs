using Speckle.Core.Kits;
using Speckle.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using ViewObjects.Clouds;
using Sasaki.Common;
using ViewObjects.Contents;
using ViewObjects.References;
using ViewObjects.Studies;
using ViewObjects.Systems;
using ViewObjects.Systems.Layouts;
using VS = ViewObjects.Speckle;
using VO = ViewObjects;

namespace ViewObjects.Converter
{

  public partial class ViewObjectsConverter : ISpeckleConverter
  {

    public virtual string Name => nameof(ViewObjectsConverter);

    public virtual string Description => "Converter for basic view objects";

    public virtual string Author => "David Morgan";

    public virtual string WebsiteOrEmail => "https://sasaki.com";

    public virtual ProgressReport Report { get; set; } = new ProgressReport();

    public virtual ReceiveMode ReceiveMode { get; set; } = ReceiveMode.Ignore;

    public virtual IEnumerable<string> GetServicedApplications()
    {
      return new[]
      {
        HostApplications.Grasshopper.GetVersion(HostAppVersion.v6),
        HostApplications.Grasshopper.GetVersion(HostAppVersion.v7),
        HostApplications.Rhino.GetVersion(HostAppVersion.v7),
        HostApplications.Rhino.GetVersion(HostAppVersion.v7)
      };
    }

    public virtual void SetContextDocument(object doc)
    { }

    public virtual void SetContextObjects(List<ApplicationObject> objects)
    { }

    public virtual void SetPreviousContextObjects(List<ApplicationObject> objects)
    { }

    public virtual void SetConverterSettings(object settings)
    { }

    public List<Base> ConvertToSpeckle(List<object> objects)
    {
      var result = new List<Base>();
      foreach(var o in objects)
      {
        var obj = ConvertToSpeckleViewObject(o);

        if(o == null)
        {
          // throw 
          continue;
        }

        result.Add(obj);
      }

      return result;
    }

    public Base ConvertToSpeckle(object @object)
    {
      return ConvertToSpeckleViewObject(@object);
    }

    public List<object> ConvertToNative(List<Base> objects)
    {
      return objects.Select(ConvertToNative).ToList();
    }

    public object ConvertToNative(Base @object)
    {
      return ConvertToNativeViewObject(@object);
    }

    public bool CanConvertToSpeckle(object @object)
    {
      switch(@object)
      {
        case IVersionReference _:
          return true;
        case IStudy<IViewObject> _:
          return true;
        case IStudy<IViewObjectReference> _:
          return true;
        case IResultCloud _:
          return true;
        case ICloud _:
          return true;
        case IContent _:
          return true;
        case ILayout _:
          return true;
        case IViewer _:
          return true;
        default:
          return false;
      }
    }

    public bool CanConvertToNative(Base @base)
    {
      switch(@base)
      {
        // V2 objects
        case VS.ViewStudy _:
          return true;
        case VS.ViewCloudReference _:
          return true;
        case VS.ContentReference _:
          return true;
        case VS.Layout _:
          return true;
        case VS.Viewer _:
          return true;
        case VS.ViewerLinked _:
          return true;
        case VS.ResultCloud _:
          return true;
        case VS.ResultLayer _:
          return true;
        default:
          return false;
      }
    }

    public VS.ViewObjectBase ConvertToSpeckleViewObject(object @object)
    {
      switch(@object)
      {
        case IStudy<IViewObject> o:
          return StudyToSpeckle(o);
        case ILayout o:
          return LayoutToSpeckle(o);
        case IViewerLinked o:
          return ViewerToSpeckle(o);
        case IViewer o:
          return ViewerToSpeckle(o);

        case IResultCloud o:
          return ResultCloudToSpeckle(o);
        case IResultLayer o:
          return ResultCloudDataToSpeckle(o);

        case ContentReference o:
          return ViewContentToSpeckle(o);

        case ViewCloudReference o:
          return ViewCloudToSpeckle(o);
        case ViewObjectReference o:
          return ReferenceToSpeckle(o);
        default:
          throw new ArgumentOutOfRangeException(nameof(@object), @object, null);
      }
    }

    public IViewObject ConvertToNativeViewObject(Base @object)
    {
      switch(@object)
      {
        case VS.ViewStudy o:
          return StudyToNative(o);
        case VS.ViewCloudReference o:
          return ViewCloudReferenceToNative(o);
        case VS.ContentReference o:
          return ContentReferenceToNative(o);
        case VS.Layout o:
          return LayoutToNative(o);
        case VS.ViewerLinked o:
          return ViewerToNative(o);
        case VS.Viewer o:
          return ViewerToNative(o);
        case VS.ResultCloud o:
          return ResultCloudToNative(o);
        case VS.ViewObjectReference o:
          return ReferenceToNative(o);
        case Base o:
          return HandleDefault(o);
        default:
          throw new ArgumentOutOfRangeException(nameof(@object), @object, null);
      }
    }
  }

}
