﻿using System;
using System.Collections;
using System.Linq;

namespace ViewObjects.Common
{

  public static class ObjUtils
  {

    public static string InitGuid => Guid.NewGuid().ToString();

    /// <summary>
    ///   Shorthand for getting trimming the object type name to the last value
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string TypeName(this object obj)
    {
      return obj != null ? obj.GetType().ToString().Split('.').Last() : "null";
    }

    /// <summary>
    ///  Shorthand for checking list object
    /// </summary>
    /// <param name="list"></param>
    /// <returns>returns true if list contains at least one item</returns>
    public static bool Valid(this IList list)
    {
      return list != null && list.Count > 0;
    }

    /// <summary>
    ///   Shorthand for checking list object
    ///   Use <seealso cref="Valid(System.Collections.IList)" /> when using <paramref name="count" /> as 0
    /// </summary>
    /// <param name="list"></param>
    /// <param name="count">Any value above 0</param>
    /// <returns>if list is not null and has the min <paramref name="count" /> of items </returns>
    public static bool Valid(this IList list, int count)
    {
      return list != null && list.Count > count;
    }

    /// <summary>
    ///   Shorthand for checking list object
    /// </summary>
    /// <param name="list"></param>
    /// <returns>returns true if list contains at least one item</returns>
    public static bool Valid(this ICollection list)
    {
      return list != null && list.Count > 0;
    }

    /// <summary>
    ///   Shorthand for checking list object
    ///   Use <seealso cref="Valid(System.Collections.ICollection)" /> when using <paramref name="count" /> as 0
    /// </summary>
    /// <param name="list"></param>
    /// <param name="count">Any value above 0</param>
    /// <returns>if list is not null and has the min <paramref name="count" /> of items </returns>
    public static bool Valid(this ICollection list, int count)
    {
      return list != null && list.Count > count;
    }

    /// <summary>
    ///   Shorthand for checking if string is null or empty
    /// </summary>
    /// <param name="value"></param>
    /// <returns>returns true if value contains something</returns>
    public static bool Valid(this string value)
    {
      return!string.IsNullOrEmpty(value);
    }

    public static string CheckIfValidId(string valueId)
    {
      return!string.IsNullOrEmpty(valueId) && Guid.TryParse(valueId, out _) ? valueId : InitGuid;
    }
  }

}
