using Cysharp.Threading.Tasks;
using Speckle.ConnectorUnity;
using Speckle.ConnectorUnity.Converter;
using Speckle.ConnectorUnity.Ops;
using UnityEngine;

namespace ViewTo.Connector.Unity
{

  public class TestHub : MonoBehaviour
  {

    [SerializeField] ScriptableConverter converter;
    

    void Start() => RunClient().Forget();

    async UniTaskVoid RunClient()
    {
      const string streamId = "81c40b04df";
      const string commitId = "c13e25ca60";

      var client = new SpeckleClient(SpeckleAccountManager.GetDefaultAccount());

      client.token = this.GetCancellationTokenOnDestroy();

      var commit = await client.CommitGet(streamId, commitId);

      if(commit == null)
      {
        Debug.Log("No commit found");
        return;
      }

      var @base = await SpeckleOps.Receive(client, streamId, commit.referencedObject);

      if(@base == null)
      {
        Debug.Log("Object did not pull down");
        return;
      }

      var hierarchy = await SpeckleOps.ConvertToScene(transform, @base, converter, client.token);
      await UniTask.SwitchToMainThread();
      hierarchy.ParentAllObjects();
      
      
    }

  }

}
