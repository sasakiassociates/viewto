// [CustomEditor(typeof(BundleRunnerDebugger))]
// public class BundleRunnerDebuggerEditor : Editor
// {
//
// 	ListView _listView;
//
// 	SerializedProperty debuggerValueList;
//
// 	BundleRunnerDebugger mono;
//
// 	VisualElement root;
//
// 	VisualTreeAsset tree;
//
// 	void OnEnable()
// 	{
// 		mono = (BundleRunnerDebugger)target;
// 		var path = @"Assets/viewto-tool/Runtime/GUI/TargetPointResultGraph.uxml";
// 		tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
// 	}
//
// 	public override VisualElement CreateInspectorGUI()
// 	{
// 		root = new VisualElement();
// 		if (tree != null) tree.CloneTree(root);
//
// 		var slider = new SliderInt("current point", 0, mono.PointCount < 1 ? 1 : mono.PointCount - 1)
// 		{
// 			showInputField = true,
// 			value = mono.PointIndex
// 		};
//
// 		slider.RegisterValueChangedCallback(evt => mono.TrySetToIndex(evt.newValue));
// 		root.Add(slider);
//
// 		root.Add(new PropertyField(serializedObject.FindProperty("stage"), "stages"));
// 		root.Add(new PropertyField(serializedObject.FindProperty("DebuggerValuesList"), "debugger values"));
// 		return root;
// 	}
// }

