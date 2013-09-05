using System.Collections.Generic;
using Qorpent.Dot.Colors;
using Qorpent.Utils.Extensions;

namespace Qorpent.Dot {
	/// <summary>
	/// Базовые свойства элементов графа
	/// </summary>
	public abstract class GraphElementBase {
		/// <summary>
		/// 
		/// </summary>
		public IDictionary<string, string> Attributes = new Dictionary<string, string>();
		/// <summary>
		/// Защищенный метод доступа к атрибутам на чтение
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public string Get(string code) {
			if (Attributes.ContainsKey(code)) {
				return Attributes[code];
			}
			return string.Empty;
		}
		/// <summary>
		/// Установить атрибут
		/// </summary>
		/// <param name="code"></param>
		/// <param name="value"></param>
		public void Set(string code, string value) {
			Attributes[code] = value;
		}

		/// <summary>
		/// Заголовок
		/// </summary>
		public string Label {
			get { return Get(DotConstants.LabelAttribute); }
			set { Set(DotConstants.LabelAttribute,value); }
		}
        /// <summary>
        /// Размер шрифта, по умолчанию 14 
        /// </summary>
        public double FontSize
        {
            get { return Get(DotConstants.FontSizeAttribute).To<double>(); }
            set
            {
                var str = value.ToStr().ToLower();
                Set(DotConstants.FontSizeAttribute, str);

            }
        }
        /// <summary>
        /// Цвет используемый для текста. По умолчанию черный 
        /// </summary>
        public ColorAttribute FontColor
        {
            get { return ColorAttribute.Native(Get(DotConstants.FontColorAttribute)); }
            set
            {
               Set(DotConstants.FontColorAttribute, value.ToString());

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
                return Get(DotConstants.FontNameAttribute); 
            }
            set {
                if (value == DotConstants.DefaultFontNameAttribute)
                {
                    Attributes.Remove(DotConstants.FontNameAttribute);
                }
                else {
                    Set(DotConstants.FontNameAttribute, value); 
                }
                
            }
        }
	}
}