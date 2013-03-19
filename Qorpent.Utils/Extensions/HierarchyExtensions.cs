using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Model;

namespace Qorpent.Utils.Extensions {
	/// <summary>
	///     Extensions for working with hierarchy
	/// </summary>
	public static class HierarchyExtensions {

		
		/// <summary>
		///     Normalizes <see cref="IWithHierarchy{TEntity}.Parent" /> of children items of given root
		/// </summary>
		/// <param name="root">root item of hierarchy</param>
		/// <typeparam name="T"></typeparam>
		/// <exception cref="Exception">children has illegal parent defined</exception>
		public static void NormalizeParentInHierarchy<T>(this T root) 
			where  T: class,IWithHierarchy<T>,IWithId,IWithCode {
			if (null == root) return;
			if (!root.HasChildren()) return;
			var childrenWithErrorParent = root.Children.Where(_ =>_.ParentId.HasValue && root.Id!=_.ParentId.Value || !string.IsNullOrWhiteSpace(_.ParentCode) && root.Code!=_.ParentCode ).ToArray();
			if (0 != childrenWithErrorParent.Length) {
				throw new Exception(string.Format("the row {0} has {1} children with not matched parent", root, childrenWithErrorParent.Length));
			}
			var childrentWithParentToFix = root.Children.Where(_ => !_.HasParent() || _.Parent != root).ToArray();
			foreach (var i in childrentWithParentToFix) {
				i.Parent =  root;
			}
			foreach (var c in root.Children.ToArray()) {
				c.NormalizeParentInHierarchy();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static IEnumerable<T> GetSelfAndDescendantsFromHierarchy<T>(this T item)
			where T : class, IWithHierarchy<T>, IWithCode, IWithId
		{
			if(null==item)yield break;
			yield return  item;
			if(!item.HasChildren())yield break;
			foreach (var i in item.Children.SelectMany(child => child.GetSelfAndDescendantsFromHierarchy())) {
				yield return i;
			}
		}	

	

		/// <summary>
		///     Normalizes <see cref="IWithHierarchy{TEntity}.Parent" /> and <see cref="IWithHierarchy{TEntity}.Children" />  in set
		/// </summary>
		/// <param name="items">set of elements that must be joined into hierarchy</param>
		/// <typeparam name="T"></typeparam>
		/// <exception cref="Exception"></exception>
		public static IEnumerable<T> BuildHierarchy<T>(this IEnumerable<T> items)
			where T : class,IWithHierarchy<T>,IWithId,IWithCode {
			var itemsarray = items.ToArray();
			foreach (var i in itemsarray) {
				i.NormalizeParentInHierarchy();
			}
			var alldict =
				itemsarray.SelectMany(_ => _.GetSelfAndDescendantsFromHierarchy())
				          .GroupBy(_ =>new{id=_.Id,code=_.Code})
						  .ToDictionary(_=>_.Key,_ => _.FirstOrDefault(__=>__.IsParentDefined())  ?? _.First());
			var allvalues = alldict.Values.ToArray();
			foreach (var i in allvalues) {
				i.Children.Clear();
			}
			foreach (var i in allvalues) {
				if (i.IsParentDefined()) {
					var key = new {id = i.ParentId.HasValue ? i.ParentId.Value : 0, code = i.ParentCode ?? ""};
					if (!alldict.ContainsKey(key)) {
						throw new Exception("some invalid parent definition in "+i+": "+key);
					}
					var parent = alldict[key];
					parent.Children.Add(i);
					i.Parent = parent;
				}
			}
			var roots = allvalues.Where(_ => !_.IsParentDefined()).ToArray();
			return roots;
		}


		/// <summary>
		/// Build copy of given <paramref name="hierarchy"/> started with given node 
		/// </summary>
		/// <param name="hierarchy"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		/// <remarks><see cref="IWithHierarchy{TEntity}.GetCopyOfHierarchy"/> used with cast to T</remarks>
		public static T BuildHierarchyCopy<T>(this T hierarchy)
			where T : class,IWithHierarchy<T>, IWithId, IWithCode {
			if (null == hierarchy) return null;
			var result = (T) hierarchy.GetCopyOfHierarchy();
			result.IsPropagationRoot = true;
			return result;
		}


		/// <summary>
		/// Marks <paramref name="hierarchy"/> and all it's children with mark named markname using test
		/// </summary>
		/// <param name="hierarchy"></param>
		/// <param name="markname"></param>
		/// <param name="test"></param>
		/// <param name="propagation"></param>
		/// <typeparam name="T"></typeparam>
		public static void MarkHierarchy<T>(this T hierarchy, string markname, Func<T, bool> test,
		                                    HierarchyPropagation propagation = HierarchyPropagation.None)
			where T : class, IWithHierarchy<T>, IWithId, IWithCode, IWithProperties {
			var all = hierarchy.GetSelfAndDescendantsFromHierarchy().Reverse().ToArray(); //force leaf nodes to process first
			foreach (var _ in all) {
				_.LocalProperties[markname] = false;
			}
			all.AsParallel().Where(test).ForAll(_=>_.LocalProperties[markname]=true);
			if (propagation == HierarchyPropagation.Up || propagation == HierarchyPropagation.UpAndDown) {
				foreach (var marked in all.Where(_=>(bool)_.LocalProperties[markname])) {
					var current = marked;
					while (current.	HasParent()&&!current.IsPropagationRoot) {
						current = current.Parent;
						current.LocalProperties[markname] = true;
					}
				}
			}
		}

		/// <summary>
		/// Cleanup <paramref name="hierarchy"/> with given markname
		/// </summary>
		/// <param name="hierarchy"></param>
		/// <param name="markname"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T CleanupHierarchy<T>(this T hierarchy, string markname)
			where T : class, IWithHierarchy<T>, IWithId, IWithCode, IWithProperties {
			if (!hierarchy.LocalProperties.ContainsKey(markname) || !(bool) hierarchy.LocalProperties[markname]) return null;
			var nonmarked =
				hierarchy.GetSelfAndDescendantsFromHierarchy()
				         .Where(_ => !_.LocalProperties.ContainsKey(markname) || !(bool) _.LocalProperties[markname])
				         .ToArray();
			foreach (var nm in nonmarked) {
				if (nm.HasParent()) {
					nm.Parent.Children.Remove(nm);
				}
			}
			return hierarchy;
		}
	}
}