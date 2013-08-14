using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Qorpent.BSharp {
	/// <summary>
	///     Абстракция компилятора, опирается на некий набор первичных компиляторов
	/// </summary>
	public abstract class BSharpCompilerBase : IBSharpCompiler {
		private IBSharpConfig _config;

		/// <summary>
		///     Возвращает конфигурацию компилятора
		/// </summary>
		/// <returns></returns>
		public IBSharpConfig GetConfig() {
			if (null == _config) {
				_config = new BSharpConfig();
			}
			return _config;
		}

		/// <summary>
		/// </summary>
		/// <param name="compilerConfig"></param>
		public void Initialize(IBSharpConfig compilerConfig) {
			_config = compilerConfig;
		}


		/// <summary>
		///     Компилирует источники в перечисление итоговых классов
		/// </summary>
		/// <param name="sources"></param>
		/// <param name="preparedContext"></param>
		/// <returns></returns>
		public IBSharpContext Compile(IEnumerable<XElement> sources , IBSharpContext preparedContext = null) {
			var cfg = GetConfig();
			if (cfg.SingleSource) {
				return BuildBatch(sources);
			}
			IBSharpContext result = new BSharpContext();
			foreach (XElement src in sources) {
				IBSharpContext subresult = BuildSingle(src);
				result.Merge(subresult);
			}
			return result;
		}

		private IBSharpContext BuildSingle(XElement source) {
			var batch = new[] {source};
			IBSharpContext context = BuildIndex(batch);
			Link(batch, context);
			return context;
		}

		/// <summary>
		///     Перекрыть для создания индексатора
		/// </summary>
		/// <param name="sources"></param>
		/// <returns></returns>
		protected abstract IBSharpContext BuildIndex(IEnumerable<XElement> sources);

		private IBSharpContext BuildBatch(IEnumerable<XElement> sources) {
			XElement[] batch = sources.ToArray();
			var context = BuildIndex(batch);
			Link(batch, context);
			return context;
		}

		/// <summary>
		///     Перекрыть для создания линковщика
		/// </summary>
		/// <param name="sources"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		protected abstract void Link(IEnumerable<XElement> sources, IBSharpContext context);
	}
}