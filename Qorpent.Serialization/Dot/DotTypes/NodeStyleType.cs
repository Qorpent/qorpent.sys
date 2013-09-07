using System;

namespace Qorpent.Dot {
	/// <summary>
	/// Стили узлов
	/// </summary>
	[Flags]
	public enum NodeStyleType {
        /// <summary>
        /// Отсутствие стиля
        /// </summary>
        None =0,
		/// <summary>
		/// Сплошная граница узла
		/// </summary>
        Solid =1,
        /// <summary>
        /// Пунктирная граница узла 
		/// </summary>
        Dashed=1<<1,
        /// <summary>
        /// Точечная граница узла 
        /// </summary>
        Dotted=1<<2,
        /// <summary>
        /// Сплошная жирная граница узла
        /// </summary>
        Bold =1<<3,
        /// <summary>
        /// Округляет углы фигуры узла
        /// </summary>
        Rounded=1<<4,
        /// <summary>
        /// Диагональные черты внутри узла
        /// </summary>
        Diagonals=1<<5, 
        /// <summary>
        /// Заполнение тела  узла цветом (серым по умолчанию)
        /// </summary>
        Filled =1<<6,
        /// <summary>
        /// На три сектора разбит круг с разными цветами, но у меня не воспроизводится....????
        /// </summary>
        Wedged =1<<7,
        /// <summary>
        /// На три сектора разбит прямоугольник с разными цветами, но у меня не воспроизводится....????
        /// </summary>
        Striped =1<<8
	}
}