using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qorpent.Utils.Extensions;

namespace Qorpent.Dot {
	/// <summary>
	/// Базовые свойства элементов графа
	/// </summary>
	public abstract class GraphElementBase {
	    /// <summary>
		/// 
		/// </summary>
		public IDictionary<string, object> Attributes = new Dictionary<string, object>();

	    private string _code;

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
		/// Установить атрибут
		/// </summary>
		/// <param name="code"></param>
		/// <param name="value"></param>
		public void Set(string code, object value) {
			Attributes[code] = value;
		}

        

		/// <summary>
		/// Заголовок
		/// </summary>
		public string Label {
			get { return Get<string>(DotConstants.LabelAttribute); }
			set { Set(DotConstants.LabelAttribute,value); }
		}
        /// <summary>
        /// Размер шрифта, по умолчанию 14 
        /// </summary>
        public double FontSize
        {
            get { return Get<double>(DotConstants.FontSizeAttribute); }
            set
            {
                Set(DotConstants.FontSizeAttribute, value);

            }
        }
        /// <summary>
        /// Цвет используемый для текста. По умолчанию черный 
        /// </summary>
        public ColorAttribute FontColor
        {
            get { return Get<ColorAttribute>(DotConstants.FontColorAttribute); }
            set
            {
               Set(DotConstants.FontColorAttribute, value);

            }
        }


	    /// <summary>
		/// Автонастройка
		/// </summary>
		public virtual void AutoTune() {
			
		}
		/// <summary>
		/// Проверяет наличие атрибута
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool HasAttribute(string name) {
			return Attributes.ContainsKey(name);
		}

        /// <summary>
        /// Заголовок
        /// </summary>
        public string FontName
        {
            get {
                return Get<string>(DotConstants.FontNameAttribute); 
            }
            set {
                Set(DotConstants.FontNameAttribute, value); 
            }
        }

        /// <summary>
        /// Заголовок
        /// </summary>
        public string Id
        {
            get
            {
                return Get<string>(DotConstants.IdAttribute);
            }
            set
            {
                Set(DotConstants.IdAttribute, value);
            }
        }

	    /// <summary>
	    /// Код субграфа
	    /// </summary>
	    public string Code {
            get { return string.IsNullOrWhiteSpace(_code) ? (_code = DotLanguageUtils.NULLCODE) : _code; }
	        set { _code = DotLanguageUtils.EscapeCode(value);}
	    }

        

	    /// <summary>
        /// 
        /// </summary>
        public object Data { get; set; }

	    /// <summary>
	    /// Сводит узлы
	    /// </summary>
	    /// <param name="otherNode"></param>
	    public void Merge(GraphElementBase otherNode) {
	        foreach (var a in otherNode.Attributes) {
	            Attributes[a.Key] = a.Value;
	        }
	    }
	}
}