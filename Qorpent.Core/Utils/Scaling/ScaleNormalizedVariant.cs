namespace Qorpent.Utils.Scaling {
    /// <summary>
    ///     Класс, представляющий вариант нормализации шкалы
    /// </summary>
    public class ScaleNormalizedVariant {
        /// <summary>
        ///     Максимальное зачение шкалы
        /// </summary>
        public double Maximal { get; set; }
        /// <summary>
        ///     Минимальное значение шкалы
        /// </summary>
        public double Minimal { get; set; }
        /// <summary>
        ///     Количество дивлайнов
        /// </summary>
        public int Divline { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Similar(ScaleNormalizedVariant obj) {
            return Maximal.Equals(obj.Maximal) && Minimal.Equals(obj.Minimal) && Divline.Equals(obj.Divline);
        }
        /// <summary>
        /// 
        /// </summary>
        public double DivSize {
            get { return (Maximal - Minimal)/(Divline + 1); }
        }
        /// <summary>
        /// 
        /// </summary>
        public void DoubleDivlines() {
            Divline = Divline*2 + 1;
        }
    }
}