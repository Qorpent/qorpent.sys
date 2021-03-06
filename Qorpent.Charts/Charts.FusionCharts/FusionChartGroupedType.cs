﻿namespace Qorpent.Charts.FusionCharts {
    /// <summary>
    /// Группы графиков FusionChart
    /// </summary>
    public enum  FusionChartGroupedType: long {
        /// <summary>
        /// Графики с одной серией
        /// </summary>
        SingleSeries = 
            FusionChartType.Column2D |
                   FusionChartType.Column3D |
                   FusionChartType.Pie2D |
                   FusionChartType.Pie3D |
                   FusionChartType.Line |
                   FusionChartType.Bar2D | 
                   FusionChartType.Area2D |
                   FusionChartType.Doughnut2D |
                   FusionChartType.Doughnut3D |
                   FusionChartType.Pareto2D |
                   FusionChartType.Pareto3D | FusionChartType.Pyramid,

        /// <summary>
        /// Мультисерийные
        /// </summary>
        MultiSeries =
            FusionChartType.MSColumn2D |
            FusionChartType.MSColumn3D |
            FusionChartType.MSLine |
            FusionChartType.ZoomLine |
            FusionChartType.MSArea |
            FusionChartType.MSBar2D |
            FusionChartType.MSBar3D |
            FusionChartType.Marimekko|
			FusionChartType.Radar,

        /// <summary>
        /// Состыкованные
        /// </summary>
        Stacked =
            FusionChartType.StackedColumn2D |
            FusionChartType.StackedColumn3D |
            FusionChartType.StackedArea2D |
            FusionChartType.StackedBar2D |
            FusionChartType.StackedBar3D |
			FusionChartType.MSStackedColumn2D
			,

        /// <summary>
        /// Комбинированные
        /// </summary>
        Combination =
            FusionChartType.MSCombi3D |
            FusionChartType.MSCombi2D |
            FusionChartType.MSColumnLine3D |
            FusionChartType.StackedColumn2DLine |
            FusionChartType.StackedColumn3DLine |
            FusionChartType.MSCombiDY2D |
            FusionChartType.MSColumn3DLineDY |
            FusionChartType.StackedColumn3DLineDY |
            FusionChartType.MSStackedColumn2DLineDY
            
    }
}
