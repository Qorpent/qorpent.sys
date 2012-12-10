namespace Qorpent.Utils {
	/// <summary>
	/// Universal parser for lists and dictionaries
	/// </summary>
	public class ComplexStringHelper {
		/// <summary>
		/// ������� �������� ������
		/// </summary>
		public string ItemPrefix { get; set; }
		/// <summary>
		/// ������ ��������
		/// </summary>
		public string ItemSuffix { get; set; }
		/// <summary>
		/// ����������� ���������
		/// </summary>
		public string ItemDelimiter { get; set; }
		/// <summary>
		/// ����������� ���-��������
		/// </summary>
		public string ValueDelimiter { get; set; }
		/// <summary>
		/// �������� ��� ���������� ��������
		/// </summary>
		public string NotExistedValue { get; set; }
		/// <summary>
		/// �������� ��� ������ ��������
		/// </summary>
		public string EmptyValue { get; set; }
		/// <summary>
		/// ������ - ����� ���  �����������
		/// </summary>
		public string ItemDelimiterSubstitution { get; set; }
		/// <summary>
		/// ������ - ����� ��� �������� �����������
		/// </summary>
		public string ItemPrefixSubstitution { get; set; }
		/// <summary>
		/// ������ - ����� ��� �������� �����������
		/// </summary>
		public string ItemSuffixSubstitution { get; set; }
		/// <summary>
		/// ������ - ����� ����������� ���-��������
		/// </summary>
		public string ValueDelimiterSubstitution { get; set; }


		/// <summary>
		/// ������� ������ ����� ���� /x:val/ /y/ /z:~\~/
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
		/// ������� ������ ����� ���� <see cref="ComplexStringExtension"/>
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