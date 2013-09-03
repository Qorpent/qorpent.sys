using System;
using System.Collections.Generic;

namespace Qorpent.Json {


	/// <summary>
	/// Лексер
	/// </summary>
	public class Lexer {

	
		private JsonItem rootitem = null;
		private JsonItem currentitem = null;
		private JsonTuple currenttuple = null;
		


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
				// обработка открытия,закрытия класса
				case JsonTokenType.BeginObject:
					ProcessOpenClass(t);
					break;
				case JsonTokenType.CloseObject:
					ProcessCloseClass(t);
					break;

				// обработка открытия,закрытия массивов
				case JsonTokenType.OpenArray:
					ProcessOpenArray(t);
					break;
				case JsonTokenType.CloseArray:
					ProcessCloseArray(t);
					break;

				//обработка двоеточий
				case JsonTokenType.Colon:
					ProcessColon();
					break;

				case JsonTokenType.Comma:
					ProcessComma();
					break;

				//  обработка значений,которые могут встречаться и в именах и значениях
				case JsonTokenType.Number:
				case JsonTokenType.Literal:
				case JsonTokenType.String:
					ProcessValue(t);
					break;

				// обработка значений которые могут быть только значениями
				case JsonTokenType.Bool:
				case JsonTokenType.Null:
					ProcessSystemLiteral(t);
					break;
				
				default:
					throw new NotSupportedException(t.Type.ToString());
			}
		}

		private void ProcessCloseArray(JsonToken jsonToken) {
			var obj = currentitem as JsonArray;
			if (null == obj) throw new Exception("cannot close array here");
			currentitem = obj.Parent;
			if (null != currentitem)
			{
				//пока не было запятой
				currentitem.CanAddItems = false;
			}
		}

		private void ProcessOpenArray(JsonToken jsonToken) {
			if (!CanSetValue()) throw new Exception("cannot open array here");
			var newobj = new JsonArray();

			// вариант с корневым объектом
			if (null == rootitem)
			{
				rootitem = newobj;
				currentitem = newobj;
				return;
			}

			// вариант с значением свойства
			if (null != currenttuple && currenttuple.HasColon)
			{
				newobj.Parent = currentitem;
				currenttuple.Value = newobj;
				(currentitem as JsonObject).Properties.Add(currenttuple);
				currenttuple = null;
				currentitem = newobj;
				return;
			}

			if (currentitem is JsonArray && currentitem.CanAddItems)
			{
				newobj.Parent = currentitem;
				(currentitem as JsonArray).Values.Add(newobj);
				currentitem = newobj;
				return;
			}
			throw new Exception("cannot open object here");
		}

		private void ProcessComma() {
			if (null == currentitem) throw new Exception("cannot add comma - not object");
			if (null != currenttuple) throw new Exception("cannot add comma - in property");
			if (currentitem is JsonValue)throw new Exception("cannot add comma - require non-literal object");
			if (currentitem.CanAddItems) throw new Exception("cannot add comma - already added");
			currentitem.CanAddItems = true;
		}

		private void ProcessColon() {
			if (null == currenttuple) throw new Exception("cannot process colon here");
			currenttuple.HasColon = true;
		}

		private void ProcessCloseClass(JsonToken jsonToken) {
			var obj = currentitem as JsonObject;
			if (null == obj) throw new Exception("cannot close object here");
			if (null != currenttuple) throw new Exception("cannot object class here - not closed property");
			currentitem = obj.Parent;
			if (null != currentitem) {
				//пока не было запятой
				currentitem.CanAddItems = false;
			}
		}

		private void ProcessOpenClass(JsonToken jsonToken) {
			if (!CanSetValue()) throw new Exception("cannot open object here");
			var newobj = new JsonObject();

			// вариант с корневым объектом
			if (null == rootitem) {
				rootitem = newobj;
				currentitem = newobj;
				return;
			}

			// вариант с значением свойства
			if (null != currenttuple && currenttuple.HasColon) {
				newobj.Parent = currentitem;
				currenttuple.Value = newobj;
				(currentitem as JsonObject).Properties.Add(currenttuple);
				currenttuple = null;
				currentitem = newobj;
				return;
			}
			
			if (currentitem is JsonArray && currentitem.CanAddItems) {
				newobj.Parent = currentitem;
				(currentitem as JsonArray).Values.Add(newobj);
				currentitem = newobj;
				return;
			}
			throw new Exception("cannot open object here");
		}

		private void ProcessSystemLiteral(JsonToken t) {
			if (CanSetValue()) {
				CollectValue(t);
				return;
			}
			throw new Exception("illegal for " + t);
		}

		private void ProcessValue(JsonToken t) {
			if (CanSetValue()) {
				CollectValue(t);
				return;
			}
			if (CanSetName()) {
				CollectName(t);
				return;
			}
			throw new Exception("illegal for val");
		}

		private void CollectName(JsonToken jsonToken) {
			if ( currentitem is JsonObject && null == currenttuple) {
				currenttuple = new JsonTuple();
				currenttuple.Name = new JsonValue(jsonToken.Type, jsonToken.Value);
				return;
			}
			throw new Exception("insuficient place of name");
		}

		private bool CanSetName() {
			if (null == currentitem) return false;
			if (!(currentitem is JsonObject)) return false;
			if (!currentitem.CanAddItems) return false;
			//если свойство уже в агенде, значит имя установлено
			if (null != currenttuple) return false;
			return true;
		}

		private bool CanSetValue() {
			//Вариант автономного значения
			if (null == currentitem) return true;
			//Вариант значения свойства
			if (null != currenttuple && null == currenttuple.Value && null != currenttuple.Name) return true;
			//Вариант массива
			if (currentitem is JsonArray) return currentitem.CanAddItems;
			return false;
		}

		private void CollectValue(JsonToken jtoken) {
			var item = new JsonValue(jtoken.Type, jtoken.Value);
			//вариант с автономным значением
			if (null == rootitem) {
				rootitem = item;
				currentitem = item;
				return;
			}

			//вариант выставления свойства значению
			if (null != currenttuple) {
				currenttuple.Value = item;
				(currentitem as JsonObject).Properties.Add(currenttuple);
				currenttuple = null;
				//пока еще не было запятой - нельзя новые значения
				currentitem.CanAddItems = false;
				return;
			}

			//вариант с массивом
			if (currentitem is JsonArray) {
				(currentitem as JsonArray).Values.Add(item);
				//пока нет запятых - нельзя новые значения
				currentitem.CanAddItems = false;
				return;
			}

			throw new Exception("insuficient place of value");

		}

		
	}
}