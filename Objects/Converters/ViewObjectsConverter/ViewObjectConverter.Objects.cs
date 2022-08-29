using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Objects.Geometry;
using Speckle.Core.Models;
using ViewObjects.Cloud;
using ViewObjects.Speckle;
using ViewObjects.Viewer;

namespace ViewObjects.Converter.Script
{
	public partial class ViewObjectConverter
	{
		protected virtual TObj ViewContentToSpeckle<TObj>(IViewContent content) where TObj : ViewContentBase
		{
			if (ViewContentToSpeckle(content) is TObj casted)
				return casted;

			return null;
		}

		protected virtual ViewContentBase ViewContentToSpeckle(IViewContent content)
		{
			ViewContentBase vc = null;
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

		protected virtual TargetContentBase TargetContentToSpeckle(ITargetContent @object) => new TargetContentBase
		{
			viewName = @object.viewName, isolate = @object.isolate, bundles = SafeConvert(@object.bundles)
		};

		protected virtual DesignContentBase DesignContentToSpeckle(IDesignContent @object) => new DesignContentBase
		{
			viewName = @object.viewName
		};

		protected virtual BlockerContentBase BlockerContentToSpeckle(IBlockerContent @object) => new BlockerContentBase
		{
			viewName = @object.viewName
		};

		protected IViewContent ViewContentToNative(ViewContentBase content)
		{
			IViewContent vc = default;
			switch (content)
			{
				case TargetContentBase o:
					vc = TargetContentToNative(o);
					break;
				case BlockerContentBase o:
					vc = BlockerContentToNative(o);
					break;
				case DesignContentBase o:
					vc = DesignContentToNative(o);
					break;
			}

			if (ViewContentDataToNative(content.objects, out var result))
				vc.objects = result;

			return vc;
		}

		protected virtual bool ViewContentDataToSpeckle(List<object> items, out List<Base> result)
		{
			result = new List<Base>();
			return true;
		}

		protected virtual bool ViewContentDataToNative(List<Base> items, out List<object> result)
		{
			result = new List<object>();
			return true;
		}

		protected virtual TObj ViewContentToNative<TObj>(ViewContentBase @base) where TObj : IViewContent
		{
			if (ViewContentToNative(@base) is TObj casted)
				return casted;

			return default;
		}

		protected virtual ITargetContent TargetContentToNative(TargetContentBase @base)
		{
			var viewObj = CreateNativeTargetContent();

			viewObj.isolate = @base.isolate;
			viewObj.viewName = @base.viewName;
			viewObj.viewColor = @base.viewColor;
			viewObj.bundles = SafeConvert(@base.bundles);

			return viewObj;
		}

		protected virtual IDesignContent DesignContentToNative(DesignContentBase @base)
		{
			var viewObj = CreateNativeDesignContent();

			viewObj.viewName = @base.viewName;

			return viewObj;
		}

		protected virtual IBlockerContent BlockerContentToNative(BlockerContentBase @base)
		{
			var viewObj = CreateNativeBlockerContent();

			viewObj.viewName = @base.viewName;

			return viewObj;
		}

		protected ViewerLayout LayoutToNative(IViewerLayout @base)
		{
			switch (@base)
			{
				case ViewerLayoutBaseCube _:
					return new ViewerLayoutCube();
				case ViewerLayoutBaseHorizontal _:
					return new ViewerLayoutHorizontal();
				case ViewerLayoutBaseOrtho o:
					return new ViewerLayoutOrtho
					{
						size = o.size
					};
				case ViewerLayoutBaseNormal o:
					return new ViewerLayoutNormal
					{
						shell = new CloudShell(typeof(ViewCloud), o.shellId, 0)
					};
				case ViewerLayoutBaseFocus o:
					return new ViewerLayoutFocus
					{
						x = o.focusPoint?.x ?? 0, y = o.focusPoint?.y ?? 0, z = o.focusPoint?.z ?? 0
					};
				case ViewerLayoutBase _:
					return new ViewerLayout();
				default:
					throw new ArgumentOutOfRangeException(nameof(@base), @base, null);
			}
		}

		protected IViewerBundle ViewerBundleToNative(ViewerBundleBase @base)
		{
			var viewObj = CreateNativeViewerBundle();

			if (@base == null)
				return viewObj;

			viewObj.layouts = new List<IViewerLayout>();
			viewObj.layouts.AddRange(@base.layouts.Select(LayoutToNative));

			if (@base is ViewerBundleBaseLinked vl)
			{
				var viewObjLink = CreateNativeViewerBundleLinked();

				viewObjLink.layouts = viewObj.layouts;
				viewObjLink.linkedClouds = vl.linkedClouds.ToView();

				return viewObjLink;
			}

			return viewObj;
		}

		protected ViewerBundleBase ViewerBundleToSpeckle<TObj>(TObj @object) where TObj : IViewerBundle
		{
			var casted = new List<IViewerLayout>();
			casted.AddRange(@object.layouts.Select(LayoutToSpeckle));

			if (@object is ViewerBundleLinked vl)
				return new ViewerBundleBaseLinked
				{
					layouts = casted, cloudsToFind = vl.linkedClouds.Select(shell => shell.objId).ToList()
				};

			return new ViewerBundleBase
			{
				layouts = casted
			};
		}

		protected ViewerLayoutBase LayoutToSpeckle<TObj>(TObj @object) where TObj : IViewerLayout
		{
			switch (@object)
			{
				case ViewerLayoutCube _:
					return new ViewerLayoutBaseCube();
				case ViewerLayoutHorizontal _:
					return new ViewerLayoutBaseHorizontal();
				case ViewerLayoutOrtho o:
					return new ViewerLayoutBaseOrtho
					{
						size = o.size
					};
				case ViewerLayoutNormal o:
					return new ViewerLayoutBaseNormal
					{
						shellId = o.shell.objId
					};
				case ViewerLayoutFocus o:
					return new ViewerLayoutBaseFocus
					{
						focusPoint = new Point
						{
							x = o.x, y = o.y, z = o.z
						}
					};
				case ViewerLayout _:
					return new ViewerLayoutBase();
				default:
					throw new ArgumentOutOfRangeException(nameof(@object), @object, null);
			}
		}

		protected virtual ViewStudyBase StudyToSpeckle(IViewStudy @object)
		{
			var casted = new List<ViewObjBase>();
			foreach (var obj in @object.objs)
			{
				var @base = ConvertToViewObjBase(obj);
				if (@base != null)
				{
					if (!@base.id.Valid())
						@base.id = @base.GetId();

					casted.Add(@base);
				}
			}

			return new ViewStudyBase
			{
				viewName = @object.viewName, objs = casted
			};
		}

		protected virtual ContentBundleBase ContentBundleToSpeckle(IViewContentBundle @object) => new ContentBundleBase
		{
			targets = SafeConvertToSpeckle<ITargetContent, TargetContentBase>(@object.contents),
			blockers = SafeConvertToSpeckle<IBlockerContent, BlockerContentBase>(@object.contents),
			designs = SafeConvertToSpeckle<IDesignContent, DesignContentBase>(@object.contents)
		};

		protected IViewContentBundle ContentBundleToNative(ContentBundleBase @base)
		{
			var obj = CreateNativeViewContentBundle();

			var items = new List<IViewContent>();
			items.AddRange(SafeConvertToNative<TargetContentBase, ITargetContent>(@base.targets));
			items.AddRange(SafeConvertToNative<BlockerContentBase, IBlockerContent>(@base.blockers));
			items.AddRange(SafeConvertToNative<DesignContentBase, IDesignContent>(@base.designs));

			obj.contents = items;

			return obj;
		}

		protected IViewStudy StudyToNative(ViewStudyBase @base)
		{
			var viewObj = CreateNativeViewStudy();

			viewObj.viewName = @base.viewName;
			var items = new List<IViewObj>();

			if (@base.objs.Valid())
			{
				foreach (var obj in @base.objs)
				{
					if (obj == null)
						continue;

					if (!obj.id.Valid())
						obj.id = obj.GetId();

					var item = ConvertToViewObj(obj);
					if (item != null)
						items.Add(item);
				}

				viewObj.objs = items;
			}

			return viewObj;
		}

		private IViewObj HandleDefault(Base @base)
		{
			IViewObj o = default;
			// check if there are layers in the ref object
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
			if (@object is ViewObjBase vo) return ConvertToViewObj(vo);

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

				// if (list.Any())
				// {
				//   // NOTE: to avoid handling trees, we only use the top most item to convert
				//   var item = list.FirstOrDefault();
				//   if (item is ViewObjBase treeBase)
				//     return ConvertToViewObj(treeBase);
				// }
			}

			return default;
		}

