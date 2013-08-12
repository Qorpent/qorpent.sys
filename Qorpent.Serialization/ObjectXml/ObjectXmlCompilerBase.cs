using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Qorpent.ObjectXml {
	/// <summary>
	/// Абстракция компилятора, опирается на некий набор первичных компиляторов
	/// </summary>
	public abstract class ObjectXmlCompilerBase : IObjectXmlCompiler {
		private IObjectXmlCompilerConfig _config;
		/// <summary>
		/// Возвращает конфигурацию компилятора
		/// </summary>
		/// <returns></returns>
		public IObjectXmlCompilerConfig GetConfig() {
			if (null == _config) {
				_config = new ObjectXmlCompilerConfig();
			}
			return _config;
		}
		public void Initialize(IObjectXmlCompilerConfig compilerConfig) {
			_config = compilerConfig;
		}


		/// <summary>
		/// Компилирует источники в перечисление итоговых классов 
		/// </summary>
		/// <param name="sources"></param>
		/// <returns></returns>
		public IEnumerable<XElement> Compile(IEnumerable<XElement> sources) {
			var cfg = GetConfig();
			if (cfg.SingleSource) {
				return BuildBatch(sources);
			}
			return sources.SelectMany(BuildSingle);
		}

		private IEnumerable<XElement> BuildSingle(XElement source) {
			var batch = new[] {source};
			var index = BuildIndex(batch);
			return Link(batch, index);
		}
		/// <summary>
		/// Перекрыть для создания индексатора
		/// </summary>
		/// <param name="sources"></param>
		/// <returns></returns>
		protected abstract ObjectXmlCompilerIndex BuildIndex(IEnumerable<XElement> sources);
			
		private IEnumerable<XElement> BuildBatch(IEnumerable<XElement> sources) {
			var batch = sources.ToArray();
			var index = BuildIndex(batch);
			return Link(batch, index);
		}
		/// <summary>
		/// Перекрыть для создания линковщика
		/// </summary>
		/// <param name="sources"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		protected abstract IEnumerable<XElement> Link(IEnumerable<XElement> sources, ObjectXmlCompilerIndex index);
	}
}