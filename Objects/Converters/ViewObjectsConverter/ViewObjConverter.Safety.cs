using System.Collections.Generic;
using System.Linq;
using ViewObjects.Speckle;

namespace ViewObjects.Converter.Script
{
	public partial class ViewObjectConverter
	{
		protected List<TResult> SafeConvertToSpeckle<TValue, TResult>(List<IViewContent> items)
			where TValue : IViewContent
			where TResult : ViewContentBase
		{
			var results = new List<TResult>();
			if (!items.Valid()) return results;

			foreach (var item in items)
				if (item is TValue)
				{
					var res = ViewContentToSpeckle<TResult>(item);
					if (res != null)
						results.Add(res);
				}

			return results;
		}

		protected List<TResult> SafeConvertToNative<TValue, TResult>(List<TValue> items)
			where TValue : ViewContentBase
			where TResult : IViewContent
		{
			var results = new List<TResult>();
			if (!items.Valid()) return results;

			foreach (var item in items)
			{
				var obj = ViewContentToNative<TResult>(item);
				if (obj != null)
					results.Add(obj);
			}

			return results;
		}

		protected List<ViewerBundleBase> SafeConvert(List<IViewerBundle> items)
		{
			var results = new List<ViewerBundleBase>();
			if (!items.Valid()) return results;

			results.AddRange(items.Select(ViewerBundleToSpeckle).Where(i => i != null));
			return results;
		}

		protected List<IViewerBundle> SafeConvert(List<ViewerBundleBase> items)
		{
			var results = new List<IViewerBundle>();
			if (!items.Valid()) return results;

			results.AddRange(items.Select(ViewerBundleToNative).Where(i => i != null));
			return results;
		}
	}
}