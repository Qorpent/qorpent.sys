using System.Collections.Generic;
using Qorpent.Graphs.Dot.Types;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.Graphs.Dot {
	/// <summary>
	/// Базовые свойства элементов графа
	/// </summary>
	[Serialize]
    public abstract class GraphElementBase {
	    /// <summary>
		/// 
		/// </summary>
    [SerializeNotNullOnly]
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
		/// Заголовок
		/// </summary>
	    [IgnoreSerialize]
	    public string Label {
	        get { return Get<string>(DotConstants.LabelAttribute); }
	        set { Set(DotConstants.LabelAttribute, value); }
	    }

	    /// <summary>
        /// Размер шрифта, по умолчанию 14 
        /// </summary>
	    [IgnoreSerialize]
	    public double FontSize {
	        get { return Get<double>(DotConstants.FontSizeAttribute); }
	        set { Set(DotConstants.FontSizeAttribute, value); }
	    }

	    /// <summary>
        /// Цвет используемый для текста. По умолчанию черный 
        /// </summary>
	    [IgnoreSerialize]
	    public ColorAttribute FontColor {
	        get { return Get<ColorAttribute>(DotConstants.FontColorAttribute); }
	        set { Set(DotConstants.FontColorAttribute, value); }
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
	    [IgnoreSerialize]
	    public string FontName {
	        get { return Get<string>(DotConstants.FontNameAttribute); }
	        set { Set(DotConstants.FontNameAttribute, value); }
	    }

	    /// <summary>
        /// Заголовок
        /// </summary>
	    [IgnoreSerialize]
	    public string Id {
	        get { return Get<string>(DotConstants.IdAttribute); }
	        set { Set(DotConstants.IdAttribute, value); }
	    }

	    /// <summary>
	    /// Код субграфа
	    /// </summary>
	    [Serialize]
        public string Code {
            get { return string.IsNullOrWhiteSpace(_code) ? (_code = DotLanguageUtils.NULLCODE) : _code; }
	        set { _code = DotLanguageUtils.EscapeCode(value);}
	    }

        

	    /// <summary>
        /// Дата
        /// </summary>
        [IgnoreSerialize]
        public object Data { get; set; }

	    /// <summary>
	    /// Цветовая схема
	    /// </summary>
	    [IgnoreSerialize]
	    public string ColorScheme {
	        get { return Get<string>(DotConstants.ColorSchemeAttribute); }
	        set { Set(DotConstants.ColorSchemeAttribute, value); }
	    }

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