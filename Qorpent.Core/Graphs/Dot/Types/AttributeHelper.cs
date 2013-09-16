using System.Text;

namespace Qorpent.Graphs.Dot.Types {
    /// <summary>
    /// Вспомогательный класс генерации строк для стилей
    /// </summary>
    public static class AttributeHelper {
        /// <summary>
        /// Конвертирует типы шейпов в строки
        /// </summary>
        /// <param name="shapeType"></param>
        /// <returns></returns>
        public static string GetAttributeString(this NodeShapeType shapeType) {
            if (shapeType == NodeShapeType.Mcircle) return "Mcircle";
            if (shapeType == NodeShapeType.Mdiamond) return "Mdiamond";
            if (shapeType == NodeShapeType.Msquare) return "Msquare";
            if (shapeType == NodeShapeType.Mrecord) return "Mrecord";
            return shapeType.ToString().ToLower();
        }

        /// <summary>
        /// Конвертирует стили узлов
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        public static string GetAttributeString(this NodeStyleType style) {
            var usebold = false;
            var cornerstyle = "";
            var linestyle = "";
            var areastyle = "";
            
            if (style.HasFlag(NodeStyleType.Bold)) {
                usebold = true;
            }
            if (style.HasFlag(NodeStyleType.Dashed)) {
                linestyle = "dashed";
            }
            if (style.HasFlag(NodeStyleType.Dotted)) {
                linestyle = "dotted";
            }
            if (style.HasFlag(NodeStyleType.Rounded)) {
                cornerstyle = "rounded";

            }
            if (style.HasFlag(NodeStyleType.Diagonals)) {
                cornerstyle = "diagonals";
            }
            if (style.HasFlag(NodeStyleType.Striped))
            {
                areastyle = "striped";
            }
            if (style.HasFlag(NodeStyleType.Wedged))
            {
                areastyle = "wedget";
            }
            if (style.HasFlag(NodeStyleType.Filled)) {
                areastyle = "filled";
            }
            if (areastyle == "striped" || areastyle == "wedged") {
                cornerstyle = "";
            }
            var sb = new StringBuilder();
            var first = true;
            var requirequots = false;
            if (usebold) {
                sb.Append("bold");
                first = false;
            }
            if (!string.IsNullOrWhiteSpace(linestyle)) {
                if (!first) {
                    sb.Append(",");
                    requirequots = true;
                }
                sb.Append(linestyle);
                first = false;
            }
            if (!string.IsNullOrWhiteSpace(cornerstyle))
            {
                if (!first)
                {
                    sb.Append(",");
                    requirequots = true;
                }
                sb.Append(cornerstyle);
                first = false;
            }
            if (!string.IsNullOrWhiteSpace(areastyle))
            {
                if (!first)
                {
                    sb.Append(",");
                    requirequots = true;
                }
                sb.Append(areastyle);
            }
            if (requirequots) return "\"" + sb + "\"";
            return sb.ToString();
        }
    }
}