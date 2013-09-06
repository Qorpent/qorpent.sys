namespace Qorpent.Dot {
	/// <summary>
	/// Типы форм узлов
	/// </summary>
	public enum ArrowType {
		/// <summary>
		/// Черный квадратик
		/// </summary>
        Box,
        /// <summary>
		/// Префикс L  оставляет только часть слева от края от основной формы (здесь левая половинка квадрата). 
		/// </summary>
        LBox,
        /// <summary>
        /// Префикс R  оставляет только часть справа от края от основной формы (здесь правая половинка квадрата). 
        /// </summary>
        RBox,
        /// <summary>
        /// Префикс O  оставляет форму открытой (без заливки) (здесь пустой квадрат)
        /// </summary>
        OBox,
        /// <summary>
        /// Сочетание префиксов O  и L оставляет только пустую часть слева от края от основной формы (здесь пустая левая половинка квадрата)
        /// </summary>
        OLBox,
        /// <summary>
        /// Сочетание префиксов O  и R оставляет только пустую часть справа от края от основной формы (здесь пустая правая половинка квадрата)
        /// </summary>
        ORBox, 
        /// <summary>
        /// Стрелка наоборот
        /// </summary>
        Crow,
        /// <summary>
        /// Левая часть стрелки наоборот
        /// </summary>
        LCrow,
        /// <summary>
        /// Правая часть стрелки наоборот
        /// </summary>
        RCrow,
        /// <summary>
        /// Якорь
        /// </summary>
        Сurve,
        /// <summary>
        /// Левая часть якоря
        /// </summary>
        LСurve,
        /// <summary>
        /// Правая часть якоря 
        /// </summary>
        RСurve,
        /// <summary>
        /// Ромб
        /// </summary>
        Diamond,
        /// <summary>
        /// Левая часть ромба
        /// </summary>
        LDiamond,
        /// <summary>
        /// Правая часть ромба 
        /// </summary>
        RDiamond,
        /// <summary>
        /// Пустой ромб
        /// </summary>
        ODiamond,
        /// <summary>
        /// Пустая левая часть ромба
        /// </summary>
        OLDiamond,
        /// <summary>
        /// Пустая правая часть ромба
        /// </summary>
        ORDiamond, 
        /// <summary>
        /// Точка (кружек)
        /// </summary>
        Dot,
        /// <summary>
        /// Пустая точка (кружек)
        /// </summary>
        ODot,
        /// <summary>
        /// Треугольник наоборот 
        /// </summary>
        Inv,
        /// <summary>
        /// Левая часть треугольника наоборот
        /// </summary>
        LInv,
        /// <summary>
        /// Правая часть треугольника наоборот 
        /// </summary>
        RInv,
        /// <summary>
        /// Пустой треугольник наоборот
        /// </summary>
        OInv,
        /// <summary>
        /// Пустая левая часть треугольника наоборот
        /// </summary>
        OLInv,
        /// <summary>
        /// Пустая правая часть треугольника наоборот
        /// </summary>
        ORInv, 
        /// <summary>
        /// Ничего 
        /// </summary>
        None,
        /// <summary>
        /// Стрелка-треугольник
        /// </summary>
        Normal,
        /// <summary>
        /// Левая часть стрелки-треугольника
        /// </summary>
        LNormal,
        /// <summary>
        /// Правая часть стрелки-треугольника 
        /// </summary>
        RNormal,
        /// <summary>
        /// Пустая стрелка-треугольник
        /// </summary>
        ONormal,
        /// <summary>
        /// Пустая левая часть стрелки-треугольника
        /// </summary>
        OLNormal,
        /// <summary>
        /// Пустая правая часть стрелки-треугольника
        /// </summary>
        ORNormal, 
        /// <summary>
        /// Прямоугольник
        /// </summary>
        Tee,
        /// <summary>
        /// Левая часть прямоугольника
        /// </summary>
        LTee,
        /// <summary>
        /// Правая часть прямоугольника 
        /// </summary>
        RTee,
        /// <summary>
        /// Стрелка обычная
        /// </summary>
        Vee,
        /// <summary>
        /// Левая часть стрелки обычной
        /// </summary>
        LVee,
        /// <summary>
        /// Правая часть стрелки обычной
        /// </summary>
        RVee,
        




	}
}