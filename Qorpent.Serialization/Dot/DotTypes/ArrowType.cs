using System;

namespace Qorpent.Dot {
	/// <summary>
	/// Типы форм узлов
	/// </summary>
    [Flags]
    public enum ArrowType {

        /// <summary>
        /// Ничего 
        /// </summary>
        None=1,
        /// <summary>
        /// Обычная стрелка
        /// </summary>
        Normal = 1 << 1,
       
		/// <summary>
		/// Черный квадратик
		/// </summary>
        Box=1<<2,
        /// <summary>
        /// Якорь
        /// </summary>
        Curve = 1 << 3,
        /// <summary>
        /// Ромб
        /// </summary>
        Diamond = 1 << 4,
        /// <summary>
        /// Развернутая стрелка
        /// </summary>
        Crow = 1<<5,
        /// <summary>
        /// Точка (кружек)
        /// </summary>
        Dot = 1<<6,
        /// <summary>
        /// Треугольник наоборот 
        /// </summary>
        Inv = 1<<7,
        /// <summary>
        /// Прямоугольник
        /// </summary>
        Tee = 1<<8,
        /// <summary>
        /// Стрелка вогнутая
        /// </summary>
        Vee =1<<9,
        /// <summary>
        /// Все формы
        /// </summary>
        Forms = Inv | Box | Curve | Diamond | Crow | Inv | Tee | Vee |None|Dot,
        /// <summary>Inv | Box | Curve | Diamond | Crow | Inv | Tee | Vee,
        /// Стрелки с поддержкой право-лево
        /// </summary>
        SideAble = Inv | Box | Curve | Diamond | Crow | Inv | Tee | Vee,
        /// <summary>
        /// Стрелки с поддержкой пустоты
        /// </summary>
        EmptyAble = Inv|Box|Diamond|Dot|Inv,
       
        /// <summary>
        /// Оставить левую половину шейпа
        /// </summary>
        Left =1<<10,
        /// <summary>
        /// Оставить правую половину шейпа
        /// </summary>
        Right = 1<<11,
        /// <summary>
        /// Пустой
        /// </summary>
        Empty = 1<<12,

        /// <summary>
        /// 
        /// </summary>
        LNormal = Normal | Left,
        /// <summary>
        /// 
        /// </summary>
        RNormal = Normal | Right,
        /// <summary>
        /// 
        /// </summary>
        ONormal = Normal | Empty,
        /// <summary>
        /// 
        /// </summary>
        RONormal = Normal|Right|Empty,
        /// <summary>
        /// 
        /// </summary>
        LONormal = Normal|Left|Empty,


        /// <summary>
        /// 
        /// </summary>
        LBox = Box | Left,
        /// <summary>
        /// 
        /// </summary>
        RBox = Box | Right,
        /// <summary>
        /// 
        /// </summary>
        OBox = Box | Empty,
        /// <summary>
        /// 
        /// </summary>
        ROBox = Box | Right | Empty,
        /// <summary>
        /// 
        /// </summary>
        LOBox = Box | Left | Empty,

        /// <summary>
        /// 
        /// </summary>
        LDiamond = Diamond | Left,
        /// <summary>
        /// 
        /// </summary>
        RDiamond = Diamond | Right,
        /// <summary>
        /// 
        /// </summary>
        ODiamond = Diamond | Empty,
        /// <summary>
        /// 
        /// </summary>
        RODiamond = Diamond | Right | Empty,
        /// <summary>
        /// 
        /// </summary>
        LODiamond = Diamond | Left | Empty,


        /// <summary>
        /// 
        /// </summary>
        LInv = Inv | Left,
        /// <summary>
        /// 
        /// </summary>
        RInv = Inv | Right,
        /// <summary>
        /// 
        /// </summary>
        OInv = Inv | Empty,
        /// <summary>
        /// 
        /// </summary>
        ROInv = Inv | Right | Empty,
        /// <summary>
        /// 
        /// </summary>
        LOInv = Inv | Left | Empty,



        /// <summary>
        /// 
        /// </summary>
        LTee = Tee | Left,
        /// <summary>
        /// 
        /// </summary>
        RTes = Tee | Right,

        /// <summary>
        /// 
        /// </summary>
        LVee = Vee | Left,
        /// <summary>
        /// 
        /// </summary>
        RVee = Vee | Right,
        /// <summary>
        /// 
        /// </summary>
        LCurve = Curve | Left,
        /// <summary>
        /// 
        /// </summary>
        RCurve = Curve | Right,

        /// <summary>
        /// 
        /// </summary>
        LCrow = Crow | Left,
        /// <summary>
        /// 
        /// </summary>
        RCrow = Crow | Right,
        /// <summary>
        /// 
        /// </summary>
        ODot  =Dot|Empty,
        
	}
}