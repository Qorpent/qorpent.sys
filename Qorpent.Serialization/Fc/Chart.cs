using Qorpent.Fc.ChartTypes;
using Qorpent.Model;
using Qorpent.Serialization;

namespace Qorpent.Fc {
    /// <summary>
    /// Константы графиков
    /// </summary>
    public static class ChartConstants {

        /// <summary>
        /// Аттрибут заголовка
        /// </summary>
        public const string CaptionAttribute = "caption";

        /// <summary>
        /// Аттрибут подзаголовка
        /// </summary>
        public const string SubCaptionAttribute = "subCaption";

        /// <summary>
        /// Аттрибут названия оси Х
        /// </summary>
        public const string XAxisName = "xAxisName";

        /// <summary>
        /// Аттрибут названия оси Y
        /// </summary>
        public const string YAxisName = "yAxisName";

        /// <summary>
        /// Атрибут цветовой палитры уровня данных
        /// </summary>
        public const string PaleteColorsAttribute = "paleteColors";

        /// <summary>
        /// Аттрибут использования анимации в графике
        /// </summary>
        public const string AnimationAttribute = "animation";

        /// <summary>
        /// Аттрибут цветовой схемы графика 
        /// </summary>
        public const string PalleteAttribute = "palette";

        /// <summary>
        /// Аттрибут скрытия/отображения подписей данных
        /// </summary>
        public const string ShowLabelsAttribute = "showLabels";

        /// <summary>
        /// Аттрибут скрытия/отображения данных
        /// </summary>
        public const string ShowValuesAttribute = "showValues";

        /// <summary>
        /// Аттрибут поворота подписей
        /// </summary>
        public const string RotateLabelsAttribute = "rotateLabels";

        /// <summary>
        /// Аттрибут поворота данных
        /// </summary>
        public const string RotateValuesAttribute = "rotateValues";

        /// <summary>
        /// Аттрибут позиционирования значений внутри колонок (только для колоночного типа графика)
        /// </summary>
        public const string PlaceValuesInsideAttribute = "placeValuesInside";

        /// <summary>
        /// Аттрибут адаптирования линий сетки
        /// </summary>
        public const string AdjustDivAttribute = "adjustDiv";

        /// <summary>
        /// Аттрибут отображения/скрытия значений вертикальной линии сетки
        /// </summary>
        public const string ShowYAxisValuesAttribute = "showYAxisValues";

        /// <summary>
        /// Аттрибут типа отображения подписей данных
        /// </summary>
        public const string LabelDisplayAttribute = "labelDisplay";
    }

    /// <summary>
    /// FusionChart chart element
    /// </summary>
    public class Chart:AttributedEntity {
        /// <summary>
        /// Control animation in chart
        /// </summary>
        [IgnoreSerialize]
        public bool Animation {
            get { return Get<bool>(ChartConstants.AnimationAttribute); }
            set {
                Set(ChartConstants.AnimationAttribute, value);
            }
        }

        /// <summary>
        /// Set color pallete in Chart 
        /// </summary>
        public int Palette { 
            get { return _pallete; }
            set { _pallete = value > 5 ? 5 : value; }
        }

        /// <summary>
        /// Устанавливает аттрибут типа отображения подписей
        /// </summary>
        [IgnoreSerialize]
        public LabelDisplayType LabelDisplayType {
            get { return Get<LabelDisplayType>(ChartConstants.LabelDisplayAttribute); }
            set { Set(ChartConstants.LabelDisplayAttribute, value); }   
        }

        /// <summary>
        /// Set colors for the data items
        /// </summary>
        [IgnoreSerialize]
        public string[] PalleteColors {
            get { return Get<string[]>(ChartConstants.PaleteColorsAttribute); }
            set {Set(ChartConstants.PaleteColorsAttribute,value);}
        }

        /// <summary>
        /// Sets the configuration whether the x-axis labels will be displayed or not
        /// </summary>
        public bool ShowLabels { get; set; }

        /// <summary>
        /// Rotate labels on the chart
        /// </summary>
        public bool RotateLabels { get; set; }

        /// <summary>
        /// Show or hide values
        /// </summary>
        public bool ShowValues { get; set; }

        /// <summary>
        /// Rotate values on the chart
        /// </summary>
        public bool RotateValues { get; set; }

        /// <summary>
        /// Added "..." to long data labels
        /// </summary>
        public bool UseEllipsesWhenOverflow { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool PlaceValuesInside { get; set; }

        private int _pallete = 0;
    }
}
