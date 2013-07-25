using System;
using System.Collections.Generic;

namespace Qorpent.Dsl.Json {


	/// <summary>
	/// Лексер
	/// </summary>
	public class Lexer {

	
		private JsonItem rootitem = null;
		private JsonItem currentitem = null;
		


		/// <summary>
		/// Скомпановать объект из токенов
		/// </summary>
		/// <param name="tokens"></param>
		/// <returns></returns>
		public JsonItem Collect(IEnumerable<JsonToken> tokens) {
			foreach (var t in tokens) {
				Collect(t);
			}
			return rootitem;
		}

		private void Collect(JsonToken t) {
			switch (t.Type) {

				case TType.Num:
					goto case TType.Str;
				case TType.Lit:
					goto case TType.Str;				
				case TType.Bool:
					if (CanSetValue()) {
						CollectValue(t);
						break;
					}
					throw new Exception("illegal for bool");
				case TType.Str:
					if (CanSetValue()) {
						CollectValue(t);
						break;
					}
					if (CanSetName()) {
						CollectName(t);
						break;
					}
					throw new Exception("illegal for val");
				default:
					throw new NotSupportedException(t.Type.ToString());
			}
		}

		private void CollectName(JsonToken jsonToken) {
			throw new NotImplementedException();
		}

		private bool CanSetName() {
			throw new NotImplementedException();
		}

		private bool CanSetValue() {
			if (null == currentitem) return true;

			return false;
		}

		private void CollectValue(JsonToken jtoken) {
			var item = new JsonValue(jtoken.Type, jtoken.Value);
			if (null == rootitem) {
				rootitem = item;
			}
			currentitem = item;
		}

		
	}
}