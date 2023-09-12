using System;
using System.Collections.Generic;
using System.Linq;
using ViewObjects;
using ViewObjects.Clouds;
using Sasaki.Common;
using ViewObjects.Contents;

namespace ViewTo
{


  public static partial class ViewCoreExtensions
  {
    private static TObj Fabricate<TObj>() => Activator.CreateInstance<TObj>();

    /// <summary>
    /// Finds all <see cref="IResultCondition"/> within a <see cref="IResultCloud"/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static List<ResultCondition> GetAllOpts(this IResultCloud obj)
    {
      List<ResultCondition> result = new();

      if(obj == default(object) || !obj.layers.Valid())
      {
        return result;
      }
      result = obj.layers.Where(x => x?.info != null).Select(x => x.info).ToList();

      return result;
    }

    /// <summary>
    /// Finds all <see cref="IResultCondition"/> within a <see cref="IResultCloud"/> with a specific <see cref="ViewContextType"/>
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="type">the type to filter by</param>
    /// <returns></returns>
    public static List<ResultCondition> GetAllOpts(this IResultCloud obj, ViewContextType type)
    {
      return obj.GetAllOpts().Where(x => x.stage == type).ToList();
    }

    /// <summary>
    /// Gets a single <see cref="ResultCondition"/> that matches the ids of the <seealso cref="ResultCondition.focus"/> and <see cref="ReResultCondition.obstruct> content 
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="targetId"></param>
    /// <param name="contentId"></param>
    /// <param name="stage"></param>
    /// <returns></returns>
    public static ResultCondition GetOpt(this IResultCloud obj, string targetId, string contentId, ViewContextType stage)
    {
      ResultCondition result = Fabricate<ResultCondition>();

      if(obj == default(object) || !obj.layers.Valid() || !targetId.Valid() || !contentId.Valid())
      {
        return result;
      }

      if(!Guid.TryParse(targetId, out _) && !Guid.TryParse(contentId, out _))
      {
        return result;
      }

      foreach(var d in obj.layers)
      {
        if(d?.info == null ||
           d.info.stage != stage ||
           !d.info.target.guid.Equals(targetId) ||
           !d.info.content.guid.Equals(contentId))
        {
          continue;
        }
        result = d.info;
        break;
      }
      return result;

    }

    /// <summary>
    /// Grabs all of the targets as a list of <see cref="IContextInfo"/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static List<IContextInfo> GetAllTargets(this IResultCloud obj)
    {
      List<IContextInfo> result = new();

      if(obj == default(object) || !obj.layers.Valid())
      {
        return result;
      }

      foreach(var d in obj.layers)
      {
        if(d?.info == null || result.Any(x => x.appId.Equals(d.info.target.guid)))
        {
          continue;
        }

        result.Add(d.info.target);
        break;
      }
      return result;

    }

    /// <summary>
    /// Grabs a <see cref="IContextInfo"/> that is linked to a target type
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="targetId">the target id to search for</param>
    /// <returns></returns>
    public static IContextInfo GetTarget(this IResultCloud obj, string targetId)
    {
      IContextInfo result = Fabricate<ContextInfo>();

      if(obj == default(object) || !obj.layers.Valid() || !targetId.Valid())
      {
        return result;
      }

      if(Guid.TryParse(targetId, out _))
      {
        return result;
      }

      foreach(var d in obj.layers)
      {
        if(d?.info == null || !d.info.target.guid.Equals(targetId))
        {
          continue;
        }

        result = d.info.target;
        break;
      }

      return result;

    }

    /// <summary>
    /// Searches for a <see cref="ResultCondition"/> that has a similar target, content, and stage type
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="targetId"></param>
    /// <param name="contentId"></param>
    /// <param name="stage"></param>
    /// <returns></returns>
    public static bool HasOpt(this IResultCloud obj, string targetId, string contentId, ViewContextType stage)
    {
      if(obj == default(object) || !obj.layers.Valid() || !Guid.TryParse(targetId, out _) || !Guid.TryParse(contentId, out _))
      {
        return false;
      }

      return obj.layers.Any(x => x.info.stage == stage && x.info.target.guid.Equals(targetId) && x.info.content.guid.Equals(contentId));
    }

    /// <summary>
    /// <para>Searches for a <see cref="ResultCondition"/> that has a similar target and stage type. This will select the target by id
    /// and the stage it was captured. Any content options that are not set as a <see cref="ViewContextType.Proposed"/> will have the
    /// same target and content id value, with the stage being the only thing needed to separate them</para>
    /// 
    /// <para>Use <see cref="HasOpt(ViewObjects.Clouds.IResultCloud,string,string,ViewContextType)"/>
    /// to look for options with <see cref="ViewContextType.Proposed"/></para>
    /// </summary>
    /// <param name="obj">cloud being used</param>
    /// <param name="targetId">the target id to search for</param>
    /// <param name="stage">the stage to make sure it has</param>
    /// <returns></returns>
    public static bool HasOpt(this IResultCloud obj, string targetId, ViewContextType stage)
    {
      if(obj == default(object) || !obj.layers.Valid() || !Guid.TryParse(targetId, out _))
      {
        return false;
      }

      return obj.layers.Any(x => x.info.stage == stage && x.info.target.guid.Equals(targetId) && x.info.content.guid.Equals(targetId));
    }

    /// <summary>
    /// Searches for a <see cref="IContextInfo"/> that has a similar target and stage type
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="targetByIdOrName"></param>
    /// <param name="stage"></param>
    /// <returns></returns>
    public static bool HasTarget(this IResultCloud obj, string targetByIdOrName, ViewContextType stage)
    {
      if(obj == default(object) || !obj.layers.Valid() || !targetByIdOrName.Valid())
      {
        return false;
      }

      // is an id so search for that
      return Guid.TryParse(targetByIdOrName, out _) ?
        obj.layers.Any(x => x.info.target.guid.Equals(targetByIdOrName) && x.info.stage == stage)
        : obj.layers.Any(x => x.info.target.name.Equals(targetByIdOrName) && x.info.stage == stage);
    }

    /// <summary>
    /// Searched for a ><see cref="IContextInfo"/> that has the same target name or id
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="targetByIdOrName"></param>
    /// <returns></returns>
    public static bool HasTarget(this IResultCloud obj, string targetByIdOrName)
    {
      if(obj == default(object) || !obj.layers.Valid() || !targetByIdOrName.Valid())
      {
        return false;
      }

      // is an id so search for that
      return Guid.TryParse(targetByIdOrName, out _) ?
        obj.layers.Any(x => x.info.target.guid.Equals(targetByIdOrName)) :
        obj.layers.Any(x => x.info.target.name.Equals(targetByIdOrName));
    }
  }

}
