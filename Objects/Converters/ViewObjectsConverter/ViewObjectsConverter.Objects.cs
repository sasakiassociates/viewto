using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Objects.Geometry;
using Speckle.Core.Models;
using ViewObjects.Cloud;
using ViewObjects.Speckle;
using ViewObjects.Viewer;

namespace ViewObjects.Converter
{
	public abstract partial class ViewObjectsConverter
	{

		#region Clouds

		protected ResultCloudBaseV1 ResultCloudToSpeckle(IResultCloud @object) => new ResultCloudBaseV1
		{
			data = @object.data.Convert(), points = @object.points.ToList().ToSpeckle(), ViewId = @object.ViewId
		};

		protected ViewCloudBaseV1 ViewCloudToSpeckle(IViewCloud @object) => @object is ResultCloud rc ?
			ResultCloudToSpeckle(rc) : new ViewCloudBaseV1
			{
				points = @object.points.ToList().ToSpeckle(), ViewId = @object.ViewId
			};

		protected IViewCloud ViewCloudToNative(ViewCloudBaseV1 baseV1)
		{
			var obj = Schema?.nativeViewCloud;
			obj.ViewId = baseV1.ViewId;
			obj.points = baseV1.points.ToView().ToArray();
			return obj;
		}

		protected IResultCloud ResultCloudToNative(ResultCloudBaseV1 baseV1)
		{
			var obj = Schema?.nativeResultCloud;
			obj.ViewId = baseV1.ViewId;
			obj.points = baseV1.points.ToView().ToArray();
			obj.data = baseV1.data.Convert();
			return obj;
		}

		#endregion

		#region View Content

		protected abstract bool ViewContentDataToSpeckle(List<object> items, out List<Base> result);

		protected abstract bool ViewContentDataToNative(List<Base> items, out List<object> result);

		protected TObj ViewContentToSpeckle<TObj>(IViewContent content) where TObj : ViewContentBaseV1
		{
			if (ViewContentToSpeckle(content) is TObj casted)
				return casted;

			return null;
		}

		protected ViewContentBaseV1 ViewContentToSpeckle(IViewContent content)
		{
			ViewContentBaseV1 vc = null;
			switch (content)
			{
				case ITargetContent o:
					vc = TargetContentToSpeckle(o);
					break;
				case IBlockerContent o:
					vc = BlockerContentToSpeckle(o);
					break;
				case IDesignContent o:
					vc = DesignContentToSpeckle(o);
					break;
				default:
					return null;
			}

			if (ViewContentDataToSpeckle(content.objects, out var result))
				vc.objects = result;

			return vc;
		}

		protected TargetContentBaseV1 TargetContentToSpeckle(ITargetContent @object) => new TargetContentBaseV1
		{
			ViewName = @object.ViewName, isolate = @object.isolate, bundles = SafeConvert(@object.bundles)
		};

		protected DesignContentBaseV1 DesignContentToSpeckle(IDesignContent @object) => new DesignContentBaseV1
		{
			ViewName = @object.ViewName
		};

		protected BlockerContentBaseV1 BlockerContentToSpeckle(IBlockerContent @object) => new BlockerContentBaseV1
		{
			ViewName = @object.ViewName
		};

		protected IViewContent ViewContentToNative(ViewContentBaseV1 content)
		{
			IViewContent vc = default;
			switch (content)
			{
				case TargetContentBaseV1 o:
					vc = TargetContentToNative(o);
					break;
				case BlockerContentBaseV1 o:
					vc = BlockerContentToNative(o);
					break;
				case DesignContentBaseV1 o:
					vc = DesignContentToNative(o);
					break;
			}

			if (ViewContentDataToNative(content.objects, out var result)) vc.objects = result;

			return vc;
		}

		protected TObj ViewContentToNative<TObj>(ViewContentBaseV1 baseV1) where TObj : IViewContent
		{
			if (ViewContentToNative(baseV1) is TObj casted)
				return casted;

			return default;
		}

