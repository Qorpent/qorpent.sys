using System.Collections.Generic;
using Qorpent.Config;
using Qorpent.Data;

namespace Qorpent.BSharp {
	/// <summary>
	/// Интерфейс результирующего контекста BSharp
	/// </summary>
	public interface IBSharpContext: IXmlConfigSource, IContext {
		/// <summary>
		/// Загружает исходные определения классов
		/// </summary>
		/// <param name="rawclasses"></param>
		void Setup(IEnumerable<IBSharpClass> rawclasses);

		/// <summary>
		///     Присоединяет и склеивается с другим результатом
		/// </summary>
		/// <param name="othercontext"></param>
		void Merge(IBSharpContext othercontext);

		/// <summary>
		/// Разрешает класс по коду и заявленному пространству имен
		/// </summary>
		/// <param name="code"></param>
		/// <param name="ns"></param>
		/// <param name="usemeta"></param>
		/// <returns></returns>
		IBSharpClass Get(string code, string ns = null, bool usemeta = false);

		/// <summary>
		/// Возвращает коллекцию классов по типу классов
		/// </summary>
		/// <param name="datatype"></param>
		/// <returns></returns>
		IEnumerable<IBSharpClass> Get(BSharpContextDataType datatype);

		/// <summary>
		/// Возвращает ошибки указанного уровня
		/// </summary>
		/// <param name="level"></param>
		/// <returns></returns>
		IEnumerable<BSharpError> GetErrors(ErrorLevel level = ErrorLevel.None);

		/// <summary>
		/// Строит рабочий индекс классов
		/// </summary>
		void Build();
		/// <summary>
		/// Регистрирует ошибку в контексте
		/// </summary>
		/// <param name="error"></param>
		void RegisterError(BSharpError error);

		/// <summary>
		/// Реестр перезагрузок классов
		/// </summary>
		List<IBSharpClass> Ignored { get; set; }

		/// <summary>
		/// Компилятор
		/// </summary>
		IBSharpCompiler Compiler { get; set; }

		/// <summary>
		/// 
		/// </summary>
		IDictionary<string, object> ExtendedData { get; }

		/// <summary>
		///    Определения  для псеавдоклассов
		/// </summary>
		IDictionary<string, IBSharpClass> MetaClasses { get; }

		/// <summary>
		///     Исходные сырые определения классов
		/// </summary>
		IDictionary<string, IBSharpClass> RawClasses { get; }


		/// <summary>
		/// Очищает данные по задачам
		/// </summary>
		void ClearBuildTasks();

		/// <summary>
		/// Строит индекс словарей
		/// </summary>
		void BuildLinkingIndex();

		/// <summary>
		/// Разрешает элементы в словаре
		/// </summary>
		/// <returns></returns>
		string ResolveDictionary(string code, string element);

		/// <summary>
		/// Определяет наличие словаря
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		bool HasDictionary(string code);

		/// <summary>
		/// Проверяет необходимость линковки контекста в целом
		/// </summary>
		/// <returns></returns>
		bool RequireLinking();

		/// <summary>
		/// Специальная индексация для модификатора all
		/// </summary>
		/// <param name="query"></param>
		/// <param name="basens"></param>
		/// <param name="usemeta"></param>
		/// <returns></returns>
		IEnumerable<IBSharpClass> ResolveAll(string query, string basens = null, bool usemeta = false);
		/// <summary>
		/// Выполняет генераторы, формируя дополнительные классы
		/// </summary>
		void ExecuteGenerators();

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		bool RequirePatching();
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		bool RequrePostProcess();

		/// <summary>
		/// Обертка для поиска классов
		/// </summary>
		/// <returns></returns>
		IBSharpClass this[string name, string ns = null] { get; }
	}
}