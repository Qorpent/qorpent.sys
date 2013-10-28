using System.Collections.Generic;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.Model {
    /// <summary>
    /// Базис для моделей внешних API
    /// </summary>
    [Serialize]
    public  class AttributedEntity {
        /// <summary>
        /// 
        /// </summary>
        [SerializeNotNullOnly]
        public IDictionary<string, object> Attributes = new Dictionary<string, object>();

        /// <summary>
        /// Заголовок
        /// </summary>
        [IgnoreSerialize]
        public string Id {
            get { return Get<string>("id"); }
            set { Set("id", value); }
        }

        /// <summary>
        /// Код субграфа
        /// </summary>
        [Serialize]
        public virtual string Code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IgnoreSerialize]
        public object Data { get; set; }

        /// <summary>
        /// Защищенный метод доступа к атрибутам на чтение
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public T Get<T>(string code) {
            if (Attributes.ContainsKey(code)) {
                return (T)Attributes[code];
            }
            return default(T);
        }

        /// <summary>
        /// Позволяет перекрывать  атрибуты
        /// </summary>
        /// <param name="code"></param>
        /// <param name="value"></param>
        public void OverrideAttribute(string code, object value) {
            object newval = value;
            if (Attributes.ContainsKey(code)) {
                var val = Attributes[code];
                var type = null == val ? typeof (string) : val.GetType();
                newval = value.ToTargetType(type);
            }
            Attributes[code] = newval;
        }

        /// <summary>
        /// Установить атрибут
        /// </summary>
        /// <param name="code"></param>
        /// <param name="value"></param>
        public void Set(string code, object value) {
            Attributes[code] = value;
        }

        /// <summary>
        /// Сводит узлы
        /// </summary>
        public void Merge(AttributedEntity other) {
            foreach (var a in other.Attributes) {
               OverrideAttribute(a.Key,a.Value);
            }
        }
    }
}