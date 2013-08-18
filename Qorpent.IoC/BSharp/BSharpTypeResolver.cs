using System;
using System.Collections;
using System.Collections.Generic;
using Qorpent.BSharp.Runtime;

namespace Qorpent.IoC.BSharp
{
	/// <summary>
	///  Специальный резольвер BSharp классов для общей IoC инфраструктуры
	/// </summary>
	public class BSharpTypeResolver:ITypeResolver
	{
		private IContainer _container;
		private IBSharpRuntimeService _bsharpRuntimeService;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="parentContainer"></param>
		public BSharpTypeResolver(IContainer parentContainer) {
			this._container = parentContainer;
			this._bsharpRuntimeService = _container.Get<IBSharpRuntimeService>();
			//это резольвер-перехватчик
			this.Idx = _container.Idx - 10;
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
			if (null != name && name.StartsWith("bsrt:"))
			{
				throw new NotImplementedException();
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
			if (null != name && name.StartsWith("bsrt:"))
			{
				throw new NotImplementedException();
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
			if (null!=name && name.StartsWith("bsrt:")) {
				throw new NotImplementedException();
			}
			else {
				yield break;
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
			if (null != name && name.StartsWith("bsrt:"))
			{
				throw new NotImplementedException();
			}
			else
			{
				yield break;
			}
		}

		/// <summary>
		/// Не поддерживается для BSharpRT
		/// </summary>
		/// <param name="obj"></param>
		public void Release(object obj) {
			
		}
	}
}
