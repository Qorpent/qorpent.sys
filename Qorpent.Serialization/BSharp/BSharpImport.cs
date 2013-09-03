using System.Xml.Linq;
using Qorpent.Config;
using Qorpent.LogicalExpressions;
using Qorpent.Serialization;
using Qorpent.Utils.LogicalExpressions;

namespace Qorpent.BSharp {
	/// <summary>
	/// </summary>
	[Serialize]
	public class BSharpImport : IBSharpImport {
		static readonly LogicalExpressionEvaluator logical = new LogicalExpressionEvaluator();
		private string _condition;

		/// <summary>
		/// </summary>
		[IgnoreSerialize]
		public IBSharpClass Target { get; set; }

		/// <summary>
		///     Тип импорта
		/// </summary>
		[SerializeNotNullOnly]
		public string Condition {
			get { return _condition; }
			set {
				_condition = value;
				
			}
		}

		/// <summary>
		///     Код цели
		/// </summary>
		[SerializeNotNullOnly]
		public string TargetCode { get; set; }

		/// <summary>
		///     Признак неразрешенного импорта
		/// </summary>
		[SerializeNotNullOnly]
		public bool Orphaned { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[SerializeNotNullOnly]
		public XElement Source { get; set; }

		/// <summary>
		/// Проверяет условные импорты
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		public bool Match(IConfig config) {
			if (string.IsNullOrWhiteSpace(Condition)) return true;
			if (null == config) return true;
			var src = new DictionaryTermSource(config);
			return logical.Eval(Condition, src);
		}
	}
}