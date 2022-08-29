// using System.Threading;
// using ConnectorUnity;
// using Speckle.Core.Api;
// using Speckle.Core.Credentials;
// using Speckle.Core.Models;
// using UnityEngine;
// using ViewTo.Converter.Script;
// using ViewTo.Speckle;
// using ViewTo.ViewObjects.Structure;
//
// namespace ViewTo.Connector.Unity
// {
//   public class StudyUpdater
//   {
//
//     public readonly Account account;
//     public readonly StreamWrapper shell;
//     public readonly IResultCloud cloud;
//
//     public StudyUpdater(Account account, StreamShell shell, IResultCloud cloud)
//     {
//       this.account = account;
//       this.shell = shell;
//       this.cloud = cloud;
//     }
//
//     public async void AddResults()
//     {
//       if (account == null)
//       {
//         Debug.Log("Set account first to add results to study");
//         return;
//       }
//
//       if (shell == null)
//       {
//         Debug.Log("Set stream details first to add results to study");
//         return;
//       }
//
//       if (cloud == null)
//       {
//         Debug.Log("Set result cloud first to add to study");
//         return;
//       }
//
//       Debug.Log("Starting process of adding results to study");
//
//       var client = new Client(account);
//
//       var @base = await client.Receive(shell.streamId, shell.branch, shell.commitId);
//
//       Debug.Log("Study recieved, jumping onto main thread");
//
//       var mainThreadContext = SynchronizationContext.Current;
//
//       mainThreadContext.Send(_ => HandleReceive(@base), null);
//     }
//
//     private void HandleReceive(Base @base)
//     {
//       if (@base == null)
//       {
//         Debug.Log("Base object was not found properly");
//         return;
//       }
//       
//       // TODO: need to unwrap object without converting mesh geometry
//
//       if (@base is ViewStudyBase study)
//       {
//         Debug.Log("Converting result Cloud");
//         var vo = new ViewObjConverter();
//         vo.ConvertToSpeckle(cloud);
//       }
//       else
//       {
//         Debug.Log("Commit ");
//       }
//
//
//     }
//
//   }
// }

