using System;

namespace ViewTo;

public static partial class ViewCoreExtensions
{
  /// <summary>
  /// Short hand for safety convert
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static int ToBitForm(this uint value) =>
    BitConverter.ToInt32(BitConverter.GetBytes(value), 0);

  /// <summary>
  ///  Short hand for safety convert
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static uint ToBitForm(this int value) =>
    BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
}
