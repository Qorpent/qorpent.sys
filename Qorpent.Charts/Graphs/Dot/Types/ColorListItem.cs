using System.Globalization;

namespace Qorpent.Graphs.Dot.Types {
    /// <summary>
    /// ��������� ������� ������ ������
    /// </summary>
    public class ColorListItem {
        /// <summary>
        /// ����
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// ���
        /// </summary>
        public decimal Weight { get; set; }

        /// <summary>
        /// ��������� ������ � ��������� ���������
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            if (0 == Weight) {
                return Color.ToString();
            }
            return Color + ";" + Weight.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// ������������ ��������� ����� � �������� ���� Single
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static implicit operator ColorListItem(Color color)
        {
            var result = new ColorListItem {Color = color};
            return result;
        }

        /// <summary>
        /// ��������� ���������� ����� � ������� ������� 
        /// </summary>
        /// <returns></returns>
        public static ColorList operator +(ColorListItem item, Color color) {
            return new ColorList() + item + color;
        }
    }
}