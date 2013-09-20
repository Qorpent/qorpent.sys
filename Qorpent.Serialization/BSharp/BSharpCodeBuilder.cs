using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qorpent.IoC;
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
            WriteLevel();
            _buffer.Append(BSharpSyntax.Namespace);
            _buffer.Append(' ');
            _buffer.Append(ns);
            _buffer.AppendLine();
            WriteAttributesInline(attributes);
            Indent();
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
                _buffer.Append(o.Key);
                _buffer.Append('=');

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

    }
}
