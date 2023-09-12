using Sasaki;
using System.Collections.Generic;
using System.Linq;
using Sasaki.Common;
using ViewObjects.Contents;
using ViewObjects.References;
using ViewObjects.Studies;
using ViewObjects.Systems;

namespace ViewObjects
{

  public static class Extensions
  {
    public static List<IResultCondition> FindObject(this IResultCloud<IResultLayer> obj)
    {
      return new();
    }

    public static TObj FindObject<TObj>(this IViewStudy obj) where TObj : IViewObject
    {
      TObj viewObj = default;
      if(!obj.objects.Any())
      {
        return viewObj;
      }

      foreach(var o in obj.objects)
      {
        if(o is TObj casted)
        {
          viewObj = casted;
          break;
        }
      }

      return viewObj;
    }

    public static List<TObj> FindObjects<TObj>(this IViewStudy obj) where TObj : IViewObject
    {
      List<TObj> viewObjs = new();

      if(obj.objects.Any())
      {
        foreach(var o in obj.objects)
        {
          if(o is TObj casted)
          {
            viewObjs.Add(casted);
          }
        }
      }

      return viewObjs;
    }

    /// <summary>
    ///   <para>
    ///     Checks if a view study has the correct data necessary to run a view study.
    ///     The correct data is at least one <see cref="ICloud" />, one <see cref="IViewer" />,
    ///     one <see cref="Context" /> marked as <see cref="ViewContextType.Potential" /> and one marked as
    ///     <see cref="ViewContextType.Existing" />
    ///   </para>
    /// </summary>
    /// <param name="study">object to check</param>
    /// <returns></returns>
    public static bool CanRun(this IViewStudy study)
    {
      return study.Has<ICloud>() && study.Has<IViewer>() && study.Has(ViewContextType.Potential) && study.Has(ViewContextType.Existing);
    }

    /// <summary>
    ///   <para>
    ///     Checks if a view study has the correct object types needed to visualize data.
    ///     The one object needed is a <see cref="IResultCloud" />
    ///   </para>
    /// </summary>
    /// <param name="study">object to check</param>
    /// <returns></returns>
    public static bool CanExplore(this IViewStudy study)
    {
      return study.Has<IResultCloud>();
    }

    /// <summary>
    ///   <para>Checks for a specific type of <see cref="IContext" /> object that fits the <see cref="ViewContextType" /></para>
    /// </summary>
    /// <param name="study">study to check</param>
    /// <param name="type">type of content to find</param>
    /// <param name="id">optional id to use</param>
    /// <returns></returns>
    public static bool Has(this IViewStudy study, ViewContextType type, string id = "")
    {
      if(study?.objects == null || !study.objects.Any())
      {
        return false;
      }

      foreach(var obj in study.GetAll<IContext>())
      {
        if(obj.contentType != type)
        {
          continue;
        }

        // found at least one but no name was passed in 
        if(!id.Valid())
        {
          return true;
        }

        if(obj is IHaveId objWithID && objWithID.appId.Equals(id))
        {
          return true;
        }
      }

      return false;
    }

    /// <summary>
    ///   <para>Checks for a specific type of object to search for</para>
    /// </summary>
    /// <param name="study">study to check</param>
    /// <param name="id">optional id to use</param>
    public static bool Has<TObjType>(this IViewStudy study, string id = "")
    {
      if(study?.objects == null || !study.objects.Any())
      {
        return false;
      }

      foreach(var obj in study.objects.OfType<TObjType>())
      {
        // found at least one but no name was passed in 
        if(!id.Valid())
        {
          return true;
        }

        if(obj is IHaveId objWithID && objWithID.appId.Equals(id))
        {
          return true;
        }
      }

      return false;
    }

    public static List<ContextInfo> GetAllTargetContentInfo(this IViewStudy obj)
    {
      return obj == default(object) ?
        new List<ContextInfo>() :
        obj.FindObjects<ContextReferences>()
          .Where(x => x != null && x.contentType == ViewContextType.Potential)
          .Select(x => new ContextInfo(x.appId, x.name))
          .ToList();
    }

    /// <summary>
    ///   <para>Gets a list of objects of a specific type</para>
    /// </summary>
    /// <param name="study">study to check</param>
    public static List<TObjType> GetAll<TObjType>(this IViewStudy study)
    {
      return study.objects.OfType<TObjType>().ToList();
    }

    /// <summary>
    ///   <para>Gets a list of objects of a specific type</para>
    /// </summary>
    /// <param name="study">study to check</param>
    public static List<TObjectType> GetAll<TObj, TObjectType>(this IStudy<TObj> study)
      where TObj : IViewObject => study.objects.OfType<TObjectType>().ToList();

    /// <summary>
    ///   <para>Gets a specific type of object to search for</para>
    /// </summary>
    /// <param name="study">study to check</param>
    /// <param name="id">optional id to use</param>
    public static TObjType Get<TObjType>(this IViewStudy study, string id = "")
    {
      foreach(var obj in study.objects.OfType<TObjType>())
      {
        // found at least one but no name was passed in 
        if(!id.Valid())
        {
          return obj;
        }

        if(obj is IHaveId objWithID && objWithID.appId.Equals(id))
        {
          return obj;
        }
      }

      return default(TObjType);
    }

    /// <summary>
    /// Shortcut for getting all available content types in a study
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="targetCount"></param>
    /// <param name="existingCount"></param>
    /// <param name="proposedCount"></param>
    public static void ContentCount(this IViewStudy obj, out int targetCount, out int existingCount, out int proposedCount)
    {
      targetCount = 0;
      existingCount = 0;
      proposedCount = 0;

      if(obj != default(object))
      {
        var contents = obj.GetAll<IContext>();

        targetCount = contents.Count(x => x.contentType == ViewContextType.Potential);
        existingCount = contents.Count(x => x.contentType == ViewContextType.Existing);
        proposedCount = contents.Count(x => x.contentType == ViewContextType.Proposed);
      }
    }

    /// <summary>
    /// Shortcut for getting all available content types 
    /// </summary>
    /// <param name="contents"></param>
    /// <param name="targetCount"></param>
    /// <param name="existingCount"></param>
    /// <param name="proposedCount"></param>
    public static void ContentCount(this List<IContext> contents, out int targetCount, out int existingCount, out int proposedCount)
    {
      targetCount = contents.Count(x => x.contentType == ViewContextType.Potential);
      existingCount = contents.Count(x => x.contentType == ViewContextType.Existing);
      proposedCount = contents.Count(x => x.contentType == ViewContextType.Proposed);
    }
  }

}
