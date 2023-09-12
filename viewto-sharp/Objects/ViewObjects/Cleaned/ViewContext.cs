using Sasaki.Common;

namespace Sasaki.ViewObjects;

/// <summary>
/// 
/// </summary>
public abstract class ViewContext : ViewObject, IContext
{ }

/// <summary>
/// 
/// </summary>
public class FocusContext : ViewContext
{ }

/// <summary>
/// 
/// </summary>
public class ProposedContext : ViewContext
{ }

/// <summary>
/// 
/// </summary>
public class ExistingContext : ViewContext
{ }

public enum ViewContextType
{
  /// <summary>
  /// <para>Known as the view target, the potential content type marks this content as a feature to analyze in the study</para>
  /// </summary>
  Potential = 0,
  /// <summary>
  /// ><para>Known as the view blocker, the <see cref="Existing"/> tag marks this content as a possible obstruction to the existing view</para>
  /// </summary>
  Existing = 1,
  /// <summary>
  /// <para><see cref="Proposed"/> are optional content types that will add their features to the existing conditions to see what changes in the view</para>
  /// </summary>
  Proposed = 2
}
