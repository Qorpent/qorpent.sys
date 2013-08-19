﻿using System;

namespace Qorpent.BSharp.Runtime {
	/// <summary>
	/// Базовый класс инициализации классов BSharp
	/// </summary>
	public class BSharpRuntimeActivatorService :ServiceBase, IBSharpRuntimeActivatorService {
		/// <summary>
		/// Индекс активатора BSharp по умолчанию
		/// </summary>
		public const int DEFAULT_ACTIVATOR_INDEX = 100;

		/// <summary></summary>
		public BSharpRuntimeActivatorService() {
			Index = DEFAULT_ACTIVATOR_INDEX;
		}

		/// <summary>
		///     Создать типизированный объект из динамического объекта BSharp
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T Activate<T>(IBSharpRuntimeClass rtcls, BSharpActivationType activationType = BSharpActivationType.Auto) where T : class {
			if (activationType == BSharpActivationType.Auto) {
				activationType = DetectActivationtType<T>(rtcls);
			}
			if (!CanActivate<T>(rtcls, activationType)) {
				return default(T);
			}
			switch (activationType) {
				case BSharpActivationType.Client:
					return ActivateClientType<T>(rtcls);
				case BSharpActivationType.Configured:
					return ActivateConfiguredType<T>(rtcls);
			}
			return default(T);
		}

		private T ActivateConfiguredType<T>(IBSharpRuntimeClass rtcls) where T:class {
			var instance = (T)rtcls.RuntimeDescriptor.Create();
			BindRuntimeClass(rtcls, instance);
			return instance;
		}

		private static void BindRuntimeClass<T>(IBSharpRuntimeClass rtcls, T instance) where T : class {
			var bound = instance as IBSharpRuntimeBound;
			if (null != bound) {
				bound.Initialize(rtcls);
			}
		}

		private T ActivateClientType<T>(IBSharpRuntimeClass rtcls)  where T:class {
			var instance = typeof (T).IsInterface ? ResolveService<T>() : Activator.CreateInstance<T>();
			BindRuntimeClass(rtcls, instance);
			return instance;
		}

		/// <summary>
		/// Определяет реальный тип инстанцирования
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="rtcls"></param>
		/// <returns></returns>
		protected virtual BSharpActivationType DetectActivationtType<T>(IBSharpRuntimeClass rtcls) {
			if (null == rtcls.RuntimeDescriptor) return BSharpActivationType.Client;
			if (
				RuntimeClassResolutionType.NotDefined == rtcls.RuntimeDescriptor.ResolutionType
				||
				RuntimeClassResolutionType.NotResolved == rtcls.RuntimeDescriptor.ResolutionType
			) {
				return BSharpActivationType.Client;
			}
			if (typeof (object) == typeof (T)) {
				return BSharpActivationType.Configured;
			}
			var resolvedType = rtcls.RuntimeDescriptor.GetActualType();

			if(null==resolvedType)return BSharpActivationType.Client;

			if(typeof(T).IsAssignableFrom(resolvedType)) return BSharpActivationType.Configured;

			return BSharpActivationType.Client;
		}

		/// <summary>
		///     Порядковый номер при обходе
		/// </summary>
		public int Index { get; set; }

		/// <summary>
		///     Проверяет поддержку сериализации
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public bool CanActivate<T>(IBSharpRuntimeClass rtcls, BSharpActivationType activationType = BSharpActivationType.Auto) where T : class {
			if (BSharpActivationType.Auto == activationType) {
				activationType = DetectActivationtType<T>(rtcls);
			}
			if (BSharpActivationType.Client == activationType) {
				return GetCanActivateClient<T>();
			}
			if (BSharpActivationType.Configured == activationType) {
				return GetCanActivateConfigured<T>(rtcls);
			}
			throw new Exception("invalid activation type " + activationType);
		}

		private bool GetCanActivateClient<T>() {
			if (typeof (T).IsInterface) {
				var sc = Container.FindComponent(typeof (T), null);
				if (null == sc) return false;
				return typeof (IBSharpRuntimeBound).IsAssignableFrom(sc.ImplementationType);
			}
			return typeof (IBSharpRuntimeBound).IsAssignableFrom(typeof (T));
		}

		private static bool GetCanActivateConfigured<T>(IBSharpRuntimeClass rtcls) {
			if (null == rtcls.RuntimeDescriptor) return false;
			var resolvedType = rtcls.RuntimeDescriptor.GetActualType();
			if (null == resolvedType) return false;
			return
				typeof (T).IsAssignableFrom(resolvedType)
				&&
				typeof (IBSharpRuntimeBound).IsAssignableFrom(resolvedType)
				;
		}
	}
}