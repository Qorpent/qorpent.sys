using Qorpent.Graphs.Dot.Types;

namespace Qorpent.Graphs.Dot
{
    /// <summary>
    /// Основные атрибуты Dot для упрощенного построения визуальных графов
    /// </summary>
    public static class BuilderExtensions
    {
        /// <summary>
        /// Устанавливает направление развертывания направленного графа
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public static T SetLabel<T>(this T builder, string label) where T:IGraphElementBuilder
        {
            builder.Set(DotConstants.LabelAttribute, label);
            return builder;
        }

        /// <summary>
        /// Устанавливает направление развертывания направленного графа
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="rankdir"></param>
        /// <returns></returns>
        public static IGraphBuilder SetRankDir(this IGraphBuilder builder, RankDirType rankdir) {
            builder.Set(DotConstants.RankDirAttribute, rankdir);
            return builder;
        }
        /// <summary>
        /// Устанавливает форму узла
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="shape"></param>
        /// <returns></returns>
        public static IGraphNodeBuilder SetShape(this IGraphNodeBuilder builder, NodeShapeType shape) {
            builder.Set(DotConstants.ShapeAttribute, shape);
            return builder;
        }

        /// <summary>
        /// Устанавливает стиль узла
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        public static IGraphNodeBuilder SetStyle(this IGraphNodeBuilder builder, NodeStyleType style)
        {
            builder.Set(DotConstants.StyleAttribute, style);
            return builder;
        }

        /// <summary>
        /// Устанавливает стиль узла
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="fillcolor"></param>
        /// <returns></returns>
        public static IGraphNodeBuilder SetFillColor(this IGraphNodeBuilder builder, ColorAttribute fillcolor)
        {
            builder.Set(DotConstants.FillColorAttribute, fillcolor);
            return builder;
        }

        /// <summary>
        /// Устанавливает стиль узла
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        public static IGraphEdgeBuilder SetStyle(this IGraphEdgeBuilder builder, EdgeStyleType style)
        {
            builder.Set(DotConstants.StyleAttribute, style);
            return builder;
        }

        /// <summary>
        /// Устанавливает стиль узла
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="arrow"></param>
        /// <returns></returns>
        public static IGraphEdgeBuilder SetArrowHead(this IGraphEdgeBuilder builder, Arrow arrow)
        {
            builder.Set(DotConstants.ArrowHeadAttribute, arrow);
            return builder;
        }

        /// <summary>
        /// Устанавливает стиль узла
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static IGraphEdgeBuilder SetPenWidht(this IGraphEdgeBuilder builder, int width)
        {
            builder.Set(DotConstants.PenwidthAttribute, width);
            return builder;
        }



        /// <summary>
        /// Устанавливает стиль узла
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static T SetColor<T>(this T builder, ColorAttribute color) where T:IGraphElementBuilder
        {
            builder.Set(DotConstants.ColorAttribute, color);
            return builder;
        }

       
    }
}
