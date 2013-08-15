using Qorpent.Config;
using Qorpent.LogicalExpressions;
using Qorpent.Utils.LogicalExpressions;

namespace Qorpent.BSharp {
	/// <summary>
	/// </summary>
	public class BSharpImport : IBSharpImport {
		static readonly LogicalExpressionEvaluator logical = new LogicalExpressionEvaluator();
		private string _condition;

		/// <summary>
		/// </summary>
		public IBSharpClass Target { get; set; }

		/// <summary>
		///     Тип импорта
		/// </summary>
		public string Condition {
			get { return _condition; }
			set {
				_condition = value;
				
			}
		}

		/// <summary>
		///     Код цели
		/// </summary>
		public string TargetCode { get; set; }

		/// <summary>
		///     Признак неразрешенного импорта
		/// </summary>
		public bool Orphaned { get; set; }
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