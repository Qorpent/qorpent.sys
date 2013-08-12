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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="compilerConfig"></param>
		public void Initialize(IObjectXmlCompilerConfig compilerConfig) {
			_config = compilerConfig;
		}


		/// <summary>
		/// Компилирует источники в перечисление итоговых классов 
		/// </summary>
		/// <param name="sources"></param>
		/// <returns></returns>
		public ObjectXmlCompilerIndex Compile(IEnumerable<XElement> sources) {
			var cfg = GetConfig();
			if (cfg.SingleSource) {
				return BuildBatch(sources);
			}
			var result = new ObjectXmlCompilerIndex();
			foreach (var src in sources) {
				var subresult = BuildSingle(src);
				result.Merge(subresult);
			}
			return result;
		}

		private ObjectXmlCompilerIndex BuildSingle(XElement source) {
			var batch = new[] {source};
			var index = BuildIndex(batch);
			Link(batch, index);
			return index;
		}
		/// <summary>
		/// Перекрыть для создания индексатора
		/// </summary>
		/// <param name="sources"></param>
		/// <returns></returns>
		protected abstract ObjectXmlCompilerIndex BuildIndex(IEnumerable<XElement> sources);
			
		private ObjectXmlCompilerIndex BuildBatch(IEnumerable<XElement> sources) {
			var batch = sources.ToArray();
			var index = BuildIndex(batch);
			Link(batch, index);
			return index;
		}
		/// <summary>
		/// Перекрыть для создания линковщика
		/// </summary>
		/// <param name="sources"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		protected abstract void Link(IEnumerable<XElement> sources, ObjectXmlCompilerIndex index);
	}
}