using NUnit.Framework;
using Qorpent.Utils.BrickScaleNormalizer;

namespace Qorpent.Utils.Tests.BrickScale.DataColonNormalizer {
	/// <summary>
	///		������� ����� �������� ��� ������������ <see cref="DataColonLabelHelper"/>
	/// </summary>
	public class DataColonTestBase {
		/// <summary>
		///		������������ �������� �� �����
		/// </summary>
		public const decimal ScaleMax = 100;
		/// <summary>
		///		����������� �������� �� �����
		/// </summary>
		public const decimal ScaleMin = 0;
		/// <summary>
		///		������ �����
		/// </summary>
		public const decimal Height = 100;
		/// <summary>
		///		������� ������������ ������ � �����
		/// </summary>
		public const ColonDataItemOrder Order = ColonDataItemOrder.Real;
		/// <summary>
		///		������ �����
		/// </summary>
		public const decimal LabelHeight = 27;
		/// <summary>
		///		�������� �������
		/// </summary>
		protected DataColonLabelHelper ColonLabelHelper;
		[SetUp]
		public void SetUp() {
			ColonLabelHelper = new DataColonLabelHelper {
				Height = Height,
				LabelHeight = LabelHeight,
				Order = Order,
				ScaleMax = ScaleMax,
				ScaleMin = ScaleMin
			};
		}
	}
}