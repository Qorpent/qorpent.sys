namespace Qorpent.Data {
    /// <summary>
    /// Опции запроса к документному хранилищу
    /// </summary>
    public class DocumentStorageOptions {
        /// <summary>
        /// Список полей проекций для показа
        /// </summary>
        public string[] Fields { get; set; }
        /// <summary>
        /// Лимит результатов
        /// </summary>
        public int Limit { get; set; }
		/// <summary>
		///		Коллекция или пространство имён
		/// </summary>
		public string Collection { get; set; }
        /// <summary>
        /// Преобразует массив строк в опции запроса на поля
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static implicit operator DocumentStorageOptions(string[] fields)
        {
            return new DocumentStorageOptions{Fields = fields};
        }
    }
}