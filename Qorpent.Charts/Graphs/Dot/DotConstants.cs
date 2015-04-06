namespace Qorpent.Graphs.Dot {
    /// <summary>
    ///     Набор констант дота
    /// </summary>
    public static class DotConstants {
        /// <summary>
        ///     Атрибут заголовка
        /// </summary>
        public const string LabelAttribute = "label";

        /// <summary>
        ///     Атрибут направления разрисовки графа
        /// </summary>
        public const string RankDirAttribute = "rankdir";

        /// <summary>
        ///     Атрибут формы узла
        /// </summary>
        public const string ShapeAttribute = "shape";

        /// <summary>
        ///     Форма конца стрелки
        /// </summary>
        public const string ArrowHeadAttribute = "arrowhead";

        /// <summary>
        ///     Форма начала стрелки
        /// </summary>
        public const string ArrowTailAttribute = "arrowtail";

        /// <summary>
        ///     Если "Да", то присоединение заголовка к стрелке происходит через подчеркивание
        /// </summary>
        public const string DecorateLabelAttribute = "decorate";

        /// <summary>
        ///     Если "Да", то заголовок (если длинный) будет пересекать другие стрелки, если "Нет", то заголовок не пересекает их - стрелки выгнуться
        /// </summary>
        public const string LabelFloatAttribute = "labelfloat";

        /// <summary>
        ///     Стрелка (голова) направляется от узла одного подграфа к другому подграфу (а не к другому узлу другого подграфа). Для этого также у подграфов д.б. compound=true и в значение Lhead нужно подставить название подграфа
        /// </summary>
        public const string LheadAttribute = "lhead";

        /// <summary>
        ///     Стрелка (хвост) направляется от подграфа к узлу другого подграфа. Для этого также у подграфов д.б. compound=true и в значение Ltail нужно подставить название подграфа
        /// </summary>
        public const string LtailAttribute = "ltail";

        /// <summary>
        ///     Стрелки (голова) направляется к одной точке подграфа (или узла), а не разным. Н-р, если A -> B  и C -> B   и в значение SameHead к ним нужно подставить название подграфа где узел B
        /// </summary>
        public const string SameHeadAttribute = "samehead";

        /// <summary>
        ///     Стрелки (хвост) отходят от одной точки подграфа (или узла), а не от разных. Н-р, если A -> B  и A -> C   и в значение SameTail к ним нужно подставить название подграфа где узел A
        /// </summary>
        public const string SameTailAttribute = "sametail";

        /// <summary>
        ///     Если "Да", то расположение графа центировано, а если "Нет", то не центировано
        /// </summary>
        public const string CenterAttribute = "center";

        /// <summary>
        ///     Если "Да", то позволяет стрелкам идти между подграфами (См. Lhead Ltail )
        /// </summary>
        public const string CompoundAttribute = "compound";

        /// <summary>
        ///     Определеяет на каких концах ребра должны быть стрелки (или не быть). Фактически же стиль стрелки можно задать с помощью ArrowTail и ArrowHead.
        /// </summary>
        public const string DirAttribute = "dir";

        /// <summary>
        ///     Сконцентрированное расположение узлов и подграфов графа
        /// </summary>
        public const string ConcentrateAttribute = "сoncentrate";

        /// <summary>
        ///     Имя у шрифта
        /// </summary>
        public const string FontNameAttribute = "fontname";

        /// <summary>
        ///     Имя у шрифта
        /// </summary>
        public const string DefaultFontNameAttribute = "Times-Roman";

        /// <summary>
        ///     Не нужный атрибут
        /// </summary>
        public const string FontNamesAttribute = "fontnames";

        /// <summary>
        ///     Если "Да", то график отображается в ландшафтном режиме, т.е. переварачивается на 90 градусов
        /// </summary>
        public const string LandscapeAttribute = "landscape";

        /// <summary>
        ///     Для graph и node
        /// </summary>
        public const string OrientationAttribute = "orientation";

        /// <summary>
        ///     Квантум увеличивает размеры узла графа на указанное значение. Н-р, quantum=0.5
        /// </summary>
        public const string QuantumAttribute = "quantum";

        /// <summary>
        ///     Если "Да" и есть несколько графов, то ?????
        /// </summary>
        public const string RemincrossAttribute = "remincross";

        /// <summary>
        ///     Если установить значение 90, то произойдет переориентации графа в альбомную страницу
        /// </summary>
        public const string RotateAttribute = "rotate";

        /// <summary>
        ///     Задает ширину линии (стрелки, узла, кластера...) в точках
        /// </summary>
        public const string PenwidthAttribute = "penwidth";

        /// <summary>
        ///     Коэффициент для shape=polygon задающий при положительном значении верхнюю часть больше, чем нижнюю и при отрицательном - наоборот
        /// </summary>
        public const string DistortionAttribute = "distortion";

        /// <summary>
        ///     Если "Да", то размер узла определяется значениями ширины и высоты - Width and Height
        /// </summary>
        public const string FixedSizeAttribute = "fixedsize";

        /// <summary>
        ///     Задает высоту узла. По умолчанию 0.5,  min значение 0.02
        /// </summary>
        public const string HeightAttribute = "height";

        /// <summary>
        ///     Задает ширину узла. По умолчанию 0.75,  min значение 0.01
        /// </summary>
        public const string WidthAttribute = "width";

        /// <summary>
        ///     Для Shape=polygon задает косые. Положительные значения - верхняя части полигона направо, отрицательные  - налево. По умолчанию 0.0
        /// </summary>
        public const string SkewAttribute = "skew";

        /// <summary>
        ///     Задает колличество рамочек вокруг узла той же формы что и форма узла
        /// </summary>
        public const string PeripheriesAttribute = "peripheries";

        /// <summary>
        ///     Размер шрифта, по умолчанию 14
        /// </summary>
        public const string FontSizeAttribute = "fontsize";

        /// <summary>
        ///     Цвет используемый для текста. По умолчанию черный
        /// </summary>
        public const string FontColorAttribute = "fontcolor";

        /// <summary>
        ///     Задает количество сторон для полигона
        /// </summary>
        public const string SidesAttribute = "sides";

        /// <summary>
        ///     Задает тип разрисовки линий графа (дуги, ломанные и т.п.)
        /// </summary>
        public const string SplinesAttribute = "splines";

        /// <summary>
        ///     Задает цвет границы подграфов. По умолчанию черный
        /// </summary>
        public const string PenColorAttribute = "pencolor";

        /// <summary>
        ///     Задает размер наконечника (головы) стрелки. По умолчанию 1
        /// </summary>
        public const string ArrowSizeAttribute = "arrowsize";

        /// <summary>
        ///     Внешняя метка для узла или края. Для узлов, названия будут помещаться вне узла, но рядом с ним и для ребра - вблизи центра ребра.
        /// </summary>
        public const string XLabeAttribute = "xlabe";

        /// <summary>
        ///     Минимальная длина ребра в рангах (разница между головой и хвостом). По умолчанию 1, минимальная 0
        /// </summary>
        public const string MinlenAttribute = "minlen";

        /// <summary>
        ///     Атрибут стиля
        /// </summary>
        public const string StyleAttribute = "style";

        /// <summary>
        ///     Цвет заливки
        /// </summary>
        public const string FillColorAttribute = "fillcolor";

        /// <summary>
        ///     Идентификатор
        /// </summary>
        public const string IdAttribute = "id";

        /// <summary>
        ///    Расстояние между рангами
        /// </summary>
        public const string RankSepAttribute = "ranksep";
        /// <summary>
        ///    Зазоры при оверлапе
        /// </summary>
        public const string SepAttribute = "sep";
        /// <summary>
        ///    Расстояния между ребрами
        /// </summary>
        public const string ESepAttribute = "esep";
        /// <summary>
        ///    Расстояния между узлами
        /// </summary>
        public const string NodeSepAttribute = "nodesep";
        /// <summary>
        /// Цветовая схема
        /// </summary>
        public const string ColorSchemeAttribute = "colorscheme";
        /// <summary>
        /// Цвет линии
        /// </summary>
        public const string ColorAttribute = "color";
        /// <summary>
        /// Подсказка
        /// </summary>
        public const string TooltipAttribute = "tooltip";
        /// <summary>
        /// Целевой фрейм
        /// </summary>
        public const string TargetAttribute = "target";
        /// <summary>
        /// Ссылка
        /// </summary>
        public const string HrefAttribute = "href";
        /// <summary>
        /// Флаг, указывающий на то, что требуется просчёт сложности графа
        /// </summary>
        public const string CalculateEdgeInWeight = "CalculateEdgeInWeight";
    }
}