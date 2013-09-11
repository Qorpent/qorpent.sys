using System.Linq;
using System.Text;
using Qorpent.Model;
using Qorpent.Serialization;

namespace Qorpent.Dot {
    /// <summary>
	/// Базовые свойства элементов графа
	/// </summary>

    public abstract class GraphElementBase : AttributedEntity {
        /// <summary>
		/// Заголовок
		/// </summary>
	    [IgnoreSerialize]
	    public string Label {
	        get { return Get<string>(DotConstants.LabelAttribute); }
	        set { Set(DotConstants.LabelAttribute, value); }
	    }

        private string _code;
        /// <summary>
        /// 
        /// </summary>
        public override string Code  {
            get { return string.IsNullOrWhiteSpace(_code) ? (_code = "NULL") : _code; }
            set { _code = DotLanguageUtils.EscapeCode(value); }
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
	    /// Цветовая схема
	    /// </summary>
	    [IgnoreSerialize]
	    public string ColorScheme {
	        get { return Get<string>(DotConstants.ColorSchemeAttribute); }
	        set { Set(DotConstants.ColorSchemeAttribute, value); }
	    }
    }
}