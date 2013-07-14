﻿namespace Qorpent.Mvc.Loader {
    /// <summary>
    /// Описание элемента для загрузки
    /// </summary>
    public class LoadItem {
        /// <summary>
        /// Type of item to be load
        /// </summary>
        public LoadItemType Type { get; set; }
        /// <summary>
        /// Level of item
        /// </summary>
        public LoadLevel Level { get; set; }

        /// <summary>
        /// file name for styles,scripts, attribute content for others
        /// </summary>
        public string Value { get; set; }
        
    }
}