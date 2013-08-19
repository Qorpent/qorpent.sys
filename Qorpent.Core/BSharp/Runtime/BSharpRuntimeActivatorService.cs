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
		public T Activate<T>(IBSharpRuntimeClass rtcls, BSharpActivationType activationType = BSharpActivationType.Auto) {
			if (activationType == BSharpActivationType.Auto) {
				activationType = DetectActivationtType<T>(rtcls);
			}
			switch (activationType) {
				case BSharpActivationType.Client:
					return ActivateClientType<T>(rtcls);
				case BSharpActivationType.Configured:
					return ActivateConfiguredType<T>(rtcls);
			}
			return default(T);
		}

		private T ActivateConfiguredType<T>(IBSharpRuntimeClass rtcls) {
			throw new System.NotImplementedException();
		}

		private T ActivateClientType<T>(IBSharpRuntimeClass rtcls) {
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Определяет реальный тип инстанцирования
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="rtcls"></param>
		/// <returns></returns>
		protected virtual BSharpActivationType DetectActivationtType<T>(IBSharpRuntimeClass rtcls) {
			throw new System.NotImplementedException();
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
		public bool CanActivate<T>(IBSharpRuntimeClass rtcls, BSharpActivationType acivationType = BSharpActivationType.Auto) {
			throw new System.NotImplementedException();
		}
	}
}