namespace Qorpent.Mvc.Loader {
    /// <summary>
    /// Тип загружаемого элемента
    /// </summary>
    public enum LoadItemType {
        /// <summary>
        /// META tag
        /// </summary>
        Meta,
        /// <summary>
        /// LINK (no style-sheet)
        /// </summary>
        Link,
        /// <summary>
        /// Script
        /// </summary>
        Script,
        /// <summary>
        /// CSS- file
        /// </summary>
        Style
    }
}