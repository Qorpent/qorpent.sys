using System;

namespace Qorpent.Utils {
    /// <summary>
    ///     Представляет HEX-число
    /// </summary>
    public class Hex {
        /// <summary>
        ///     Значение
        /// </summary>
        public Int32 Value { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Hex() {}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hex"></param>
        public Hex(string hex) {
            Value = Convert.ToInt32(hex, 16);
        }
        /// <summary>
        ///     Приведение к <see cref="int"/>
        /// </summary>
        /// <returns>Результат приведения</returns>
        public int ToInt32() {
            return Value;
        }
        /// <summary>
        /// Приведение к HEX
        /// </summary>
        /// <returns>Результат приведения</returns>
        public string ToHex() {
            return String.Format("{0:X}", Value);
        }
        /// <summary>
        ///     Приведение к строке
        /// </summary>
        /// <param name="hex">Представление числа</param>
        /// <returns>Результат приведения</returns>
        public static implicit operator string(Hex hex) {
            return hex.ToHex();
        }
        /// <summary>
        ///     Приведение строки к представлению
        /// </summary>
        /// <param name="hex">Представление числа</param>
        /// <returns>Результат приведения</returns>
        public static implicit operator Hex(string hex) {
            return new Hex(hex);
        }
        /// <summary>
        ///     Операция вычитания
        /// </summary>
        /// <param name="h1">Первый операнд</param>
        /// <param name="h2">Второй операнд</param>
        /// <returns>Результат вычитания</returns>
        public static Hex operator -(Hex h1, Hex h2) {
            return new Hex { Value = h1.Value - h2.Value };
        }
        /// <summary>
        ///     Операция сложения
        /// </summary>
        /// <param name="h1">Первый операнд</param>
        /// <param name="h2">Второй операнд</param>
        /// <returns>Результат сложения</returns>
        public static Hex operator +(Hex h1, Hex h2) {
            return new Hex { Value = h1.Value + h2.Value };
        }
        /// <summary>
        ///     Операция умножения
        /// </summary>
        /// <param name="h1">Первый операнд</param>
        /// <param name="h2">Второй операнд</param>
        /// <returns>Результат умножения</returns>
        public static Hex operator *(Hex h1, Hex h2) {
            return new Hex { Value = h1.Value + h2.Value };
        }
        /// <summary>
        ///     Операция деления
        /// </summary>
        /// <param name="h1">Первый операнд</param>
        /// <param name="h2">Второй операнд</param>
        /// <returns>Результат деления</returns>
        public static Hex operator /(Hex h1, Hex h2) {
            return new Hex { Value = h1.Value + h2.Value };
        }
    }
}