		private bool IsList(object @object)
		{
			if (@object == null)
				return false;

			var type = @object.GetType();

			return typeof(IEnumerable).IsAssignableFrom(type) && !typeof(IDictionary).IsAssignableFrom(type) && type != typeof(string);
		}

		#region Clouds

		protected IResultCloud ResultCloudToNative(ResultCloudBase @base)
		{
			var obj = CreateNativeResultCloud();

			obj.viewID = @base.viewID;
			obj.data = @base.data.Convert();
			obj.points = @base.points.ToView().ToArray();

			return obj;
		}

		protected virtual ResultCloudBase ResultCloudToSpeckle(IResultCloud @object) => new ResultCloudBase
		{
			data = @object.data.Convert(), points = @object.points.ToList().ToSpeckle(), viewID = @object.viewID
		};

		protected virtual ViewCloudBase ViewCloudToSpeckle(IViewCloud @object) => @object is ResultCloud rc ?
			ResultCloudToSpeckle(rc) :
			new ViewCloudBase
			{
				points = @object.points.ToList().ToSpeckle(), viewID = @object.viewID
			};

		protected IViewCloud ViewCloudToNative(ViewCloudBase @base)
		{
			// current way of passing hash
			var obj = CreateNativeViewCloud();

			obj.viewID = @base.viewID;
			obj.points = @base.points.ToView().ToArray();

			return obj;
		}

		#endregion

	}

}