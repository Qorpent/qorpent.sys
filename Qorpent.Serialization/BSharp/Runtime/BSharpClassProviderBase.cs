using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Qorpent.BSharp.Runtime {
	/// <summary>
	/// </summary>
	public abstract class BSharpClassProviderBase :ServiceBase, IBSharpRuntimeProvider {
		/// <summary>
		///     Кэш загруженных дескрипторов
		/// </summary>
		protected IDictionary<string, BSharpRuntimeClassDescriptor> Cache =
			new Dictionary<string, BSharpRuntimeClassDescriptor>();

		/// <summary>
		///     Маркер первичной загрузки
		/// </summary>
		protected bool IndexWasBuilt = false;


		/// <summary>
		///     Разрешает имена классов с использованием корневого неймспейса
		///     используется при поздних референсах
		/// </summary>
		/// <param name="name"></param>
		/// <param name="rootnamespace"></param>
		/// <returns></returns>
		public string Resolve(string name, string rootnamespace) {
		    lock (this) {
		        CheckIndex();

		        if (string.IsNullOrWhiteSpace(rootnamespace)) {
		            if (Cache.ContainsKey(name)) {
		                return name;
		            }
			        return Cache.Keys.FirstOrDefault(_ => _.Split('.').Last() == name);
		        }

		        string[] nssplit = rootnamespace.Split('.');
		        for (int i = nssplit.Length; i >= 0; i--) {
		            string query = name;
		            if (i != 0) {
		                query = string.Join(".", nssplit.Take(i)) + "." + name;
		            }
		            if (Cache.ContainsKey(query)) {
		                return query;
		            }
		        }

		        return null;
		    }
		}

	    private void CheckIndex() {
	        if (!IndexWasBuilt) {
	            lock (this) {
	                if (!IndexWasBuilt) {
	                    Refresh();
	                }
	            }
	        }
	    }

	    /// <summary>
		///     Возвращает исходное определение класса BSharp
		/// </summary>
		/// <param name="fullname"></param>
		/// <returns></returns>
		public IBSharpRuntimeClass GetRuntimeClass(string fullname) {
			lock (this) {
				if (!IndexWasBuilt) Refresh();
				if (!Cache.ContainsKey(fullname)){
					fullname = Resolve(fullname, "");
					if (null==fullname || !Cache.ContainsKey(fullname)){
						return null;
					}
				}
				BSharpRuntimeClassDescriptor descriptor = Cache[fullname];
				if (null == descriptor.CachedClass || !descriptor.CachedClass.Loaded || !IsActual(descriptor) ) {
					ReloadClass(descriptor);
				}
				return descriptor.CachedClass;
			}
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> GetClassNames(string mask = null) {
			lock (this) {
				if (!IndexWasBuilt) Refresh();
				if (string.IsNullOrWhiteSpace(mask)) return Cache.Keys;
				return Cache.Keys.Where(_ => IsMatchMask(_, mask));
			}
		}


		/// <summary>
		///     Метод обновления кэшей при их наличии
		/// </summary>
		public virtual void Refresh() {
			lock (this) {
				RebuildIndex();
				IndexWasBuilt = true;
			}
		}

		/// <summary>
		///     Проверяет соответствие имени файла метке
		/// </summary>
		/// <param name="s"></param>
		/// <param name="mask"></param>
		/// <returns></returns>
		protected virtual bool IsMatchMask(string s, string mask) {
			mask = mask.Replace(".", @"\.");
			string regex = ("^" + mask + "$").Replace("*", @"([^\.]+\.~)*").Replace("?", @"[^\.]+").Replace("~","?");
			return Regex.IsMatch(s, regex);
		}

		/// <summary>
		///     Перегружает класс с диска
		/// </summary>
		/// <param name="descriptor"></param>
		protected virtual void ReloadClass(BSharpRuntimeClassDescriptor descriptor) {}

		/// <summary>
		///     Проверяет актуальность класса
		/// </summary>
		/// <param name="descriptor"></param>
		/// <returns></returns>
		protected virtual bool IsActual(BSharpRuntimeClassDescriptor descriptor) {
			return true;
		}

		/// <summary>
		///     Обновляет индекс дескрипторов
		/// </summary>
		protected virtual void RebuildIndex() {}

	    /// <summary>
	    /// Осуществляет поиск класса по пространству имен и/или прототипу
	    /// </summary>
	    /// <param name="ns"></param>
	    /// <param name="prototype"></param>
	    /// <returns></returns>
	    public IEnumerable<IBSharpRuntimeClass> FindClasses(string ns = null,string prototype = null) {
	        lock (this) {
	            CheckIndex();
	            foreach (var descriptor in Cache.Values) {
	                if (null == descriptor.CachedClass || null == descriptor.CachedClass.PrototypeCode ||
	                    null == descriptor.CachedClass.Namespace) {
	                    ReloadClass(descriptor);
	                }
	                if (!string.IsNullOrWhiteSpace(ns)) {
	                    if (descriptor.CachedClass.Namespace != ns) continue;
	                }
	                if (!string.IsNullOrWhiteSpace(prototype)) {
	                    if (descriptor.CachedClass.PrototypeCode != prototype) continue;
	                }
	                if (!descriptor.CachedClass.Loaded) {
	                    ReloadClass(descriptor);
	                }
	                yield return descriptor.CachedClass;
	            }
	        }
	    }
	}
}