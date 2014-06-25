using System;
using System.Collections.Generic;
using System.Text;
using Qorpent.IoC;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.BSharp{
	/// <summary>
	///     Построитель кода на B#
	/// </summary>
	[ContainerComponent(ServiceType = typeof (IBSharpCodeBuilder))]
	public class BSharpCodeBuilder : IBSharpCodeBuilder{
		private readonly StringBuilder _buffer = new StringBuilder();

		private int _level;
		private int lastAppendLine = -1;

		/// <summary>
		///     Уровень вложенности элементов
		/// </summary>
		public int Level{
			get { return _level; }
		}

		/// <summary>
		///     Увеличить отступ
		/// </summary>
		public void Indent(){
			_level++;
		}

		/// <summary>
		///     Уменьшить отступ
		/// </summary>
		public void Dedent(){
			if (_level > 0){
				_level -= 1;
			}
		}

		/// <summary>
		///     Прописывает отступ по уровню
		/// </summary>
		public void WriteLevel(){
			for (int i = 0; i < Level; i++){
				_buffer.Append('\t');
			}
		}

		/// <summary>
		///     Прописывает блок комментариев
		/// </summary>
		/// <param name="title"></param>
		/// <param name="commentSource"></param>
		public void WriteCommentBlock(string title, object commentSource = null){
			if (null == commentSource){
				return;
			}
			WriteCommentLine();
			for (int i = 0; i < 4; i++){
				_buffer.Append('#');
			}
			int delt = 120 - 8 - title.Length;
			var start = (int) Math.Floor(delt/2.0);
			var end = (int) Math.Ceiling(delt/2.0);
			for (int i = 0; i < start; i++){
				_buffer.Append(' ');
			}
			_buffer.Append(title);
			for (int i = 0; i < end; i++){
				_buffer.Append(' ');
			}
			for (int i = 0; i < 4; i++){
				_buffer.Append('#');
			}
			EnsureAppendLine();
			IDictionary<string, object> dict = commentSource.ToDict();
			foreach (var p in dict){
				for (int i = 0; i < 4; i++){
					_buffer.Append('#');
				}
				for (int i = 0; i < 20; i++){
					_buffer.Append(' ');
				}
				string key = p.Key;
				_buffer.Append(key);
				for (int i = 0; i < 30 - key.Length; i++){
					_buffer.Append(' ');
				}
				_buffer.Append(" : ");
				string val = p.Value.ToString();
				_buffer.Append(val);
				for (int i = 0; i < 39 - val.Length; i++){
					_buffer.Append(' ');
				}
				for (int i = 0; i < 20; i++){
					_buffer.Append(' ');
				}
				for (int i = 0; i < 4; i++){
					_buffer.Append('#');
				}
				EnsureAppendLine();
			}

			WriteCommentLine();
		}

		/// <summary>
		///     Формирует строку комментариев
		/// </summary>
		public void WriteCommentLine(){
			for (int i = 0; i < 120; i++){
				_buffer.Append('#');
			}
			EnsureAppendLine();
		}

		/// <summary>
		///     Начинает блок пространства имен
		/// </summary>
		/// <param name="ns"></param>
		/// <param name="attributes"></param>
		public void StartNamespace(string ns, object attributes = null){
			StartElement(BSharpSyntax.Namespace, ns, null, null, attributes);
		}

		/// <summary>
		///     Начать элемент
		/// </summary>
		/// <param name="elementname"></param>
		/// <param name="code"></param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="inlineattributes"></param>
		/// <param name="linedattributes"></param>
		public void StartElement(string elementname, string code = null, string name = null, string value = null,
		                         object inlineattributes = null, object linedattributes = null){
			WriteElement(elementname, code, name, value, inlineattributes, linedattributes);
			Indent();
		}


		/// <summary>
		///     Записывает  элемента
		/// </summary>
		/// <param name="elementname"></param>
		/// <param name="code"></param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="inlineattributes"></param>
		/// <param name="linedattributes"></param>
		public void WriteElement(string elementname, string code = null, string name = null, string value = null,
		                         object inlineattributes = null, object linedattributes = null){
			WriteLevel();
			_buffer.Append(elementname.Escape(EscapingType.BxlLiteral));
			_buffer.Append(' ');
			_buffer.Append(code.Escape(EscapingType.BxlStringOrLiteral));
			if (!string.IsNullOrWhiteSpace(name)){
				_buffer.Append(' ');
				_buffer.Append(name.Escape(EscapingType.BxlSinglelineString));
			}
			WriteAttributesInline(inlineattributes);
			if (!string.IsNullOrWhiteSpace(value)){
				_buffer.Append(" : ");
				_buffer.Append(value.Escape(EscapingType.BxlMultilineString));
			}

			Indent();
			if (null != linedattributes){
				EnsureAppendLine();
				WriteAttributesLined(linedattributes);
			}
			Dedent();
			EnsureAppendLine();
		}

		/// <summary>
		///     Начинает блок класса
		/// </summary>
		/// <param name="name"></param>
		/// <param name="attributes"></param>
		public void StartClass(string name, object attributes = null){
			StartElement(BSharpSyntax.Class, name, null, null, attributes);
		}

		/// <summary>
		///     Формирует строку атрибутов в той же строке с элементом
		/// </summary>
		/// <param name="attributes"></param>
		public void WriteAttributesInline(object attributes){
			if (null == attributes) return;
			IDictionary<string, object> dict = attributes.ToDict();
			foreach (var o in dict){
				string val = GetValue(o.Value);
				if (null != val){
					_buffer.Append(' ');
					WriteAttribute(o.Key, o.Value);
				}
			}
		}

		/// <summary>
		///     Формирует строку атрибутов в той же строке с элементом
		/// </summary>
		/// <param name="attributes"></param>
		public void WriteAttributesLined(object attributes){
			if (null == attributes) return;
			IDictionary<string, object> dict = attributes.ToDict();
			foreach (var o in dict){
				string val = GetValue(o.Value);
				if (null == val) continue;
				WriteLevel();
				WriteAttribute(o.Key, o.Value);
				EnsureAppendLine();
			}
		}

		/// <summary>
		///     Записать атрибут
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void WriteAttribute(string key, object value){
			string val = GetValue(value);
			if (null == val) return;

			_buffer.Append(key.Escape(EscapingType.BxlLiteral));
			_buffer.Append('=');
			if (val.IsLiteral(EscapingType.BxlLiteral)){
				_buffer.Append(val);
			}
			else{
				_buffer.Append(val.Escape(EscapingType.BxlMultilineString));
			}
		}

		/// <summary>
		///     Закончить пространство
		/// </summary>
		public void EndNamespace(){
			Dedent();
		}

		/// <summary>
		///     Закончить блок класса
		/// </summary>
		public void EndClass(){
			Dedent();
		}

		/// <summary>
		///     Закончить блок класса
		/// </summary>
		public void EndElement(){
			Dedent();
		}

		/// <summary>
		///     Записывает информацию об элементах
		/// </summary>
		/// <param name="dictcode"></param>
		/// <returns></returns>
		public IBSharpCodeBuilder WriteClassExport(string dictcode){
			WriteElement(BSharpSyntax.ClassExportDefinition, dictcode);
			return this;
		}

		/// <summary>
		///     Записывает информацию об элементах
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		public IBSharpCodeBuilder WriteClassElement(string element){
			WriteElement(BSharpSyntax.ClassElementDefinition, element);
			return this;
		}

		/// <summary>
		///     Возвращает текущее строчное преобразование буфера
		/// </summary>
		/// <returns></returns>
		public override string ToString(){
			return _buffer.ToString();
		}

		/// <summary>
		///     Позволяет добавить перенос строки, но не позволяет это сделать 2жды подряд
		/// </summary>
		private void EnsureAppendLine(){
			if (_buffer.Length == lastAppendLine) return;
			_buffer.AppendLine();
			lastAppendLine = _buffer.Length;
		}

		private static string GetValue(object value){
			if (null == value) return null;
			if (Equals(false, value)) return null;
			if (Equals(0, value)) return null;
			if (value is DateTime){
				if (((DateTime) value) == QorpentConst.Date.Begin){
					return null;
				}
			}
			string val = value.ToStr().Trim();
			if (string.IsNullOrEmpty(val)){
				return null;
			}
			return val;
		}
	}
}