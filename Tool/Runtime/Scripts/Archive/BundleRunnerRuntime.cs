// using System;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
// using ViewObjects;
// using ViewObjects.Content;
// using ViewObjects.Unity;
//
// namespace ViewTo.Connector.Unity
// {
//   public class BundleRunnerRuntime : BundleRunner
//   {
//     public event Action<ResultCloudMono> OnDone;
//
//     private void CompileStageData(RigStage rigStage)
//     {
//       Debug.Log($"Last point reached for {stage}");
//
//       string meta = null;
//       if (stage == RigStage.Design)
//       {
//         var c = designContentInScene.FirstOrDefault();
//         meta = c.viewName;
//         designContentInScene.Remove(c);
//       }
//
//       foreach (var sys in layouts)
//         sys.RecordStageData(rigStage, meta);
//
//       Debug.Log("All Systems Compiled");
//
//       CheckNextStage();
//     }
//
//     private void CheckNextStage()
//     {
//       SetNewStage = stage.GetNextStage();
//
//       if (stage == RigStage.Complete || stage == RigStage.Design && !designContentInScene.Valid())
//       {
//         ViewConsole.Log($"{name} complete! Saving All Data");
//
//         var cloud = new GameObject().AddComponent<ResultCloudMono>();
//
//         cloud.points = points.Select(x => x.ToView()).ToArray();
//         cloud.data = BundleData(layouts.ToDictionary(sys => sys.name, sys => sys.GetData()));
//
//         Completed();
//         OnDone?.Invoke(cloud);
//       }
//       else
//       {
//         Debug.Log($"Starting on {stage}");
//         StartRunner();
//       }
//     }
//
//     private List<IResultData> BundleData(Dictionary<string, List<ResultData32>> tempData)
//     {
//       var pixelData = new List<IResultData>();
//
//
//       foreach (var sys in tempData)
//       {
//         int color = System.Drawing.Color.Black.ToArgb();
//
//         foreach (var d in sys.Value)
//         {
//
//           string contentName = null;
//           foreach (var i in nameAndColor)
//           {
//             if (i.Compare(d.color))
//             {
//               contentName = i.content;
//               color = i.ToUnity().ToHex();
//               break;
//             }
//           }
//
//           pixelData.Add(
//             new ContentResultData(d.values.ToList(), d.stage.ToString().Split('.').Last(),
//                                   contentName, color, d.meta, sys.Key));
//         }
//       }
//       return pixelData;
//     }
//
//     protected override void CheckSystemAtPoint()
//     {
//       if (!LayoutsComplete)
//         return;
//
//
//       if (!CheckRepeatThreshold())
//       {
//         PreRenderSystems();
//         return;
//       }
//
//       IsRunning(false);
//
//       foreach (var layout in layouts)
//         layout.RecordDataAt(PointIndex);
//
//       PointIndex++;
//       if (PointIndex < PointCount)
//       {
//         PreRenderSystems();
//         SetToPoint();
//       }
//       else
//       {
//         CompileStageData(stage);
//       }
//     }
//
//     protected override ViewerBundleSystem CreateLayoutSystem(ViewerLayoutMono layout)
//     {
//       var layoutSys = base.CreateLayoutSystem(layout);
//       OnStartRun += layoutSys.StartRun;
//       return layoutSys;
//     }
//
//   }
// }

