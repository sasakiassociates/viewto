using System;
using System.Collections.Generic;
using System.Linq;
using ViewObjects;
using ViewObjects.Clouds;
using ViewObjects.Common;
using ViewObjects.Contents;

namespace ViewTo
{


  public static partial class ViewCoreExtensions
  {
    private static TObj Fabricate<TObj>() => Activator.CreateInstance<TObj>();

    /// <summary>
    /// Finds all <see cref="IContentOption"/> within a <see cref="IResultCloud"/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static List<ContentOption> GetAllOpts(this IResultCloud obj)
    {
      List<ContentOption> result = new();

      if(obj == default(object) || !obj.Data.Valid())
      {
        return result;
      }
      result = obj.Data.Where(x => x?.info != null).Select(x => x.info).ToList();

      return result;
    }

    /// <summary>
    /// Finds all <see cref="IContentOption"/> within a <see cref="IResultCloud"/> with a specific <see cref="ViewContentType"/>
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="type">the type to filter by</param>
    /// <returns></returns>
    public static List<ContentOption> GetAllOpts(this IResultCloud obj, ViewContentType type)
    {
      return obj.GetAllOpts().Where(x => x.stage == type).ToList();
    }

    /// <summary>
    /// Gets a single <see cref="ContentOption"/> that matches the ids of the <seealso cref="ContentOption.target"/> and <see cref="ContentOption.content"/> content 
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="targetId"></param>
    /// <param name="contentId"></param>
    /// <param name="stage"></param>
    /// <returns></returns>
    public static ContentOption GetOpt(this IResultCloud obj, string targetId, string contentId, ViewContentType stage)
    {
      ContentOption result = Fabricate<ContentOption>();

      if(obj == default(object) || !obj.Data.Valid() || !targetId.Valid() || !contentId.Valid())
      {
        return result;
      }

      if(!Guid.TryParse(targetId, out _) && !Guid.TryParse(contentId, out _))
      {
        return result;
      }

      foreach(var d in obj.Data)
      {
        if(d?.info == null ||
           d.info.stage != stage ||
           !d.info.target.ViewId.Equals(targetId) ||
           !d.info.content.ViewId.Equals(contentId))
        {
          continue;
        }
        result = d.info;
        break;
      }
      return result;

    }

    /// <summary>
    /// Grabs all of the targets as a list of <see cref="IContentInfo"/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static List<IContentInfo> GetAllTargets(this IResultCloud obj)
    {
      List<IContentInfo> result = new();

      if(obj == default(object) || !obj.Data.Valid())
      {
        return result;
      }

      foreach(var d in obj.Data)
      {
        if(d?.info == null || result.Any(x => x.ViewId.Equals(d.info.target.ViewId)))
        {
          continue;
        }

        result.Add(d.info.target);
        break;
      }
      return result;

    }

    /// <summary>
    /// Grabs a <see cref="IContentInfo"/> that is linked to a target type
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="targetId">the target id to search for</param>
    /// <returns></returns>
    public static IContentInfo GetTarget(this IResultCloud obj, string targetId)
    {
      IContentInfo result = Fabricate<ContentInfo>();

      if(obj == default(object) || !obj.Data.Valid() || !targetId.Valid())
      {
        return result;
      }

      if(Guid.TryParse(targetId, out _))
      {
        return result;
      }

      foreach(var d in obj.Data)
      {
        if(d?.info == null || !d.info.target.ViewId.Equals(targetId))
        {
          continue;
        }

        result = d.info.target;
        break;
      }

      return result;

    }

    /// <summary>
    /// Searches for a <see cref="ContentOption"/> that has a similar target, content, and stage type
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="targetId"></param>
    /// <param name="contentId"></param>
    /// <param name="stage"></param>
    /// <returns></returns>
    public static bool HasOpt(this IResultCloud obj, string targetId, string contentId, ViewContentType stage)
    {
      if(obj == default(object) || !obj.Data.Valid() || !Guid.TryParse(targetId, out _) || !Guid.TryParse(contentId, out _))
      {
        return false;
      }

      return obj.Data.Any(x => x.info.stage == stage && x.info.target.ViewId.Equals(targetId) && x.info.content.ViewId.Equals(contentId));
    }

    /// <summary>
    /// <para>Searches for a <see cref="ContentOption"/> that has a similar target and stage type. This will select the target by id
    /// and the stage it was captured. Any content options that are not set as a <see cref="ViewContentType.Proposed"/> will have the
    /// same target and content id value, with the stage being the only thing needed to separate them</para>
    /// 
    /// <para>Use <see cref="HasOpt(ViewObjects.Clouds.IResultCloud,string,string,ViewObjects.ViewContentType)"/>
    /// to look for options with <see cref="ViewContentType.Proposed"/></para>
    /// </summary>
    /// <param name="obj">cloud being used</param>
    /// <param name="targetId">the target id to search for</param>
    /// <param name="stage">the stage to make sure it has</param>
    /// <returns></returns>
    public static bool HasOpt(this IResultCloud obj, string targetId, ViewContentType stage)
    {
      if(obj == default(object) || !obj.Data.Valid() || !Guid.TryParse(targetId, out _))
      {
        return false;
      }

      return obj.Data.Any(x => x.info.stage == stage && x.info.target.ViewId.Equals(targetId) && x.info.content.ViewId.Equals(targetId));
    }

    /// <summary>
    /// Searches for a <see cref="IContentInfo"/> that has a similar target and stage type
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="targetByIdOrName"></param>
    /// <param name="stage"></param>
    /// <returns></returns>
    public static bool HasTarget(this IResultCloud obj, string targetByIdOrName, ViewContentType stage)
    {
      if(obj == default(object) || !obj.Data.Valid() || !targetByIdOrName.Valid())
      {
        return false;
      }

      // is an id so search for that
      return Guid.TryParse(targetByIdOrName, out _) ?
        obj.Data.Any(x => x.info.target.ViewId.Equals(targetByIdOrName) && x.info.stage == stage)
        : obj.Data.Any(x => x.info.target.ViewName.Equals(targetByIdOrName) && x.info.stage == stage);
    }

    /// <summary>
    /// Searched for a ><see cref="IContentInfo"/> that has the same target name or id
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="targetByIdOrName"></param>
    /// <returns></returns>
    public static bool HasTarget(this IResultCloud obj, string targetByIdOrName)
    {
      if(obj == default(object) || !obj.Data.Valid() || !targetByIdOrName.Valid())
      {
        return false;
      }

      // is an id so search for that
      return Guid.TryParse(targetByIdOrName, out _) ?
        obj.Data.Any(x => x.info.target.ViewId.Equals(targetByIdOrName)) :
        obj.Data.Any(x => x.info.target.ViewName.Equals(targetByIdOrName));
    }
  }

}
