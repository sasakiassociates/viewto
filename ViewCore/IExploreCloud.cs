using ViewObjects;

namespace ViewTo
{
  public interface IExploreCloud
  {
    public IResultCloud source { get; }
    
    public string activeTarget { get; set; }

    public int[] activeValues { get; set; }

    public int activePoint { get; set; }
  }
}
