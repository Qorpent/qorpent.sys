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
// PROJECT ORIGIN: Qorpent.Data/MetaFacadeBase.cs
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Model;

namespace Qorpent.Data.MetaStorage {
	/// <summary>
	/// 	Абстрактный фасад репозитория
	/// </summary>
	/// <typeparam name="T"> </typeparam>
	public abstract class MetaFacadeBase<T> : ServiceBase,  IMetaFacade<T> where T : class, IWithId, IWithCode, new() {
		/// <summary>
		/// 	Возвращает строку по указанному критерию
		/// </summary>
		/// <param name="criteria"> </param>
		/// <returns> </returns>
		public virtual T Get(object criteria) {
			if (null == criteria) {
				throw new MetaStorageException("Критерий поиска строки не может быть null");
			}
			if (criteria is int) {
				var id = (int) criteria;
				if (0 == id) {
					throw new MetaStorageException("Критерий поиска строки не может быть 0");
				}
				return GetByPrimaryKey(id);
			}
			if (criteria is string) {
				var query = (string) criteria;
				if ("" == query) {
					throw new MetaStorageException("Критерий поиска строки не может быть пустой строкой");
				}

				if (IsSpecialGetQuery(query)) {
					return GetWithSpecial(query);
				}
				if (query.Contains(" ")) {
					throw new MetaStorageException("Нестандартный формат запроса " + query);
				}
				return GetByCode(query);
			}
			if (criteria is Func<T, bool>) {
				var predicat = (Func<T, bool>) criteria;
				return GetWithPredicat(predicat);
			}
			if (IsSpecialType(criteria)) {
				return GetWithSpecialType(criteria);
			}
			throw new MetaStorageException("Неизвестный тип запроса " + criteria.GetType().FullName);
		}

		/// <summary>
		/// Возвращает строки по указанному критерию
		/// </summary>
		/// <param name="criteria">условие поиска - ID,Code,Query,Func</param>
		/// <param name="persistentCode">код для сохранения запроса в специальном кэше</param>
		/// <returns> </returns>
		public IEnumerable<T> Select(Func<T, bool> criteria, string persistentCode = null) {
			return Select((object) criteria, persistentCode);
		}


		/// <summary>
		/// 	Возвращает строки по указанному критерию
		/// </summary>
		/// <param name="criteria"> условие поиска - ID,Code,Query,Func </param>
		/// <param name="persistentCode"> код для сохранения запроса в специальном кэше </param>
		/// <returns> </returns>
		public IEnumerable<T> Select(object criteria, string persistentCode = null) {
			if (null != persistentCode) {
				if (Storage.QueryCache.ContainsKey(persistentCode)) {
					return Storage.QueryCache[persistentCode];
				}
			}
			if (null == criteria) {
				throw new MetaStorageException("Критерий поиска строк не может быть null");
			}

			if (criteria is string) {
				var query = (string) criteria;
				if ("" == query) {
					throw new MetaStorageException("Критерий поиска строк не может быть пустой строкой");
				}
				if (IsSpecialSelectQuery(query)) {
					var result = SelectWithSpecial(query);
					if (null != persistentCode) {
						Storage.QueryCache[persistentCode] = result.ToArray();
					}

					return result;
				}
				if (query.Contains(" ")) {
					throw new MetaStorageException("Нестандартный формат запроса " + query);
				}
				throw new MetaStorageException("Необрабатываемый запрос на выбор строк '" + query + "'");
			}
			if (criteria is Func<T, bool>) {
				var predicat = (Func<T, bool>) criteria;
				var result = SelectWithPredicat(predicat);
				if (null != persistentCode) {
					Storage.QueryCache[persistentCode] = result.ToArray();
				}

				return result;
			}
			if (IsSpecialType(criteria)) {
				var result = SelectWithSpecialType(criteria);
				if (null != persistentCode) {
					Storage.QueryCache[persistentCode] = result.ToArray();
				}

				return result;
			}
			throw new MetaStorageException("Неизвестный тип запроса " + criteria.GetType().FullName);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="criteria"></param>
		/// <returns></returns>
		protected virtual IEnumerable<T> SelectWithSpecialType(object criteria) {
			throw new NotImplementedException();
		}

		/// <summary>
		/// 	Ссылка на локальное хранилище
		/// </summary>
		public IMetaStorage<T> Storage { get; set; }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="criteria"></param>
		/// <returns></returns>
		protected virtual T GetWithSpecialType(object criteria) {
			throw new NotImplementedException();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="criteria"></param>
		/// <returns></returns>
		protected virtual bool IsSpecialType(object criteria) {
			return false;
		}


		private T GetWithPredicat(Func<T, bool> predicat) {
			using (Storage.EnterRead()) {
				return Storage.IdCache.Select(x => x.Value).FirstOrDefault(x => predicat(x));
			}
		}

		private T GetWithSpecial(string query) {
			return SelectWithSpecial(query).FirstOrDefault();
		}

		private bool IsSpecialGetQuery(string query) {
			return IsSpecialSelectQuery(query);
		}

		private IEnumerable<T> SelectWithPredicat(Func<T, bool> predicat) {
			using (Storage.EnterRead()) {
				return Storage.IdCache.Where(x => predicat(x.Value)).Select(x => x.Value).ToArray();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		protected virtual IEnumerable<T> SelectWithSpecial(string query) {
			throw new NotImplementedException();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		protected virtual bool IsSpecialSelectQuery(string query) {
			return false;
		}

		private T GetByPrimaryKey(int id) {
			using (Storage.EnterRead()) {
				if (Storage.IdCache.ContainsKey(id))
				{
					return Storage.IdCache[id];
				}	
			}
			
			return null;
		}

		private T GetByCode(string code) {
			var id = 0;
			var isid = int.TryParse(code, out id);
			if (isid) {
				return GetByPrimaryKey(id);
			}
			using (Storage.EnterRead()) {
				if (Storage.CodeCache.ContainsKey(code))
				{
					return Storage.CodeCache[code];
				}	
			}
			
			return null;
		}
	}
}