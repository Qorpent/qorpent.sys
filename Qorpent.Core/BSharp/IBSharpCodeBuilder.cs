using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qorpent.BSharp
{
    /// <summary>
    /// Интерфейс построителя кода на B#
    /// </summary>
    public interface IBSharpCodeBuilder
    {
        /// <summary>
        /// Уровень вложенности элементов
        /// </summary>
        int Level { get; }

        /// <summary>
        /// Увеличить отступ
        /// </summary>
        void Indent();

        /// <summary>
        /// Уменьшить отступ
        /// </summary>
        void Dedent();

        /// <summary>
        /// Прописывает отступ по уровню
        /// </summary>
        void WriteLevel();

        /// <summary>
        /// Прописывает блок комментариев
        /// </summary>
        /// <param name="title"></param>
        /// <param name="commentSource"></param>
        void WriteCommentBlock(string title,object commentSource = null);

        /// <summary>
        /// Формирует строку комментариев
        /// </summary>
        void WriteCommentLine();

        /// <summary>
        /// Начинает блок пространства имен
        /// </summary>
        /// <param name="ns"></param>
        /// <param name="attributes"></param>
        void StartNamespace(string ns, object attributes =null);

        /// <summary>
        /// Начать элемент
        /// </summary>
        /// <param name="elementname"></param>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="inlineattributes"></param>
        /// <param name="linedattributes"></param>
        void StartElement(string elementname, string code, string name, string value = null, object inlineattributes = null, object linedattributes = null);

        /// <summary>
        /// Записывает  элемента
        /// </summary>
        /// <param name="elementname"></param>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="inlineattributes"></param>
        /// <param name="linedattributes"></param>
        void WriteElement(string elementname, string code, string name, string value=null, object inlineattributes=null,object linedattributes = null);

        /// <summary>
        /// Начинает блок класса
        /// </summary>
        /// <param name="name"></param>
        /// <param name="attributes"></param>
        void StartClass(string name, object attributes = null);

        /// <summary>
        /// Формирует строку атрибутов в той же строке с элементом
        /// </summary>
        /// <param name="attributes"></param>
        void WriteAttributesInline(object attributes);

        /// <summary>
        /// Формирует строку атрибутов в той же строке с элементом
        /// </summary>
        /// <param name="attributes"></param>
        void WriteAttributesLined(object attributes);

        /// <summary>
        /// Записать атрибут
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void WriteAttribute(string key,object value);

        /// <summary>
        /// Закончить пространство
        /// </summary>
        void EndNamespace();

        /// <summary>
        /// Закончить блок класса
        /// </summary>
        void EndClass();

        /// <summary>
        /// Закончить блок класса
        /// </summary>
        void EndElement();
    }
}
