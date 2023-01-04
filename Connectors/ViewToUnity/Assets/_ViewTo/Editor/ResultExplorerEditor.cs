// using System.Threading.Tasks;
// using UnityEditor;
// using UnityEditor.UIElements;
// using UnityEngine;
// using UnityEngine.UIElements;
// using ViewTo.Connector.Unity;
// using ViewTo.Objects.Mono;
//
// namespace ViewTo
// {
//
//   [CustomEditor(typeof(ResultExplorerMono))]
//   public class ResultExplorerMonoEditor : Editor
//   {
//     private ResultExplorerMono mono;
//     private VisualElement root;
//     private VisualTreeAsset tree;
//
//     public void OnEnable()
//     {
//       mono = (ResultExplorerMono)target;
//     }
//
//     public override VisualElement CreateInspectorGUI()
//     {
//       root = new VisualElement();
//
//       // set result cloud obj
//       var object_cloudMono = new ObjectField("result cloud")
//       {
//         bindingPath = "resultCloud",
//         objectType = typeof(ResultCloudMono)
//       };
//
//       // object_cloudMono.RegisterValueChangedCallback(evt => mono.Load((ResultCloudMono)evt.newValue));
//       root.Add(object_cloudMono);
//
//       root.Add(new ObjectField("viewer")
//       {
//         bindingPath = "viewer",
//         objectType = typeof(FirstPersonViewer)
//       });
//
//       root.Add(new ObjectField("rig")
//       {
//         bindingPath = "rig",
//         objectType = typeof(RigMono)
//       });
//
//       var slider_pointSize = new Slider("point size")
//       {
//         showInputField = true,
//         highValue = 10f,
//         lowValue = 0f,
//         value = mono.pointSize,
//         bindingPath = "pointSize"
//       };
//
//       slider_pointSize.RegisterValueChangedCallback(evt => mono.SetPointSize());
//       root.Add(slider_pointSize);
//
//
//       var tog_usePow = new Toggle("use pow")
//       {
//         value = mono.usePow, bindingPath = nameof(mono.usePow)
//       };
//
//       tog_usePow.RegisterValueChangedCallback(evt => mono.UpdateCloud());
//       root.Add(tog_usePow);
//
//
//       var slider_powValue = new Slider("pow")
//       {
//         showInputField = true,
//         highValue = 10.00f,
//         lowValue = 0.01f,
//         value = mono.powNum,
//         bindingPath = nameof(mono.powNum)
//       };
//
//       slider_powValue.RegisterValueChangedCallback(evt => mono.UpdateCloud());
//       root.Add(slider_powValue);
//
//       var gradient_valueColors = new GradientField("gradient")
//       {
//         bindingPath = "gradient"
//       };
//       gradient_valueColors.RegisterValueChangedCallback(evt => mono.MapColors(evt.newValue));
//       root.Add(gradient_valueColors);
//
//
//       root.Add(CreateLabel("view content"));
//
//       root.CreateAndAdd("active target", mono.activeTarget, x => mono.SetTarget(x.newValue), mono.targets);
//       root.CreateAndAdd("filters", mono.obstructedFilter, x => mono.SetFilter(x.newValue));
//
//
//       var slider_range = new MinMaxSlider("range", 0f, 1f, 0f, 1f)
//       {
//         tooltip = "value range to filter points by",
//         bindingPath = nameof(mono.range)
//       };
//       slider_range.RegisterValueChangedCallback(evt => mono.UpdateCloud());
//       root.Add(slider_range);
//
//       root.Add(CreateLabel("viewer rig"));
//
//       var tog_useRunner = new Toggle("use rig")
//       {
//         value = false, bindingPath = nameof(mono.useRunner)
//       };
//
//       tog_useRunner.SetEnabled(Application.isPlaying);
//       tog_useRunner.RegisterValueChangedCallback(evt => mono.ToggleRunner());
//       root.Add(tog_useRunner);
//
//       root.CreateAndAdd("mask type", mono.activeMask, x => mono.SetMask(x.newValue));
//
//       var slider_activePoint = new SliderInt("active point")
//       {
//         showInputField = true,
//         bindingPath = "activePointIndex",
//         highValue = mono.GetPointCount,
//         lowValue = 0,
//         value = mono.ActivePointIndex
//       };
//
//       slider_activePoint.RegisterValueChangedCallback(evt => { mono.SetViewerPos(); });
//       root.Add(slider_activePoint);
//
//       var slider_cameraRotation = new Slider(0f, 360f) { label = "rotation" };
//       slider_cameraRotation.RegisterValueChangedCallback(evt => mono.SetViewerRotation(evt.newValue));
//       root.Add(slider_cameraRotation);
//
//
//       var tog_showRunnerPos = new Toggle("Show Runner")
//       {
//         value = mono.showFru, bindingPath = nameof(mono.showFru)
//       };
//       tog_showRunnerPos.RegisterValueChangedCallback(evt => mono.ToggleRunnerPos());
//       root.Add(tog_showRunnerPos);
//
//
//       root.Add(CreateLabel("recorded values"));
//
//
//       root.Add(CreateFloatField(nameof(mono.potentialValue), "potential"));
//       root.Add(CreateFloatField(nameof(mono.existValue), "exisiting"));
//       root.Add(CreateFloatField(nameof(mono.proposedValue), "proposed"));
//
//       root.Add(CreateLabel("runner values"));
//
//       root.Add(CreateFloatField(nameof(mono.potentialValueRunner), "potential"));
//       root.Add(CreateFloatField(nameof(mono.existValueRunner), "exisiting"));
//       root.Add(CreateFloatField(nameof(mono.proposedValueRunner), "proposed"));
//
//       BuildTestActions();
//
//       root.Add(CreateLabel("test events"));
//
//       root.Add(new PropertyField(serializedObject.FindProperty("OnActivePointSet")));
//       root.Add(new PropertyField(serializedObject.FindProperty("OnActiveValueSet")));
//       return root;
//     }
//
//     private void BuildTestActions()
//     {
//       root.Add(CreateLabel("test actions"));
//
//       var container_testButtons = new VisualElement
//       {
//         style =
//         {
//           flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row),
//           flexBasis = new StyleLength(StyleKeyword.Auto)
//         }
//       };
//
//       container_testButtons.Add(new Button(() => mono.FindBest())
//       {
//         text = "Find Best"
//       });
//       container_testButtons.Add(new Button(() => mono.SetRunner())
//       {
//         text = "Create Runner"
//       });
//
//       container_testButtons.Add(new Button(() => Task.Run(() => { mono.CheckPoints(); }))
//       {
//         text = "Check Points"
//       });
//
//       root.Add(container_testButtons);
//     }
//     private static Label CreateLabel(string text) => new Label(text)
//     {
//       style =
//       {
//         alignSelf = new StyleEnum<Align>(Align.Center),
//         unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold),
//         unityTextOutlineColor = new StyleColor(new Color32(32, 32, 32, 255)),
//         paddingTop = 2,
//         paddingBottom = 2
//       }
//     };
//
//     private static FloatField CreateFloatField(string bPath, string label)
//     {
//       return new FloatField
//       {
//         bindingPath = bPath,
//         label = label,
//         isReadOnly = true
//       };
//     }
//
//   }
//
// }

