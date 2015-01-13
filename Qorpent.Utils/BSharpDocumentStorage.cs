using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Bxl;
using Qorpent.Data;

namespace Qorpent.Utils {
	/// <summary>
	///		Хранилище документов на базе B#
	/// </summary>
	public class BSharpDocumentStorage : IDocumentStorage {
		/// <summary>
		///		Атрибут, указывающий на признак комплексного сета
		/// </summary>
		public const string IsComplexResultAttribute = "__isComplexResult";
		/// <summary>
		///		Атрибут, указываюший на признак пустого сета
		/// </summary>
		public const string IsEmptyResultAttribute = "__isEmptyResult";
		/// <summary>
		///		Имя обрётки, используемое по умолчанию
		/// </summary>
		public const string DefaultWrapperName = "document";
		private IBSharpContext _bSharpContext;
		private IBSharpCompiler _bSharpCompiler;
		private IBxlParser _bxlParser;
		private string _wrapperName;
		/// <summary>
		///		Используемый компилятор B#
		/// </summary>
		public IBSharpCompiler BSharpCompiler {
			get { return _bSharpCompiler ?? (_bSharpCompiler = WellKnownHelper.Create<IBSharpCompiler>()); }
			set { _bSharpCompiler = value; }
		}
		/// <summary>
		///		Используемый парсер BXL
		/// </summary>
		public IBxlParser BxlParser {
			get { return _bxlParser ?? (_bxlParser = WellKnownHelper.Create<IBxlParser>()); }
			set { _bxlParser = value; }
		}
		/// <summary>
		///		Используемый контекст B#
		/// </summary>
		public IBSharpContext BSharpContext {
			get { return _bSharpContext; }
		}
		/// <summary>
		///		Указывает на то, что результат нужно оборачивать всегда
		/// </summary>
		public bool WrapAlways { get; set; }
		/// <summary>
		///		Имя элемента-обёртки
		/// </summary>
		public string WrapperName {
			get {
				if (string.IsNullOrWhiteSpace(_wrapperName)) {
					_wrapperName = DefaultWrapperName;
				}
				return _wrapperName;
			}
			set { _wrapperName = value; }
		}
		/// <summary>
		///		Определяет текущий контекст
		/// </summary>
		/// <returns>Контекст</returns>
		public IContext GetContext() {
			return BSharpContext;
		}
		/// <summary>
		///		Выполнение запроса
		/// </summary>
		/// <param name="query">Запрос</param>
		/// <param name="options">Параметры запроса</param>
		/// <returns>Результирующий <see cref="XElement"/></returns>
		public XElement ExecuteQuery(string query, DocumentStorageOptions options = null) {
			if (null == _bSharpContext) return null;
			XElement result;
			string ns = null;
			if (options != null) {
				if (!string.IsNullOrWhiteSpace(options.Collection)) {
					ns = options.Collection;
				}
			}
			var selected = _bSharpContext.ResolveAll(query, ns).ToArray();
			if (selected.Length == 0) {
				if (WrapAlways) {
					result = new XElement(WrapperName);
					result.SetAttributeValue(IsEmptyResultAttribute, true);
				} else {
					result = null;
				}
			} else if (selected.Length == 1) {
				if (WrapAlways) {
					result = new XElement(WrapperName);
					result.Add(selected[0].Compiled);
				} else {
					result = selected[0].Compiled;
				}
			} else {
				result = new XElement(WrapperName);
				result.SetAttributeValue(IsComplexResultAttribute, true);
				foreach (var bSharpClass in selected) {
					result.Add(bSharpClass.Compiled);
				}
			}
			return result;
		}
		/// <summary>
		///		Установка контекста работа
		/// </summary>
		/// <param name="context">Контекст</param>
		/// <returns>Замыкание на <see cref="IDocumentStorage"/></returns>
		public IDocumentStorage SetContext(IContext context) {
			var bSharpContext = context as IBSharpContext;
			if (bSharpContext != null) {
				SetContext(bSharpContext);
			}
			throw new Exception("Cannot set undefined context");
		}
		/// <summary>
		///		Установка контекста
		/// </summary>
		/// <param name="database">База данных</param>
		/// <param name="collection">Коллекция</param>
		/// <returns>Замыкание на хранилище документов</returns>
		public IDocumentStorage SetContext(string database, string collection) {
			throw new NotSupportedException("BSharp document storage does not supports SetContext");
		}
		/// <summary>
		///		Установка контекста
		/// </summary>
		/// <param name="context">Контекст</param>
		public void SetContext(IBSharpContext context) {
			_bSharpContext = context;
		}
		/// <summary>
		///		Установка контекста
		/// </summary>
		/// <param name="bSharp">Код на B#</param>
		public void SetContext(string bSharp) {
			SetContext(new[] {bSharp});
		}
		/// <summary>
		///		Установка контекста
		/// </summary>
		/// <param name="bSharps">Массив кода на B#</param>
		public void SetContext(string[] bSharps) {
			var parsed = bSharps.Select(_ => BxlParser.Parse(_));
			var compiled = BSharpCompiler.Compile(parsed);
			SetContext(compiled);
		}

		/// <summary>
		///		Установка контекста <see cref="BSharpDocumentStorage"/> с указанием исходной директории
		/// </summary>
		/// <param name="storage">Исходное хранилище для установки контекста</param>
		/// <param name="path">Путь к файлам B#</param>
		/// <param name="pattern">Паттерн поиска</param>
		/// <param name="recursive">Применять рекурсивную стратегию поиска файлов B#</param>
		/// <param name="resolve">Признак того, что нужно применить резолюцию</param>
		/// <returns>Настроенный экземпляр <see cref="BSharpDocumentStorage"/></returns>
		public static void SetContext(BSharpDocumentStorage storage, string path, string pattern = "*.bxls", bool recursive = true, bool resolve = true) {
			if (resolve) path = EnvironmentInfo.ResolvePath(path);
			if (!Directory.Exists(path)) {
				var opts = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
				var files = Directory.GetFiles(path, pattern, opts).Select(File.ReadAllText).ToArray();
				storage.SetContext(files);
			} else if (File.Exists(path)) {
				var contents = File.ReadAllText(path);
				storage.SetContext(contents);
			} else {
				throw new Exception("Cannot resolve target");
			}
		}
	}
}
