using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Qorpent.BSharp.Runtime;

namespace Qorpent.IoC.BSharp
{
	/// <summary>
	///  Специальный резольвер BSharp классов для общей IoC инфраструктуры
	/// </summary>
	public class BSharpTypeResolver:ITypeResolver {
		/// <summary>
		/// Префикс имен BSharp-классов
		/// </summary>
		public const string COMPONENT_NAME_PREFIX = "bsharp://";
		private IContainer _container;
		private IBSharpRuntimeService _bsharpRuntimeService;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="parentContainer"></param>
		public BSharpTypeResolver(IContainer parentContainer) {
			_container = parentContainer;
			_bsharpRuntimeService = _container.Get<IBSharpRuntimeService>();
			//это резольвер-дополнение
			Idx = _container.Idx + 100;
		}

		/// <summary>
		/// Приоритет при разрешении типов
		/// </summary>
		public int Idx { get; set; }
		/// <summary>
		/// Работает только для имен на "bstr:" ищет по полному имени
		/// </summary>
		/// <param name="name"></param>
		/// <param name="ctorArguments"></param>
		/// <returns></returns>
		public T Get<T>(string name = null, params object[] ctorArguments) where T : class {
			if (null != name && name.StartsWith(COMPONENT_NAME_PREFIX)) {
				var classname = name.Replace(COMPONENT_NAME_PREFIX, "");
				return _bsharpRuntimeService.Activate<T>(classname);
			}
			return default(T);
		}
		/// <summary>
		/// Работает только для имен на "bstr:" ищет по полному имени
		/// </summary>
		/// <param name="type"></param>
		/// <param name="name"></param>
		/// <param name="ctorArguments"></param>
		/// <returns></returns>
		public object Get(Type type, string name = null, params object[] ctorArguments) {
			if (null != name && name.StartsWith(COMPONENT_NAME_PREFIX))
			{
				var classname = name.Replace(COMPONENT_NAME_PREFIX, "");
				return _bsharpRuntimeService.Activate<object>(classname);
			}
			return null;
		}
		/// <summary>
		/// Работает только для имен на "bstr:" ищет по маске имени
		/// </summary>
		/// <param name="name"></param>
		/// <param name="ctorArguments"></param>
		/// <returns></returns>
		public IEnumerable<T> All<T>(string name = null, params object[] ctorArguments) where T : class {
			if (null != name && name.StartsWith(COMPONENT_NAME_PREFIX))
			{
				var classmask = name.Replace(COMPONENT_NAME_PREFIX, "");
				var result = _bsharpRuntimeService.GetClassNames(classmask)
				                            .Select(_ => _bsharpRuntimeService.Activate<T>(_))
				                            .Where(_ => null != _);
				foreach (var r in result) {
					yield return r;
				}
			}
		}
		/// <summary>
		/// Работает только для имен на "bstr:" ищет по маске имени
		/// </summary>
		/// <param name="type"></param>
		/// <param name="name"></param>
		/// <param name="ctorArguments"></param>
		/// <returns></returns>
		public IEnumerable All(Type type, string name = null, params object[] ctorArguments) {
			if (null != name && name.StartsWith(COMPONENT_NAME_PREFIX))
			{
				var classmask = name.Replace(COMPONENT_NAME_PREFIX, "");
				var result = _bsharpRuntimeService.GetClassNames(classmask)
											.Select(_ => _bsharpRuntimeService.Activate<object>(_))
											.Where(_ => null != _);
				foreach (var r in result)
				{
					yield return r;
				}
			}
		}

		/// <summary>
		/// Не поддерживается для BSharpRT
		/// </summary>
		/// <param name="obj"></param>
		public void Release(object obj) {
			
		}

	    public IEnumerable All2(Type type, string name, params object[] ctorArguments) {
	        return All(type, name, (object[]) ctorArguments);
	    }
	}
}
