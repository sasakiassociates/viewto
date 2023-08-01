namespace ViewObjects
{

  public enum ViewContentType
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

  public enum ExplorerValueType
  {
    ExistingOverPotential = 0,
    ProposedOverExisting = 1,
    ProposedOverPotential = 2

  }

  public enum ViewDirection
  {
    Up,
    Down,
    Left,
    Right,
    Front,
    Back
  }

  public static class ViewerDir
  {
    public const string UP = "Up";
    public const string DOWN = "Down";
    public const string LEFT = "Left";
    public const string RIGHT = "Right";
    public const string FRONT = "Front";
    public const string BACK = "Back";
  }

}
