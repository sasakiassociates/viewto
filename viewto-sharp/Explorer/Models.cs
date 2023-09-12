using System;
using System.Collections.Generic;
using System.Linq;
using ViewObjects;
using ViewObjects.Clouds;
using ViewObjects.Contents;
using ViewObjects.Studies;
using ViewObjects.Systems;
using ViewTo.Values;

namespace ViewTo
{

  /// <summary>
  /// A simple input object to use when searching a <see cref="IResultCloud"/> for a specific <see cref="IResultCondition"/>
  /// </summary>
  public class ContentOptionInput
  {
    /// <summary>
    /// Constructor for searching for <see cref="IResultCondition"/> that is a <see cref="ViewContextType.Potential"/> or <see cref="ViewContextType.Existing"/> type
    /// </summary>
    /// <param name="stage">Stage to search for </param>
    /// <param name="targetId">The target id to find</param>
    public ContentOptionInput(ViewContextType stage, string targetId)
    {
      this.stage = stage;
      this.targetId = targetId;
      this.contentId = targetId;
    }

    /// <summary>
    /// Constructor for searching for <see cref="IResultCondition"/> that is only a <see cref="ViewContextType.Proposed"/> type
    /// </summary>
    /// <param name="stage">Stage to search for </param>
    /// <param name="contentId">The proposed content in option</param>
    /// <param name="targetId">The target in option</param>
    public ContentOptionInput(ViewContextType stage, string contentId, string targetId)
    {
      this.stage = stage;
      this.contentId = contentId;
      this.targetId = targetId;
    }

    public ViewContextType stage {get;private set;}
    public string contentId {get;private set;}
    public string targetId {get;private set;}
  }


  /// <summary>
  /// A simple object that returns all of <see cref="IViewStudy.objects"/> data within a <see cref="IViewStudy"/>/>
  /// </summary>
  public class DeconstructedStudy
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="viewers"></param>
    /// <param name="clouds"></param>
    /// <param name="results"></param>
    /// <param name="proposed"></param>
    /// <param name="existing"></param>
    /// <param name="targets"></param>
    public DeconstructedStudy(List<IViewer> viewers, List<ICloud> clouds, List<IResultCloud> results, List<IContext> proposed, List<IContext> existing, List<IContext> targets)
    {
      this.viewers = viewers;
      this.clouds = clouds;
      this.results = results;
      this.proposed = proposed;
      this.existing = existing;
      this.targets = targets;
    }

    public List<IViewer> viewers {get;private set;}
    public List<ICloud> clouds {get;private set;}
    public List<IResultCloud> results {get;private set;}
    public List<IContext> proposed {get;private set;}
    public List<IContext> existing {get;private set;}
    public List<IContext> targets {get;private set;}

  }

  public class ExplorerFilterInput
  {
    public readonly IntRange pixelRange;
    public readonly DoubleRange valueRange;
    public readonly int[] indexes;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="filteredIndexes"></param>
    /// <param name="maxValue"></param>
    /// <param name="pixelMin"></param>
    /// <param name="pixelMax">This should be related to the max value received by the pixel finder</param>
    /// <param name="minValue"></param>
    public ExplorerFilterInput(double minValue = 0.0, double maxValue = 1.0, int pixelMin = 0, int pixelMax = 16384, IEnumerable<int> filteredIndexes = null)
    {
      this.valueRange = new DoubleRange {min = minValue, max = maxValue};
      this.pixelRange = new IntRange {min = pixelMin, max = pixelMax};
      this.indexes = filteredIndexes == null ? Array.Empty<int>() : filteredIndexes.ToArray();
    }

    public override string ToString() => $"Pixel Range:{pixelRange}\nValue Range:{valueRange}\nIndexes:{indexes.Length}";

  }


}
