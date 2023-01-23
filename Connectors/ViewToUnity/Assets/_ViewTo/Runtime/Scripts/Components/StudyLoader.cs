using Cysharp.Threading.Tasks;
using Objects.Geometry;
using Speckle.ConnectorUnity;
using Speckle.ConnectorUnity.Converter;
using Speckle.ConnectorUnity.Models;
using Speckle.ConnectorUnity.Ops;
using Speckle.Core.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using ViewObjects.Clouds;
using ViewObjects.Common;
using VS = ViewObjects.Speckle;
using VU = ViewObjects.Unity;

namespace ViewTo.Connector.Unity
{

  public class StudyLoader : MonoBehaviour
  {

    SpeckleClient _client;
    SpeckleStreamObject _stream;

    public UnityAction onLoadComplete;

    public async UniTask Run(VU.ViewStudy obj, SpeckleClient client, SpeckleStreamObject stream, ScriptableConverter converter)
    {

      if(stream == null || !stream.IsValid())
      {
        Debug.Log("Stream is not valid to use for populating the study with");
        return;
      }
      if(client == null || !client.IsValid())
      {
        Debug.Log("Client is not valid");
        return;
      }
      if(converter == null)
      {
        Debug.Log("No valid converter used");
        return;
      }

      if(obj == null || !ObjUtils.Valid(obj.Objects))
      {
        Debug.Log("View study is not valid to pull reference objects from");
        return;
      }

      _stream = stream;
      _client = client;

      // start pulling objects one by one 
      foreach(var viewObj in obj.Objects)
      {
        if(viewObj is not IStreamReference)
        {
          Debug.Log($"Not a reference object, dont care about {viewObj.GetType()}!");
          continue;

        }
        switch(viewObj)
        {
          case VU.Content vo:
            var items = await PullFromStream(vo);
            foreach(var i in items)
            {
              var hierarchy = await SpeckleOps.ConvertToScene(vo.transform, i, converter, _client.token);
              UniTask.SwitchToMainThread();
              hierarchy.ParentAllObjects();
            }
            vo.Objects = GetKids(vo.transform).ToList();
            break;
          case VU.ViewCloud vo:
            var data = await PullFromStream(vo);
            TryLoadReference(vo, data.FirstOrDefault(), out Pointcloud pc);
            vo.Points = Utils.ToVectorArray(pc.points, pc.units).Select(v => new CloudPoint(v.x, v.y, v.z)).ToArray();
            break;
          default: break;
        }

        Debug.Log("Object from study complete");
        UniTask.Yield();

      }

      onLoadComplete?.Invoke();

    }

    async UniTask<List<Base>> PullFromStream(IStreamReference source)
    {
      var items = new List<Base>();

      foreach(var commitId in source.References)
      {
        if(_client.token.IsCancellationRequested)
        {
          return items;
        }

        var refCommit = await _client.CommitGet(_stream.Id, commitId);
        var @base = await SpeckleOps.Receive(_client, _stream.Id, refCommit.referencedObject);
        items.Add(@base);
      }

      UniTask.Yield();

      return items;
    }


    static List<GameObject> GetKids(Transform parent)
    {
      var currentList = new List<GameObject>();

      foreach(Transform child in parent)
      {
        currentList.Add(child.gameObject);
        if(child.childCount > 0)
          currentList.AddRange(GetKids(child));
      }

      return currentList;
    }

    static bool TryLoadReference<TBase, TMono>(TMono mono, Base refObj, out TBase obj) where TBase : Base where TMono : VU.ViewObjectMono
    {
      obj = default(TBase);

      if(mono == null || refObj == null)
      {
        Debug.Log("Objects are not valid\n" +
                  $"{typeof(TMono)}={(mono == null ? "null" : "valid")}\n" +
                  $"{typeof(TBase)}={(refObj == null ? "null" : "valid")}"
        );

        return false;
      }

      obj = refObj.SearchForTypeSync<TBase>(true);

      Debug.Log($"{typeof(TBase)} was {(obj == null ? "not found" : "found")}");

      return obj != null;
    }
  }

}