		protected ITargetContent TargetContentToNative(TargetContentBaseV1 baseV1)
		{
			var viewObj = Schema?.nativeTargetContent;

			viewObj.isolate = baseV1.isolate;
			viewObj.ViewName = baseV1.ViewName;
			viewObj.viewColor = baseV1.viewColor;
			viewObj.bundles = SafeConvert(baseV1.bundles);

			return viewObj;
		}

		protected IDesignContent DesignContentToNative(DesignContentBaseV1 baseV1)
		{
			var viewObj = Schema?.nativeDesignContent;
			viewObj.ViewName = baseV1.ViewName;
			return viewObj;
		}

		protected IBlockerContent BlockerContentToNative(BlockerContentBaseV1 baseV1)
		{
			var viewObj = Schema?.nativeBlockerContent;
			viewObj.ViewName = baseV1.ViewName;
			return viewObj;
		}

		#endregion

		#region Safety

		protected List<TResult> SafeConvertToSpeckle<TValue, TResult>(List<IViewContent> items)
			where TValue : IViewContent
			where TResult : ViewContentBaseV1
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
			where TValue : ViewContentBaseV1
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

		protected List<ViewerBundleBaseV1> SafeConvert(List<IViewerBundle> items)
		{
			var results = new List<ViewerBundleBaseV1>();
			if (!items.Valid()) return results;

			results.AddRange(items.Select(ViewerBundleToSpeckle).Where(i => i != null));
			return results;
		}

		protected List<IViewerBundle> SafeConvert(List<ViewerBundleBaseV1> items)
		{
			var results = new List<IViewerBundle>();
			if (!items.Valid()) return results;

			results.AddRange(items.Select(ViewerBundleToNative).Where(i => i != null));
			return results;
		}

		#endregion

		#region Viewers

		protected ViewerLayout LayoutToNative(IViewerLayout @base)
		{
			switch (@base)
			{
				case ViewerLayoutBaseV1Cube _:
					return new ViewerLayoutCube();
				case ViewerLayoutBaseV1Horizontal _:
					return new ViewerLayoutHorizontal();
				case ViewerLayoutBaseV1Ortho o:
					return new ViewerLayoutOrtho
					{
						size = o.size
					};
				case ViewerLayoutBaseV1Normal o:
					return new ViewerLayoutNormal
					{
						shell = new CloudShell(typeof(ViewCloud), o.shellId, 0)
					};
				case ViewerLayoutBaseV1Focus o:
					return new ViewerLayoutFocus
					{
						x = o.focusPoint?.x ?? 0, y = o.focusPoint?.y ?? 0, z = o.focusPoint?.z ?? 0
					};
				case ViewerLayoutBaseV1 _:
					return new ViewerLayout();
				default:
					throw new ArgumentOutOfRangeException(nameof(@base), @base, null);
			}
		}

		protected IViewerBundle ViewerBundleToNative(ViewerBundleBaseV1 baseV1)
		{
			var viewObj = Schema?.nativeViewerBundle;

			if (baseV1 == null)
				return viewObj;

			viewObj.layouts = new List<IViewerLayout>();
			viewObj.layouts.AddRange(baseV1.layouts.Select(LayoutToNative));

			if (baseV1 is ViewerBundleBaseV1Linked vl)
			{
				var viewObjLink = Schema?.nativeViewerBundleLinked;

				viewObjLink.layouts = viewObj.layouts;
				viewObjLink.linkedClouds = vl.linkedClouds.ToView();

				return viewObjLink;
			}

			return viewObj;
		}

		protected ViewerBundleBaseV1 ViewerBundleToSpeckle<TObj>(TObj @object) where TObj : IViewerBundle
		{
			var casted = new List<IViewerLayout>();
			casted.AddRange(@object.layouts.Select(LayoutToSpeckle));

			if (@object is ViewerBundleLinked vl)
				return new ViewerBundleBaseV1Linked
				{
					layouts = casted, cloudsToFind = vl.linkedClouds.Select(shell => shell.objId).ToList()
				};

			return new ViewerBundleBaseV1
			{
				layouts = casted
			};
		}

