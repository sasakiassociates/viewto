using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Speckle.ConnectorUnity.Converter;
using Speckle.Core.Models;
using UnityEngine;
using ViewObjects.Converter.Script;
using ViewObjects.Unity;

namespace ViewObjects.Converter.Unity
{

	public class ViewObjectsConverterUnity : ScriptableSpeckleConverter
	{

		[SerializeField] ScriptableSpeckleConverter _supportConverter;

		ViewObjectsConverterScript wrappedConverter { get; set; }

		public override void SetConverterSettings(object settings)
		{
			base.SetConverterSettings(settings);
			_supportConverter.SetConverterSettings(new ScriptableConverterSettings() { style = ConverterStyle.Queue });
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			
			wrappedConverter ??= new ViewObjectsConverterScript()
			{
				SupportConverter = _supportConverter,
				Schema = new ViewObjectUnitySchema()
			};
		}

		public override async UniTask PostWork()
		{
			if (wrappedConverter.SupportConverter is ScriptableSpeckleConverter sc && sc.converters.Valid())
			{
				foreach (var c in sc.converters)
				{
					if (c == null)
						continue;

					await c.PostWorkAsync();
					await UniTask.Yield();
				}
			}
			
			foreach (var c in converters)
			{
				if (c == null)
					continue;

				await c.PostWorkAsync();
				await UniTask.Yield();
			}
		}

		/// <summary>
		/// this does not work correctly or should not be returning the serialized list. 
		/// </summary>
		/// <returns></returns>
		public override List<ComponentConverter> StandardConverters()
		{
			var items = new List<ComponentConverter>
			{
				CreateInstance<MeshComponentConverter>(),
				CreateInstance<PolylineComponentConverter>(),
				CreateInstance<PointComponentConverter>(),
				CreateInstance<PointCloudComponentConverter>(),
				CreateInstance<View3DComponentConverter>(),
				CreateInstance<BrepComponentConverter>()
			};
			return items;
		}

		public override bool CanConvertToNative(Base @base) =>
			wrappedConverter.CanConvertToNative(@base) || wrappedConverter.SupportConverter.CanConvertToNative(@base);

		public override Base ConvertToSpeckle(object @object)
		{
			if (@object is GameObject go)
			{
				var res = go.GetComponent(typeof(ViewObjMono));
				return wrappedConverter.ConvertToSpeckle(res);
			}

			return wrappedConverter.SupportConverter.ConvertToSpeckle(@object);
		}

		public override bool CanConvertToSpeckle(object @object)
		{
			if (@object is GameObject go)
			{
				var res = go.GetComponents(typeof(ViewObjMono));

				foreach (var comp in res)
					if (wrappedConverter.CanConvertToSpeckle(comp))
						return true;
			}

			// try to convert anyways 
			return wrappedConverter.SupportConverter.CanConvertToSpeckle(@object);
		}

		public override object ConvertToNative(Base @base)
		{
			if (@base == null)
			{
				Debug.LogWarning("Trying to convert a null object! Beep Beep! I don't like that");
				return null;
			}

			if (wrappedConverter.CanConvertToNative(@base))
			{
				var vo = wrappedConverter.ConvertToNative(@base);
				return vo;
			}

			Debug.Log($"Converting with native kit for {@base.speckle_type}");

			return wrappedConverter.SupportConverter.ConvertToNative(@base);
		}

	}
}