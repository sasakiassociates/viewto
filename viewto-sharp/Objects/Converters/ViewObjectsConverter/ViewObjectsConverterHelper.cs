using Speckle.Core.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ViewObjects.Converter
{

  public static class ViewObjectsConverterHelper
  {

    public static TBase SearchForType<TBase>(this Base obj, bool recursive) where TBase : Base
    {
      if(obj is TBase simpleCast)
      {
        return simpleCast;
      }

      foreach(var member in obj.GetMembers().Keys)
      {
        var nestedObj = obj[member];

        // 1. Direct cast for object type 
        if(nestedObj.IsBase(out TBase memberCast))
        {
          return memberCast;
        }

        // 2. Check if member is base type
        if(nestedObj.IsBase(out var nestedBase))
        {
          var objectToFind = nestedBase.SearchForType<TBase>(recursive);

          if(objectToFind != default(object))
          {
            return objectToFind;
          }
        }
        else if(nestedObj.IsList(out var nestedList))
        {
          foreach(var listObj in nestedList)
          {
            if(listObj.IsBase(out TBase castedListObjectType))
            {
              return castedListObjectType;
            }

            // if not set to recursive we dont look through any other objects
            if(!recursive)
            {
              continue;
            }

            // if its not a base object we turn around
            if(!listObj.IsBase(out var nestedListBase))
            {
              continue;
            }

            var objectToFind = nestedListBase.SearchForType<TBase>(true);
            if(objectToFind?.GetType() == typeof(TBase))
            {
              return objectToFind;
            }
          }
        }
      }

      return null;
    }

    public static bool IsList(this object obj, out List<object> list)
    {
      list = new List<object>();

      if(obj.IsList())
      {
        list = ((IEnumerable)obj).Cast<object>().ToList();
      }

      return list.Any();
    }

    public static bool IsList(this object obj)
    {
      return obj != null && obj.GetType().IsList();
    }

    public static bool IsList(this Type type)
    {
      if(type == null)
      {
        return false;
      }

      return typeof(IEnumerable).IsAssignableFrom(type) && !typeof(IDictionary).IsAssignableFrom(type) && type != typeof(string);
    }

    public static bool IsBase<TBase>(this object value, out TBase @base) where TBase : Base
    {
      @base = default(TBase);

      if(value != null && !value.GetType().IsSimpleType() && value is TBase o)
      {
        @base = o;
      }

      return @base != null;
    }

    public static bool IsBase(this object value, out Base @base)
    {
      @base = null;

      if(value != null && !value.GetType().IsSimpleType() && value is Base o)
      {
        @base = o;
      }

      return @base != null;
    }
  }

}
