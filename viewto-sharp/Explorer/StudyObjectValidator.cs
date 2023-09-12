using System.Collections.Generic;
using System.Linq;
using ViewObjects;
using ViewObjects.Clouds;
using ViewObjects.Contents;
using ViewObjects.Systems;

namespace ViewTo.Receivers
{

  public class StudyObjectValidator
  {
    /// <summary>
    ///   Simple check for looking at a list for objects
    /// </summary>
    /// <param name="list"></param>
    /// <param name="message"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public bool DataIsValid<T>(IReadOnlyList<T> list, out string message)
    {
      message = null;
      if(list == null || list.Count > 0)
      {
        message = $"Collection of {list.GetType().GetGenericArguments().Single()} is invalid";
      }

      return!string.IsNullOrEmpty(message);
    }

    public bool CompareClouds(IEnumerable<IViewer> viewers, IReadOnlyList<ICloud> clouds, out string message)
    {
      message = "";

      foreach(var viewer in viewers)
      {
        if(viewer is IViewerLinked sys)
        {
          if(sys.Clouds == null || !sys.Clouds.Any())
          {
            continue;
          }

          foreach(var cloudId in sys.Clouds)
          {
            if(!clouds.Any(x => x.guid.Equals(cloudId)))
            {
              message = $"No Id for {nameof(ICloud)} found. Looking for id {cloudId}";
              return false;
            }
          }
        }
      }

      return true;
    }

    public bool CheckData(IReadOnlyList<IContext> contents, IReadOnlyList<ICloud> clouds, IReadOnlyList<IViewer> viewers, out string message)
    {
      var countTarget = contents.Count(x => x.contentType == ViewContextType.Potential);
      var countExisting = contents.Count(x => x.contentType == ViewContextType.Existing);
      var countProposed = contents.Count(x => x.contentType == ViewContextType.Proposed);
      var countViewerLinked = viewers.Count(x => x is IViewerLinked);

      var countTotalPoints = 0;
      foreach(var o in clouds)
      {
        if(o?.Points != null)
        {
          countTotalPoints += o.Points.Length;
        }
      }

      var countTotalLayouts = 0;
      foreach(var o in viewers)
      {
        if(o?.Layouts != null)
        {
          countTotalLayouts += o.Layouts.Count;
        }
      }

      message = $"{nameof(IContext)}s: "
                + $"{nameof(ViewContextType.Potential)}={countTarget}, "
                + $"{nameof(ViewContextType.Existing)}={countExisting}, "
                + $"{nameof(ViewContextType.Proposed)}={countProposed}\n"
                + $"{nameof(ICloud)}s: "
                + $"Total={clouds.Count}, "
                + $"Points={countTotalPoints}\n"
                + $"{nameof(IViewer)}s: "
                + $"Global={viewers.Count - countViewerLinked}, "
                + $"Linked={countViewerLinked}, "
                + $"Layouts={countTotalLayouts}\n";

      return countTarget > 0 && countExisting > 0 && countViewerLinked > 0;
    }
  }

}
