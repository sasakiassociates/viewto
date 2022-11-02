namespace ViewObjects
{
  public enum ContentType
  {
    Target = 0,
    Existing = 1,
    Proposed = 2
  }

  public enum ResultStage
  {
    Potential = 0,
    Existing = 1,
    Proposed = 2
  }

  public enum ExplorerValueType
  {
    ExistingOverPotential = 0,
    ProposedOverExisting = 1,
    ProposedOverPotential = 2

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

  public enum ViewDirection
  {

    Up,
    Down,
    Left,
    Right,
    Front,
    Back
  }

}
