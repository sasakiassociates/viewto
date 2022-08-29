// using System;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
// using ViewObjects;
// using ViewObjects.Unity;
// using ViewTo.Connector.Unity.Tests;
//
// namespace ViewTo.Connector.Unity
// {
//   [ExecuteAlways]
//   public class BundleRunnerDebugger : BundleRunner
//   {
//     [SerializeField] private bool drawGizmo;
//     [SerializeField] private List<LayoutDebuggerValues> valuesList;
//     [SerializeField] private LayoutDebuggerValues debugValues;
//
//     public double GetValue(ResultType type)
//     {
//       return debugValues == null ? 0 : type switch
//       {
//         ResultType.Potential => debugValues.potential,
//         ResultType.Existing => debugValues.existing,
//         ResultType.Proposed => debugValues.proposed,
//         _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
//       };
//     }
//
//     public void SetStage(ResultType type)
//     {
//       SetNewStage = type switch
//       {
//         ResultType.Potential => RigStage.Target,
//         ResultType.Existing => RigStage.Blocker,
//         ResultType.Proposed => RigStage.Design,
//         _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
//       };
//     }
//     
//     public List<Color32> GetColors
//     {
//       get => viewColors.Valid() ? viewColors.Select(x => x.ToUnity()).ToList() : new List<Color32>();
//     }
//
//     protected override ViewerBundleSystem CreateLayoutSystem(ViewerLayoutMono layout)
//     {
//       var layoutSys = base.CreateLayoutSystem(layout);
//       valuesList ??= new List<LayoutDebuggerValues>();
//
//       var dv = new LayoutDebuggerValues { sys = layoutSys };
//
//       layoutSys.OnLayoutComplete += () => dv.SetData(stage);
//
//       valuesList.Add(dv);
//
//       return layoutSys;
//     }
//
//     protected void OnValidate()
//     {
//       SetNewStage = stage;
//     }
//
//     protected override void CheckSystemAtPoint()
//     {
//       if (!LayoutsComplete || IsComplete)
//         return;
//
//       foreach (var vl in valuesList)
//         vl.SetData(stage);
//
//       debugValues = valuesList.FirstOrDefault();
//
//     }
//
//     public void OnDrawGizmos()
//     {
//       if (!drawGizmo)
//         return;
//
//       Gizmos.color = Color.white;
//       foreach (var sys in layouts)
//       {
//         foreach (var viewer in sys.ViewerSystems)
//         {
//           // Gizmos.matrix = viewer.transform.localToWorldMatrix;
//           // viewer.GetViewCam/**/.Camera.DrawFrustum();
//           Gizmos.DrawCube(viewer.transform.position, new Vector3(10, 10, 10));
//           // Gizmos.matrix = Matrix4x4.TRS(viewer.transform.position, viewer.transform.rotation, Vector3.one);
//           // Gizmos.DrawFrustum(viewer.transform.position, cam.fov, 100, cam.near, cam.aspect);
//         }
//
//       }
//     }
//
//     public void VisualizeLocation(bool value)
//     {
//       drawGizmo = value;
//     }
//   }
// }

