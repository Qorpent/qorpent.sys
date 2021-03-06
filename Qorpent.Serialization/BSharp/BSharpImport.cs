﻿using System.Xml.Linq;
using Qorpent.LogicalExpressions;
using Qorpent.Serialization;
using Qorpent.Utils.LogicalExpressions;

namespace Qorpent.BSharp{
	/// <summary>
	/// </summary>
	[Serialize]
	public class BSharpImport : IBSharpImport{
		private static readonly LogicalExpressionEvaluator logical = new LogicalExpressionEvaluator();

		/// <summary>
		/// </summary>
		[SerializeNotNullOnly]
		public string Alias { get; set; }

		/// <summary>
		/// </summary>
		[IgnoreSerialize]
		public IBSharpClass Target { get; set; }

		/// <summary>
		///     Тип импорта
		/// </summary>
		[SerializeNotNullOnly]
		public string Condition { get; set; }

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
		/// </summary>
		[SerializeNotNullOnly]
		public XElement Source { get; set; }

		/// <summary>
		///     Проверяет условные импорты
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		public bool Match(IScope config){
			if (string.IsNullOrWhiteSpace(Condition)) return true;
			if (null == config) return true;
			var src = new DictionaryTermSource<object>(config);
			return logical.Eval(Condition, src);
		}
	}
}