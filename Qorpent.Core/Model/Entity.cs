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
// PROJECT ORIGIN: Qorpent.Core/Entity.cs
#endregion
using System;
using System.Collections.Generic;
using Qorpent.Utils.Extensions;

namespace Qorpent.Model {
	/// <summary>
	/// Simple <see cref="IEntity"/> implementation
	/// </summary>
	public class Entity : IEntity, IWithProperties
	{
	    private object _context;
		/// <summary>
		/// 
		/// </summary>
		protected IDictionary<string, object> _localProperties;

		/// <summary>
		/// PK ID in database terms
		/// </summary>
		public virtual int Id { get; set; }

		/// <summary>
		/// Unique memo-code
		/// </summary>
		public virtual string Code { get; set; }

		/// <summary>
		///Name of the entity
		/// </summary>
		public virtual string Name { get; set; }


		/// <summary>
		/// User-defined order index
		/// </summary>
		public virtual int Idx { get; set; }


		/// <summary>
		///The TAG string
		/// </summary>
		public virtual string Tag { get; set; }

		/// <summary>
		/// Проверяет наличие установленного тега, сверяет его значение
		/// </summary>
		/// <param name="tagname"></param>
		/// <param name="testvalue"></param>
		/// <returns></returns>
		public virtual bool IsTagSet(string tagname, string testvalue = null) {
			if (string.IsNullOrWhiteSpace(Tag)) return false;
			if (null == testvalue) {
				return Tag.Contains("/" + tagname + ":");
			}
			return Tag.Replace(" ","").Contains("/" + tagname + ":" + testvalue + "/");
		}

		/// <summary>
		/// Возвращает наличие установленного тега
		/// </summary>
		/// <param name="tagname"></param>
		/// <returns></returns>
		public virtual string TagGet(string tagname) {
			if (string.IsNullOrWhiteSpace(Tag)) return string.Empty;
			var teststr = "/" + tagname + ":";
			var startindex = Tag.IndexOf(teststr, StringComparison.Ordinal);
			if (-1 == startindex) return string.Empty;
			startindex += teststr.Length;
			var endindex = Tag.IndexOf("/", startindex, StringComparison.Ordinal);
			if (-1 == endindex) return string.Empty;
			var length = endindex - startindex;
			return Tag.Substring(startindex, length).Trim();
		}
		/// <summary>
		///		Типизированное получение тега
		/// </summary>
		/// <typeparam name="T">Типизация результата</typeparam>
		/// <param name="tagname">Имя тега</param>
		/// <param name="def">Дефолтное значение</param>
		/// <returns>Типизированный результат</returns>
		public T TagGet<T>(string tagname, T def) {
			return TagGet(tagname).To<T>();
		}

		/// <summary>
		/// User's comment string
		/// </summary>
		public virtual string Comment { get; set; }

		/// <summary>
		/// User's or system's time stamp
		/// </summary>
		public virtual DateTime Version { get; set; }

		/// <summary>
		///     Helper code that maps any foreign coding system
		/// </summary>
		public virtual string OuterCode { get; set; }

		/// <summary>
	    /// Общий метод установления некоего контекстного объекта, использование зависит от реализации
	    /// </summary>
	    /// <param name="context"></param>
	    public virtual  void SetContext(object context) {
	        _context = context;
	    }

	    /// <summary>
	    /// Возвращает контекстный объект
	    /// </summary>
	    /// <returns></returns>
	    public object GetContext() {
	        return _context;
	    }

		/// <summary>
		/// Ключ сортировки
		/// </summary>
		/// <returns></returns>
		public string GetSortKey()
		{
			return string.Format("{0:00000}_{1}_{2}", Idx, OuterCode ?? "", Code);
		}

		/// <summary>
		/// 
		/// </summary>
		public IDictionary<string, object> LocalProperties{
			get { return _localProperties??(_localProperties=new Dictionary<string, object>()); }
			set { _localProperties = value; }
		}
	}
}