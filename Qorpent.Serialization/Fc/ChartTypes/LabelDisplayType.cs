namespace Qorpent.Fc.ChartTypes {
    /// <summary>
    /// Типы отображения подписей
    /// </summary>
    public enum LabelDisplayType {
        /// <summary>
        /// Автоматически (в зависимости от размеров графика)
        /// </summary>
        Auto,
        /// <summary>
        /// Обрезает текст подписи если он привышает допустимые размеры
        /// </summary>
        Wrap,
        /// <summary>
        /// Разворачивает подпись вертикально
        /// </summary>
        Rotate,
        /// <summary>
        /// Разбивает подпись на несколько строк 
        /// </summary>
        Stagger,
        /// <summary>
        /// 
        /// </summary>
        None
    }
}
