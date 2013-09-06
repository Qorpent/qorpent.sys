﻿using System.Collections.Generic;
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
	    /// Код субграфа
	    /// </summary>
	    public string Code { get; set; }
	}
}