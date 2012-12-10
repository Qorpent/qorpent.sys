namespace Qorpent.Utils {
	/// <summary>
	/// Universal parser for lists and dictionaries
	/// </summary>
	public class ComplexStringHelper {
		/// <summary>
		/// Префикс элемента строки
		/// </summary>
		public string ItemPrefix { get; set; }
		/// <summary>
		/// Суффкс элемента
		/// </summary>
		public string ItemSuffix { get; set; }
		/// <summary>
		/// Разделитель элементов
		/// </summary>
		public string ItemDelimiter { get; set; }
		/// <summary>
		/// Разделитель имя-значение
		/// </summary>
		public string ValueDelimiter { get; set; }
		/// <summary>
		/// Значение при отсутствии элемента
		/// </summary>
		public string NotExistedValue { get; set; }
		/// <summary>
		/// Значение при пустом элементе
		/// </summary>
		public string EmptyValue { get; set; }
		/// <summary>
		/// Строка - экран для  разделителя
		/// </summary>
		public string ItemDelimiterSubstitution { get; set; }
		/// <summary>
		/// Строка - экран для префикса разделителя
		/// </summary>
		public string ItemPrefixSubstitution { get; set; }
		/// <summary>
		/// Строка - экран для суффикса разделителя
		/// </summary>
		public string ItemSuffixSubstitution { get; set; }
		/// <summary>
		/// Строка - экран разделителя имя-значение
		/// </summary>
		public string ValueDelimiterSubstitution { get; set; }


		/// <summary>
		/// Создает парсер строк вида /x:val/ /y/ /z:~\~/
		/// </summary>
		/// <returns></returns>
		public static ComplexStringHelper CreateTagParser () {
			return new ComplexStringHelper
				{
					ItemPrefix = "/",
					ItemSuffix = "/",
					ItemDelimiter = "",
					ValueDelimiter = ":",
					NotExistedValue = "",
					EmptyValue = "",
					ItemPrefixSubstitution = "~\\",
					ItemSuffixSubstitution = "~\\",
					ValueDelimiterSubstitution = "~"
				};
		}

		/// <summary>
		/// Создает парсер строк вида <see cref="ComplexStringExtension"/>
		/// </summary>
		/// <returns></returns>
		public static ComplexStringHelper CreateComplexStringParser()
		{
			return new ComplexStringHelper
				{
					ItemPrefix = "",
					ItemSuffix = "",
					ItemDelimiter = "|",
					ValueDelimiter = ":",
					NotExistedValue = "",
					EmptyValue = "",
					ItemDelimiterSubstitution = "~\\",
					ValueDelimiterSubstitution = "~"
				};
		}
	}
}