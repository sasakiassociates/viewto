#region

using UnityEditor;
using UnityEngine.UIElements;
using ViewTo.Connector.Unity;

#endregion

[CustomEditor(typeof(ViewToHub))]
public class ViewToControllerEditor : Editor
{
	VisualTreeAsset tree;

	void OnEnable()
	{
		tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/viewto-tool/Runtime/GUI/ViewToController.uxml");
	}

	public override VisualElement CreateInspectorGUI()
	{
		return base.CreateInspectorGUI();
		// var mono = (ViewToController)target;

		if (tree == null)
			return base.CreateInspectorGUI();

		var root = new VisualElement();
		tree.CloneTree(root);

		var container = root.Q<VisualElement>("container-group");
		return root;
	}
}