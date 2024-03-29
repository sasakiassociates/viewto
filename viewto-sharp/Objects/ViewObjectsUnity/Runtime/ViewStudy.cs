using System.Collections.Generic;
using UnityEngine;
using ViewObjects.Common;
using ViewObjects.Studies;

namespace ViewObjects.Unity
{


  public class ViewStudy : ViewObjectMono, IViewStudy
  {

    [SerializeField] string viewId;
    [SerializeField] List<ViewObjectMono> loadedObjs = new List<ViewObjectMono>();

    public bool IsValid
    {
      get => objects.Valid() && ViewName.Valid();
    }

    public string ViewId
    {
      get => viewId;
      set => viewId = value;
    }

    public string ViewName
    {
      get => gameObject.name;
      set => name = value;
    }

    public List<IViewObject> objects
    {
      get
      {
        var res = new List<IViewObject>();

        foreach(var obj in loadedObjs)
        {
          if(obj != null && obj is IViewObject casted)
            res.Add(casted);
        }

        return res;
      }
      set
      {
        loadedObjs = new List<ViewObjectMono>();

        foreach(var obj in value)
          if(obj is ViewObjectMono mono)
          {
            mono.transform.SetParent(transform);
            loadedObjs.Add(mono);
          }
          else
          {
            Debug.Log(obj.TypeName() + "- is not valid for mono");
          }
      }
    }

    public ResultCloud TrySetResults(ResultsForCloud results)
    {
      if(results == null || !results.id.Valid())
      {
        Debug.Log("Cannot set results to cloud");
        return null;
      }

      Debug.Log($"Trying to set new results to cloud {results.id}");
      ResultCloud rc = null;

      for(var i = 0; i < loadedObjs.Count; i++)
      {
        if(loadedObjs[i] is not ViewCloud cloud || !cloud.ViewId.Equals(results.id))
          continue;

        // reference game object with view cloud attached
        var go = cloud.gameObject;

        rc = go.AddComponent<ResultCloud>();
        rc.name = "Result Cloud";
        rc.ViewId = cloud.ViewId;
        rc.Points = cloud.Points;
        rc.Data = results.data;

        // Remove the view cloud component
        Destroy(go.GetComponent<ViewCloud>());

        // replace the view cloud with the result cloud
        loadedObjs[i] = rc;
      }

      return rc;
    }
  }

}
