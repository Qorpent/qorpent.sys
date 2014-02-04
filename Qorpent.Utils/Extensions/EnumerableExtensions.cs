#region LICENSE
// Copyright 2007-2013 Qorpent Team - http://github.com/Qorpent
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// PROJECT ORIGIN: Qorpent.Utils/EnumerableExtensions.cs
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Qorpent.Utils.Extensions {
	/// <summary>
	/// 	Some methods for Enumerables
	/// </summary>
	public static class EnumerableExtensions {
		/// <summary>
		/// 	test if enumerable is null or no elements or all elements are nulls
		/// </summary>
		/// <param name="e"> </param>
		/// <returns> </returns>
		public static bool IsEmptyCollection(this IEnumerable e) {
			return null == e || !e.OfType<object>().Any();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="source"></param>
		/// <param name="values"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static bool ContainsAll<T>(this IEnumerable<T> source, IEnumerable<T> values)
		{
			if (source == null || values == null) return false;
			return values.All(source.Contains);
		}
		/// <summary>
		/// Упрощенный синтаксис проверки присутствия в массиве
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="objs"></param>
		/// <returns></returns>
		public static bool IsIn(this object obj, params object[] objs) {
			return -1 != Array.IndexOf(objs, obj);
		}
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="objs"></param>
        /// <returns></returns>
        public static bool IsIn<T>(this T obj, IEnumerable<T> objs) {
            return objs.Any(_ => _.Equals(obj));
        }
		/// <summary>
		/// 	test if enumerable is not null and contains not null elements
		/// </summary>
		/// <param name="e"> </param>
		/// <returns> </returns>
		public static bool IsNotEmpty(this IEnumerable e) {
			return !IsEmptyCollection(e);
		}


		/// <summary>
		/// Applyes <paramref name="expression"/> for each element in set
		/// </summary>
		/// <typeparam name="T">type of collection </typeparam>
		/// <param name="set">collection</param>
		/// <param name="expression">delegate to execute</param>
		public static IEnumerable<T> DoForEach<T>(this IEnumerable<T> set, Action<T> expression)
		{
			return DoForEach(set, expression, null);
		}

		/// <summary>
		/// Applyes <paramref name="expression"/> for each element in set and controll exceptions
		/// </summary>
		/// <typeparam name="T">type of collection </typeparam>
		/// <param name="set">collection</param>
		/// <param name="expression">delegate to execute</param>
		/// <param name="exceptionHandler">действие при исключении</param>
		public static IEnumerable<T> DoForEach<T>(this IEnumerable<T> set, Action<T> expression, Action<T, int, Exception> exceptionHandler)
		{
			if (null == set || null == expression) return set;
			var idx = 0;
			foreach (var t in set)
			{
				try
				{
					expression(t);
					idx++;
				}
				catch (Exception ex)
				{
					if (exceptionHandler.ToBool()) exceptionHandler(t, idx, ex);
					else throw;
				}
			}
			return set;
		}
		/// <summary>
		/// Защищенный метод получения значения из словаря
		/// </summary>
		/// <typeparam name="K"></typeparam>
		/// <typeparam name="V"></typeparam>
		/// <param name="dictionary"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public static V SafeGet<K,V>(this IDictionary<K, V> dictionary , K key) {
			if(null==dictionary) return default (V);
			if(dictionary.ContainsKey(key)) return dictionary[key];
			return default(V);
		}

	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="dict"></param>
        /// <param name="correctNames"></param>
	    /// <typeparam name="R"></typeparam>
	    /// <returns></returns>
	    public static R Create<R>(this IDictionary<string, object> dict,bool correctNames=false) where R : class, new()
	    {
	       return dict.Apply(new R(), correctNames);
	    }

        /// <summary>
        /// 
        /// </summary>
        public static R Apply<T,R>(this IDictionary<string,T> dict, R target, bool correctNames = false)
        {
            if (null == target) return target;
            if (null == dict) return target;
            var type = target.GetType();
            foreach (var property in dict)
            {
                var prop = type.FindValueMember(property.Key, true, false, false, true);
                if (null == prop && correctNames && property.Key.Contains("_"))
                {
                    var correctedName = property.Key.Replace("_", "");
                    prop = type.FindValueMember(correctedName, true, false, false, true);
                }
                if (null != prop)
                {
                    try
                    {
                        prop.Set(target, property.Value.ToTargetType(prop.Type));
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            return target;
        }



        /// <summary>
        /// Возврат с дефолтным значением из словаря типа строка-строка
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="defval"></param>
        /// <typeparam name="V"></typeparam>
        /// <returns></returns>
        public static V SafeGet<V>(this IDictionary<string,string> dictionary, string key, V defval = default(V))
        {
            if (null == dictionary) return defval;
            if (dictionary.ContainsKey(key)) return dictionary[key].To<V>();
            return defval;
        }

		/// <summary>
		/// Защищенный метод получения значения из словаря
		/// </summary>
		/// <typeparam name="V"></typeparam>
		/// <param name="dictionary"></param>
		/// <param name="key"></param>
		/// <param name="defval"> </param>
		/// <returns></returns>
		public static V SafeGet<V>(this IDictionary<string, object> dictionary, string key,V defval = default(V))
		{
			if (null == dictionary) return default(V);
			if (dictionary.ContainsKey(key)) return dictionary[key].To<V>();
			return defval;
		}

		/// <summary>
		/// Защищенный метод получения значения из словаря в модели "кэша" - при отсутвии вызывает
		/// метод генератор и заполняет внутренний кэш
		/// </summary>
		/// <typeparam name="K"></typeparam>
		/// <typeparam name="V"></typeparam>
		/// <param name="dictionary"></param>
		/// <param name="key"></param>
		/// <param name="generator"> </param>
		/// <returns></returns>
		public static V CachedGet<K, V>(this IDictionary<K, V> dictionary, K key, Func<V> generator )
		{
			if (null == dictionary) return default(V);
			if (dictionary.ContainsKey(key)) return dictionary[key];
			var result = generator();
			dictionary[key] = result;
			return result;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="startpos"></param>
		/// <param name="boundaryBytes"></param>
		/// <returns></returns>
		public static Int32 IndexOf(this Byte[] buffer, int startpos, Byte[] boundaryBytes)
		{
			for (Int32 i = startpos; i <= buffer.Length - boundaryBytes.Length; i++)
			{
				Boolean match = true;
				for (Int32 j = 0; j < boundaryBytes.Length && match; j++)
				{
					match = buffer[i + j] == boundaryBytes[j];
				}

				if (match)
				{
					return i;
				}
			}

			return -1;
		}
	}
}