		protected ViewerLayoutBaseV1 LayoutToSpeckle<TObj>(TObj @object) where TObj : IViewerLayout
		{
			switch (@object)
			{
				case ViewerLayoutCube _:
					return new ViewerLayoutBaseV1Cube();
				case ViewerLayoutHorizontal _:
					return new ViewerLayoutBaseV1Horizontal();
				case ViewerLayoutOrtho o:
					return new ViewerLayoutBaseV1Ortho
					{
						size = o.size
					};
				case ViewerLayoutNormal o:
					return new ViewerLayoutBaseV1Normal
					{
						shellId = o.shell.objId
					};
				case ViewerLayoutFocus o:
					return new ViewerLayoutBaseV1Focus
					{
						focusPoint = new Point
						{
							x = o.x, y = o.y, z = o.z
						}
					};
				case ViewerLayout _:
					return new ViewerLayoutBaseV1();
				default:
					throw new ArgumentOutOfRangeException(nameof(@object), @object, null);
			}
		}

		#endregion

		protected ViewStudyBaseV1 StudyToSpeckle(IViewStudy @object)
		{
			var casted = new List<ViewObjectBase_v1>();
			foreach (var obj in @object.objs)
			{
				var @base = ConvertToSpeckle(obj);
				if (@base is ViewObjectBase_v1 vo)
				{
					if (!vo.id.Valid()) vo.id = vo.GetId();

					casted.Add(vo);
				}
			}

			return new ViewStudyBaseV1
			{
				ViewName = @object.ViewName, objs = casted
			};
		}

		protected ContentBundleBaseV1 ContentBundleToSpeckle(IViewContentBundle @object) => new ContentBundleBaseV1
		{
			targets = SafeConvertToSpeckle<ITargetContent, TargetContentBaseV1>(@object.contents),
			blockers = SafeConvertToSpeckle<IBlockerContent, BlockerContentBaseV1>(@object.contents),
			designs = SafeConvertToSpeckle<IDesignContent, DesignContentBaseV1>(@object.contents)
		};

		protected IViewContentBundle ContentBundleToNative(ContentBundleBaseV1 baseV1)
		{
			var obj = Schema?.nativeContentBundle;

			var items = new List<IViewContent>();
			items.AddRange(SafeConvertToNative<TargetContentBaseV1, ITargetContent>(baseV1.targets));
			items.AddRange(SafeConvertToNative<BlockerContentBaseV1, IBlockerContent>(baseV1.blockers));
			items.AddRange(SafeConvertToNative<DesignContentBaseV1, IDesignContent>(baseV1.designs));

			obj.contents = items;

			return obj;
		}

		protected IViewStudy StudyToNative(ViewStudyBaseV1 baseV1)
		{
			var viewObj = Schema?.nativeViewStudy;

			viewObj.ViewName = baseV1.ViewName;
			var items = new List<IViewObj>();

			if (baseV1.objs.Valid())
			{
				foreach (var obj in baseV1.objs)
				{
					if (obj == null)
						continue;

					if (!obj.id.Valid())
						obj.id = obj.GetId();

					var item = ConvertToNative(obj);
					if (item is IViewObj vo)
						items.Add(vo);
				}

				viewObj.objs = items;
			}

			return viewObj;
		}

		private IViewObj HandleDefault(Base @base)
		{
			IViewObj o = default;

			foreach (var item in @base.GetMemberNames())
			{
				var member = @base[item];

				if (member.IsList())
				{
					var objects = ((IEnumerable)member).Cast<object>();
					foreach (var obj in objects)
					{
						o = RecursiveToNative(obj);
					}
				}

				if (member.IsBase(out var child))
					o = HandleDefault(child);
			}

			return o;
		}

		private IViewObj RecursiveToNative(object @object)
		{
			if (@object == null)
			{
				Console.WriteLine("Base object is not a wrapper, sending back default");
				return default;
			}

			// is basic speckle type
			if (@object is ViewObjectBase_v1 vo && ConvertToNative(vo) is IViewObj viewObj) return viewObj;

			if (@object.IsList())
			{
				var list = ((IEnumerable)@object).Cast<object>().ToList();
				// // TODO handle sending back trees
				foreach (var item in list)
				{
					var res = RecursiveToNative(item);
					if (res != default)
						return res;
				}
			}

			return default;
		}

	}
}