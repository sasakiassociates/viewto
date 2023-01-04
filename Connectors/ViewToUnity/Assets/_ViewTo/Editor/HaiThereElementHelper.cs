#region

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

#endregion

namespace ViewTo
{
	public static class Archive
	{
		//   [MenuItem("PCX/SourceData", false, 100)]
		//   public static void CreatePcxSourceData()
		//   {
		//
		//     var asset = ScriptableObject.CreateInstance<PointCloudData>();
		//
		//     asset.Initialize(new List<Vector3>() { Vector3.zero }, new List<Color32>() { Color.black });
		//     AssetDatabase.CreateAsset(asset, "Assets/viewto-tool/Runtime/PointCloudData.asset");
		//     AssetDatabase.SaveAssets();
		//
		//     EditorUtility.FocusProjectWindow();
		//
		//     Selection.activeObject = asset;
		//   }
	}

	public static class HaiThereElementHelper
	{
		public static PopupField<TObj> CreateAndAdd<TObj>(this VisualElement root, string label, TObj value, Action<ChangeEvent<TObj>> callback)
			where TObj : Enum => CreateAndAdd(root, label, value, callback, Enum.GetValues(typeof(TObj)).Cast<TObj>().ToList());

		public static PopupField<TObj> CreateAndAdd<TObj>(
			this VisualElement root, string label, TObj value, Action<ChangeEvent<TObj>> callback, List<TObj> choices
		)
		{
			var pop = new PopupField<TObj>(label)
			{
				value = value,
				choices = choices
			};

			pop.RegisterValueChangedCallback(x => callback?.Invoke(x));
			root.Add(pop);
			return pop;
		}

		public static DropdownField SearchAndSet(
			this VisualElement root, string fieldName, List<string> items, int activeItem, Action<ChangeEvent<string>> callback
		)
		{
			var dropDown = root.Q<DropdownField>(fieldName);
			dropDown.choices = items;
			dropDown.index = activeItem;
			dropDown.RegisterValueChangedCallback(callback.Invoke);
			return dropDown;
		}
	}

}