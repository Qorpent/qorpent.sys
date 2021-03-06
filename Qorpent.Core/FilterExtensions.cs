﻿using System.Collections.Generic;
using System.Linq;

namespace Qorpent{
	/// <summary>
	/// Расширения для фильтров
	/// </summary>
	public static class FilterExtensions{
		/// <summary>
		/// F-style filter caller with filter state support
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="filters"></param>
		/// <param name="target"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public static T Apply<T>(this IEnumerable<IFilter<T>> filters, T target, IScope context = null)where T:class{
			if (null == target) return null;
			if (null == filters) return target;
			context = context ?? new Scope();
			foreach (var filter in filters.OrderBy(_=>_.Idx)){
				if (filter.IsMatch(target, context)){
					var state = filter.Apply(target, context);
					if (state == FilterState.Finished) break;
				}
			}
			return target;
		}


	}
}