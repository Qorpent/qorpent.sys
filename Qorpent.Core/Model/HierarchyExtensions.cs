using System;
using System.Collections.Generic;
using System.Linq;

namespace Qorpent.Model {
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
		public static void NormalizeParentInHierarchy<T>(this Hierarchy<T> root) 
			where  T: Hierarchy<T> {
			if (null == root) return;
			if (!root.HasChildren()) return;
			var childrenWithErrorParent = root.Children.Where(_ =>_.ParentId.HasValue && root.Id!=_.ParentId.Value || !string.IsNullOrWhiteSpace(_.ParentCode) && root.Code!=_.ParentCode ).ToArray();
			if (0 != childrenWithErrorParent.Length) {
				throw new Exception(string.Format("the row {0} has {1} children with not matched parent", root, childrenWithErrorParent.Length));
			}
			var childrentWithParentToFix = root.Children.Where(_ => !_.HasParent() || _.Parent != root).ToArray();
			foreach (var i in childrentWithParentToFix) {
				i.Parent = (T) root;
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
		public static IEnumerable<Hierarchy<T>> GetSelfAndDescendantsFromHierarchy<T>(this Hierarchy<T> item)
			where T : Hierarchy<T>{
			if(null==item)yield break;
			yield return  item;
			if(!item.HasChildren())yield break;
			foreach (var i in item.Children.SelectMany(child => child.GetSelfAndDescendantsFromHierarchy())) {
				yield return i;
			}
		}	

		/// <summary>
		/// Evaluates default path as full code based path /CODE1/.../SELF/
		/// </summary>
		/// <param name="item"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		/// <exception cref="Exception">cannot evaluate path when code is not defined</exception>
		public static string EvaluateDefaultPath<T>(this Hierarchy<T> item)
			where T : class, IWithHierarchy<T>, IWithCode, IWithId {
			var selfcode = item.Code;
			if (string.IsNullOrWhiteSpace(selfcode))
			{
				throw new Exception("cannot evaluate path when code is not defined");
			}
			if (!item.HasParent()) return "/" + selfcode + "/";
			return item.Parent.Path + selfcode + "/";
		}

		/// <summary>
		///     Normalizes <see cref="IWithHierarchy{TEntity}.Parent" /> and <see cref="IWithHierarchy{TEntity}.Children" />  in set
		/// </summary>
		/// <param name="items">set of elements that must be joined into hierarchy</param>
		/// <typeparam name="T"></typeparam>
		/// <exception cref="Exception"></exception>
		public static IEnumerable<Hierarchy<T>> BuildHierarchy<T>(this IEnumerable<Hierarchy<T>> items)
			where T : Hierarchy<T> {
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
					parent.Children.Add((T) i);
					i.Parent = (T) parent;
				}
			}
			var roots = allvalues.Where(_ => !_.IsParentDefined()).ToArray();
			return roots;
		}
	}

	
}