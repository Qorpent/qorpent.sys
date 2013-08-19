using System;
using Qorpent.IoC;

namespace Qorpent.BSharp.Runtime {
	/// <summary>
	/// Описатель класса рантайм
	/// </summary>
	public class RuntimeClassDescriptor {
		private IContainer _container;
		/// <summary>
		/// Создает целевой объект
		/// </summary>
		/// <param name="typename"></param>
		/// <param name="container"></param>
		public RuntimeClassDescriptor(string typename,IContainer container) {
			SourceClassName = typename;
			_container = container;
			Initialize();
		}

		private void Initialize() {
			if (string.IsNullOrWhiteSpace(SourceClassName)) {
				ResolutionType = RuntimeClassResolutionType.NotDefined;
			}
			else if (SourceClassName.Contains(",")) {
				InitializeWithDirectClassName();
			}else {
				InitializeFromContainer();
			}
		}

		private void InitializeFromContainer() {
			ResolutionType = RuntimeClassResolutionType.ContainerName;
		}

		private void InitializeWithDirectClassName() {
			ResolutionType = RuntimeClassResolutionType.Resolved;
			var type = ResolvedType =  Type.GetType(SourceClassName,false);
			if (null == type) {
				ResolutionType = RuntimeClassResolutionType.NotResolved;
			}else if (type.IsInterface) {
				ResolutionType = RuntimeClassResolutionType.ContainerService;
			}
		}

		/// <summary>
		/// Тип резолюции класса
		/// </summary>
		public RuntimeClassResolutionType ResolutionType { get; set; }

		/// <summary>
		/// Исходное имя типа
		/// </summary>
		public string SourceClassName { get; set; }

		/// <summary>
		/// Разрешенный тип
		/// </summary>
		public Type ResolvedType { get; set; }
	

		/// <summary>
		/// Возвращает текущую полную резолюцию типа
		/// </summary>
		/// <returns></returns>
		public Type GetActualType() {
			switch (ResolutionType) {
				case RuntimeClassResolutionType.NotResolved:
				case RuntimeClassResolutionType.NotDefined:
					return null;
				case RuntimeClassResolutionType.Resolved:
					return ResolvedType;
				case RuntimeClassResolutionType.ContainerService:
					var stc = _container.FindComponent(ResolvedType, null);
					return stc==null?null:stc.ImplementationType;
				case RuntimeClassResolutionType.ContainerName:
					var ntc = _container.FindComponent(null,SourceClassName);
					return ntc==null?null:ntc.ImplementationType;
				default :
					throw new Exception("unknown resolution type " + ResolutionType);
			}
		}
		/// <summary>
		/// Возвращает рантайм-класс (просто дефолтное создаение)
		/// </summary>
		/// <returns></returns>
		public object Create() {
			switch (ResolutionType)
			{
				case RuntimeClassResolutionType.NotResolved:
				case RuntimeClassResolutionType.NotDefined:
					return null;
				case RuntimeClassResolutionType.Resolved:
					return Activator.CreateInstance(ResolvedType);
				case RuntimeClassResolutionType.ContainerService:
					return _container.Get(ResolvedType);
				case RuntimeClassResolutionType.ContainerName:
					return _container.Get(null,SourceClassName);
				default:
					throw new Exception("unknown resolution type " + ResolutionType);
			}
		}

	}
}