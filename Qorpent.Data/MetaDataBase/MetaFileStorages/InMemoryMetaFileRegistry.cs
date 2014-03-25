using System;
using System.Collections.Generic;
using System.Linq;

namespace Qorpent.Data.MetaDataBase{
	/// <summary>
	/// Реализация IMetaFileRegistry для регистрации файлов в памяти
	/// Хранимые значения обладают immutable
	/// </summary>
	public class InMemoryMetaFileRegistry : MetaFileRegistryBase {
		readonly IDictionary<string,IList<MetaFileDescriptor>> _registry = new Dictionary<string, IList<MetaFileDescriptor>>();
		readonly IDictionary<string,MetaFileDescriptor> _active = new Dictionary<string, MetaFileDescriptor>();
	

		void Ensure(string code){
			if (!_registry.ContainsKey(code)){
				_registry[code] = new List<MetaFileDescriptor>();
			}
			if (!_active.ContainsKey(code)){
				_active[code] = null;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="search"></param>
		/// <returns></returns>
		MetaFileDescriptor Find(MetaFileDescriptor search){
			if (!_registry.ContainsKey(search.Code)) return null;
			return _registry[search.Code].FirstOrDefault(_ => _.IsMatchRevision(search));
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="descriptor"></param>
		/// <returns></returns>
		public override MetaFileDescriptor GetByRevision(MetaFileDescriptor descriptor){
			return Find(descriptor);
		}

		/// <summary>
		/// Прочитать набор кодов
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<string> GetCodes(string prefix = null){
			return _registry.Keys.Where(_ => string.IsNullOrWhiteSpace(prefix) || _.StartsWith(prefix));
		}

		/// <summary>
		/// Проверяет наличие файла с кодом
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public override bool Exists(string code){
			return _registry.ContainsKey(code);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="savedescriptor"></param>
		protected override void InternalRegister(MetaFileDescriptor savedescriptor){
			var existed = Find(savedescriptor);
			if (null != existed){
				return;
			}

			Ensure(savedescriptor.Code);

			_registry[savedescriptor.Code].Add(savedescriptor);
			if (null == _active[savedescriptor.Code]){
				_active[savedescriptor.Code] = savedescriptor;
			}
		}

		/// <summary>
		/// Устанавливает указанную ревизию в качестве текущей (не требует контента)
		/// </summary>
		/// <param name="descriptor"></param>
		public override MetaFileDescriptor Checkout(MetaFileDescriptor descriptor){
			Ensure(descriptor.Code);
			var revisionToActivate = Find(descriptor);
			if (null == revisionToActivate){
				if (descriptor.IsFullyDefined()){
					revisionToActivate = Register(descriptor);

				}
				else{
					throw new Exception("cannot find revision for "+descriptor);
				}
			}
			_active[revisionToActivate.Code] = revisionToActivate;
			return revisionToActivate.Copy();
		}

		/// <summary>
		/// Получить ревизии файла по коду
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public override IEnumerable<RevisionDescriptor> GetRevisions(string code){
			if (_registry.ContainsKey(code)){
				foreach (var i in _registry[code].OrderByDescending(_=>_.RevisionTime)){
					yield return i.Copy();
				}
			}
		}

		/// <summary>
		/// Возвращает текущий файл по коду
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public override MetaFileDescriptor GetCurrent(string code){
			if (_active.ContainsKey(code)) return _active[code].Copy();
			throw new Exception("cannot find file with such code "+code);
		}
	}
}