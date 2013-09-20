using System.Text;
using Qorpent.IoC;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.BSharp
{
    /// <summary>
    /// Построитель кода на B#
    /// </summary>
    [ContainerComponent(ServiceType = typeof(IBSharpCodeBuilder))]
    public class BSharpCodeBuilder:IBSharpCodeBuilder {
        private readonly StringBuilder _buffer = new StringBuilder();
        /// <summary>
        /// Уровень вложенности элементов
        /// </summary>
        public int Level {
            get { return _level; }
        }

        /// <summary>
        /// Возвращает текущее строчное преобразование буфера
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return _buffer.ToString();
        }

        private int _level;
        /// <summary>
        /// Увеличить отступ
        /// </summary>
        public void Indent() {
            _level++;
        }
        /// <summary>
        /// Уменьшить отступ
        /// </summary>
        public void Dedent() {
            if (_level > 0) {
                _level -= 1;
            }
        }
        /// <summary>
        /// Прописывает отступ по уровню
        /// </summary>
        public void WriteLevel() {
            for (var i = 0; i < Level; i++) {
                _buffer.Append('\t');
            }
        }

        /// <summary>
        /// Прописывает блок комментариев
        /// </summary>
        /// <param name="commentSource"></param>
        public void WriteCommentBlock(object commentSource = null) {
            if (null == commentSource) {
                return;
            }
            WriteCommentLine();
            var dict = commentSource.ToDict();
            foreach (var p in dict) {
                for (var i = 0; i < 4; i++) {
                    _buffer.Append('#');    
                }
                for (var i = 0; i < 20; i++)
                {
                    _buffer.Append(' ');
                }
                var key = p.Key;
                _buffer.Append(key);
                for (var i = 0; i < 30 - key.Length;i++ )
                {
                    _buffer.Append(' ');
                }
                _buffer.Append(" : ");
                var val = p.Value.ToString();
                _buffer.Append(val);
                for (var i = 0; i < 39 - val.Length;i++) {
                    _buffer.Append(' ');    
                }
                for (var i = 0; i < 20; i++)
                {
                    _buffer.Append(' ');
                }
                for (var i = 0; i < 4; i++)
                {
                    _buffer.Append('#');
                }
                _buffer.AppendLine();
            }

            WriteCommentLine();
        }
        /// <summary>
        /// Формирует строку комментариев
        /// </summary>
        public void WriteCommentLine() {
            for (var i = 0; i < 120; i++) {
                _buffer.Append('#');
            }
            _buffer.AppendLine();
        }

        /// <summary>
        /// Начинает блок пространства имен
        /// </summary>
        /// <param name="ns"></param>
        /// <param name="attributes"></param>
        public void StartNamespace(string ns, object attributes =null) {
            StartElement(BSharpSyntax.Namespace, ns, null, null, attributes);
        }
        /// <summary>
        /// Начать элемент
        /// </summary>
        /// <param name="elementname"></param>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="inlineattributes"></param>
        /// <param name="linedattributes"></param>
        public void StartElement(string elementname, string code=null, string name = null, string value = null, object inlineattributes = null, object linedattributes = null)
        {
            WriteElement(elementname,code,name,value,inlineattributes,linedattributes);
            Indent();
        }
        

        /// <summary>
        /// Записывает  элемента
        /// </summary>
        /// <param name="elementname"></param>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="inlineattributes"></param>
        /// <param name="linedattributes"></param>
        public void WriteElement(string elementname, string code=null, string name=null, string value=null, object inlineattributes=null,object linedattributes = null) {
            WriteLevel();
            _buffer.Append(elementname.Escape(EscapingType.BxlLiteral));
            _buffer.Append(' ');
            _buffer.Append(code);
            if (!string.IsNullOrWhiteSpace(name)) {
                _buffer.Append(' ');
                _buffer.Append(name.Escape(EscapingType.BxlSinglelineString));
            }
            WriteAttributesInline(inlineattributes);
            if (!string.IsNullOrWhiteSpace(value)) {
                _buffer.Append(" : ");
                _buffer.Append(value.Escape(EscapingType.BxlMultilineString));
            }
            Indent();
            WriteAttributesLined(linedattributes);
            Dedent();
            _buffer.AppendLine();
        }

        /// <summary>
        /// Начинает блок класса
        /// </summary>
        /// <param name="name"></param>
        /// <param name="attributes"></param>
        public void StartClass(string name, object attributes = null) {
            StartElement(BSharpSyntax.Class, name, null, null,attributes );
        }

        /// <summary>
        /// Формирует строку атрибутов в той же строке с элементом
        /// </summary>
        /// <param name="attributes"></param>
        public void WriteAttributesInline(object attributes) {
            if (null == attributes) return;
            var dict = attributes.ToDict();
            foreach (var o in dict) {
                _buffer.Append(' ');
                WriteAttribute(o.Key,o.Value);
            }
        }
        /// <summary>
        /// Формирует строку атрибутов в той же строке с элементом
        /// </summary>
        /// <param name="attributes"></param>
        public void WriteAttributesLined(object attributes)
        {
            if (null == attributes) return;
            var dict = attributes.ToDict();
            foreach (var o in dict)
            {
                WriteLevel();
                WriteAttribute(o.Key,o.Value);
                _buffer.AppendLine();
            }
        }
        /// <summary>
        /// Записать атрибут
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void WriteAttribute(string key,object value) {
            var val = value.ToStr();
            _buffer.Append(key.Escape(EscapingType.BxlLiteral));
            _buffer.Append('=');
            if (val.IsLiteral(EscapingType.BxlLiteral)) {
                _buffer.Append(val);
            }
            else {
                _buffer.Append(val.Escape(EscapingType.BxlMultilineString));
            }
        }

        /// <summary>
        /// Закончить пространство
        /// </summary>
        public void EndNamespace() {
            Dedent();
        }
        /// <summary>
        /// Закончить блок класса
        /// </summary>
        public void EndClass()
        {
            Dedent();
        }

        /// <summary>
        /// Закончить блок класса
        /// </summary>
        public void EndElement()
        {
            Dedent();
        }

    }
}
