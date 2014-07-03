using System;
using System.Linq;

namespace Qorpent.Graphs.Dot.Types {
    /// <summary>
    /// Описывает цвет
    /// </summary>
    public struct Color {
       /// <summary>
       /// 
       /// </summary>
       /// <param name="nativeString"></param>
        public Color(string nativeString) : this() {
            NativeString = nativeString;
        }
	

        /// <summary>
        /// Определение цвета в DOT
        /// </summary>
        public string NativeString { get;private set; }

        /// <summary>
        /// Конвертирует отдельные цвета в атрибуты типа Single
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static implicit operator ColorAttribute(Color color) {
            return ColorAttribute.Single(color);
        }

        /// <summary>
        /// Позволяет складывать цвета с числами получая 
        /// </summary>
        /// <param name="color"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static ColorListItem operator +(Color color, decimal weight) {
            return new ColorListItem{Color = color,Weight = weight};
        }
        /// <summary>
        /// Позволяет складывать цвета с числами получая 
        /// </summary>
        /// <param name="color"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static ColorListItem operator +(Color color, double weight)
        {
            return new ColorListItem { Color = color, Weight = Convert.ToDecimal(weight) };
        }
        /// <summary>
        /// Формирует RGB цвет
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Color RGB(byte r, byte g, byte b, byte a = 0) {
            var str = "#" + r.ToString("x2") + g.ToString("x2") + b.ToString("x2");
            if (0 != a) {
                str += a.ToString("x2");
            }
            return new Color(str);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="def"></param>
        /// <returns></returns>
        public static Color Create(string def) {
            
            if (def.Length==6 &&def.All(_ => (_ >= '0' && _ <= '9') || (_ >= 'A' && _ <= 'F') || (_ >= 'a' && _ <= 'f'))) {
                return new Color("#"+def);
            }

            return new Color(def);
        }

       
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return NativeString;
        }

        ///<summary />
        public const string COLOR_ALICEBLUE = "aliceblue";

        ///<summary />
        public static readonly Color Aliceblue = Create(COLOR_ALICEBLUE);


        ///<summary />
        public const string COLOR_BEIGE = "beige";

        ///<summary />
        public static readonly Color Beige = Create(COLOR_BEIGE);


        ///<summary />
        public const string COLOR_BLUEVIOLET = "blueviolet";

        ///<summary />
        public static readonly Color Blueviolet = Create(COLOR_BLUEVIOLET);


        ///<summary />
        public const string COLOR_CHOCOLATE = "chocolate";

        ///<summary />
        public static readonly Color Chocolate = Create(COLOR_CHOCOLATE);


        ///<summary />
        public const string COLOR_CYAN = "cyan";

        ///<summary />
        public static readonly Color Cyan = Create(COLOR_CYAN);


        ///<summary />
        public const string COLOR_DARKGREEN = "darkgreen";

        ///<summary />
        public static readonly Color Darkgreen = Create(COLOR_DARKGREEN);


        ///<summary />
        public const string COLOR_DARKORANGE = "darkorange";

        ///<summary />
        public static readonly Color Darkorange = Create(COLOR_DARKORANGE);


        ///<summary />
        public const string COLOR_DARKSLATEBLUE = "darkslateblue";

        ///<summary />
        public static readonly Color Darkslateblue = Create(COLOR_DARKSLATEBLUE);


        ///<summary />
        public const string COLOR_DEEPPINK = "deeppink";

        ///<summary />
        public static readonly Color Deeppink = Create(COLOR_DEEPPINK);


        ///<summary />
        public const string COLOR_FIREBRICK = "firebrick";

        ///<summary />
        public static readonly Color Firebrick = Create(COLOR_FIREBRICK);


        ///<summary />
        public const string COLOR_GHOSTWHITE = "ghostwhite";

        ///<summary />
        public static readonly Color Ghostwhite = Create(COLOR_GHOSTWHITE);


        ///<summary />
        public const string COLOR_GREEN = "green";

        ///<summary />
        public static readonly Color Green = Create(COLOR_GREEN);


        ///<summary />
        public const string COLOR_INDIGO = "indigo";

        ///<summary />
        public static readonly Color Indigo = Create(COLOR_INDIGO);


        ///<summary />
        public const string COLOR_LAWNGREEN = "lawngreen";

        ///<summary />
        public static readonly Color Lawngreen = Create(COLOR_LAWNGREEN);


        ///<summary />
        public const string COLOR_LIGHTGOLDENRODYELLOW = "lightgoldenrodyellow";

        ///<summary />
        public static readonly Color Lightgoldenrodyellow = Create(COLOR_LIGHTGOLDENRODYELLOW);


        ///<summary />
        public const string COLOR_LIGHTSALMON = "lightsalmon";

        ///<summary />
        public static readonly Color Lightsalmon = Create(COLOR_LIGHTSALMON);


        ///<summary />
        public const string COLOR_LIGHTSTEELBLUE = "lightsteelblue";

        ///<summary />
        public static readonly Color Lightsteelblue = Create(COLOR_LIGHTSTEELBLUE);


        ///<summary />
        public const string COLOR_MAGENTA = "magenta";

        ///<summary />
        public static readonly Color Magenta = Create(COLOR_MAGENTA);


        ///<summary />
        public const string COLOR_MEDIUMPURPLE = "mediumpurple";

        ///<summary />
        public static readonly Color Mediumpurple = Create(COLOR_MEDIUMPURPLE);


        ///<summary />
        public const string COLOR_MEDIUMVIOLETRED = "mediumvioletred";

        ///<summary />
        public static readonly Color Mediumvioletred = Create(COLOR_MEDIUMVIOLETRED);


        ///<summary />
        public const string COLOR_NAVAJOWHITE = "navajowhite";

        ///<summary />
        public static readonly Color Navajowhite = Create(COLOR_NAVAJOWHITE);


        ///<summary />
        public const string COLOR_ORANGE = "orange";

        ///<summary />
        public static readonly Color Orange = Create(COLOR_ORANGE);


        ///<summary />
        public const string COLOR_PALETURQUOISE = "paleturquoise";

        ///<summary />
        public static readonly Color Paleturquoise = Create(COLOR_PALETURQUOISE);


        ///<summary />
        public const string COLOR_PINK = "pink";

        ///<summary />
        public static readonly Color Pink = Create(COLOR_PINK);


        ///<summary />
        public const string COLOR_ROSYBROWN = "rosybrown";

        ///<summary />
        public static readonly Color Rosybrown = Create(COLOR_ROSYBROWN);


        ///<summary />
        public const string COLOR_SEAGREEN = "seagreen";

        ///<summary />
        public static readonly Color Seagreen = Create(COLOR_SEAGREEN);


        ///<summary />
        public const string COLOR_SLATEBLUE = "slateblue";

        ///<summary />
        public static readonly Color Slateblue = Create(COLOR_SLATEBLUE);


        ///<summary />
        public const string COLOR_STEELBLUE = "steelblue";

        ///<summary />
        public static readonly Color Steelblue = Create(COLOR_STEELBLUE);


        ///<summary />
        public const string COLOR_TURQUOISE = "turquoise";

        ///<summary />
        public static readonly Color Turquoise = Create(COLOR_TURQUOISE);


        ///<summary />
        public const string COLOR_YELLOW = "yellow";

        ///<summary />
        public static readonly Color Yellow = Create(COLOR_YELLOW);


        ///<summary />
        public const string COLOR_ANTIQUEWHITE = "antiquewhite";

        ///<summary />
        public static readonly Color Antiquewhite = Create(COLOR_ANTIQUEWHITE);


        ///<summary />
        public const string COLOR_BISQUE = "bisque";

        ///<summary />
        public static readonly Color Bisque = Create(COLOR_BISQUE);


        ///<summary />
        public const string COLOR_BROWN = "brown";

        ///<summary />
        public static readonly Color Brown = Create(COLOR_BROWN);


        ///<summary />
        public const string COLOR_CORAL = "coral";

        ///<summary />
        public static readonly Color Coral = Create(COLOR_CORAL);


        ///<summary />
        public const string COLOR_DARKBLUE = "darkblue";

        ///<summary />
        public static readonly Color Darkblue = Create(COLOR_DARKBLUE);


        ///<summary />
        public const string COLOR_DARKGREY = "darkgrey";

        ///<summary />
        public static readonly Color Darkgrey = Create(COLOR_DARKGREY);


        ///<summary />
        public const string COLOR_DARKORCHID = "darkorchid";

        ///<summary />
        public static readonly Color Darkorchid = Create(COLOR_DARKORCHID);


        ///<summary />
        public const string COLOR_DARKSLATEGRAY = "darkslategray";

        ///<summary />
        public static readonly Color Darkslategray = Create(COLOR_DARKSLATEGRAY);


        ///<summary />
        public const string COLOR_DEEPSKYBLUE = "deepskyblue";

        ///<summary />
        public static readonly Color Deepskyblue = Create(COLOR_DEEPSKYBLUE);


        ///<summary />
        public const string COLOR_FLORALWHITE = "floralwhite";

        ///<summary />
        public static readonly Color Floralwhite = Create(COLOR_FLORALWHITE);


        ///<summary />
        public const string COLOR_GOLD = "gold";

        ///<summary />
        public static readonly Color Gold = Create(COLOR_GOLD);


        ///<summary />
        public const string COLOR_GREENYELLOW = "greenyellow";

        ///<summary />
        public static readonly Color Greenyellow = Create(COLOR_GREENYELLOW);


        ///<summary />
        public const string COLOR_IVORY = "ivory";

        ///<summary />
        public static readonly Color Ivory = Create(COLOR_IVORY);


        ///<summary />
        public const string COLOR_LEMONCHIFFON = "lemonchiffon";

        ///<summary />
        public static readonly Color Lemonchiffon = Create(COLOR_LEMONCHIFFON);


        ///<summary />
        public const string COLOR_LIGHTGRAY = "lightgray";

        ///<summary />
        public static readonly Color Lightgray = Create(COLOR_LIGHTGRAY);


        ///<summary />
        public const string COLOR_LIGHTSEAGREEN = "lightseagreen";

        ///<summary />
        public static readonly Color Lightseagreen = Create(COLOR_LIGHTSEAGREEN);


        ///<summary />
        public const string COLOR_LIGHTYELLOW = "lightyellow";

        ///<summary />
        public static readonly Color Lightyellow = Create(COLOR_LIGHTYELLOW);


        ///<summary />
        public const string COLOR_MAROON = "maroon";

        ///<summary />
        public static readonly Color Maroon = Create(COLOR_MAROON);


        ///<summary />
        public const string COLOR_MEDIUMSEAGREEN = "mediumseagreen";

        ///<summary />
        public static readonly Color Mediumseagreen = Create(COLOR_MEDIUMSEAGREEN);


        ///<summary />
        public const string COLOR_MIDNIGHTBLUE = "midnightblue";

        ///<summary />
        public static readonly Color Midnightblue = Create(COLOR_MIDNIGHTBLUE);


        ///<summary />
        public const string COLOR_NAVY = "navy";

        ///<summary />
        public static readonly Color Navy = Create(COLOR_NAVY);


        ///<summary />
        public const string COLOR_ORANGERED = "orangered";

        ///<summary />
        public static readonly Color Orangered = Create(COLOR_ORANGERED);


        ///<summary />
        public const string COLOR_PALEVIOLETRED = "palevioletred";

        ///<summary />
        public static readonly Color Palevioletred = Create(COLOR_PALEVIOLETRED);


        ///<summary />
        public const string COLOR_PLUM = "plum";

        ///<summary />
        public static readonly Color Plum = Create(COLOR_PLUM);


        ///<summary />
        public const string COLOR_ROYALBLUE = "royalblue";

        ///<summary />
        public static readonly Color Royalblue = Create(COLOR_ROYALBLUE);


        ///<summary />
        public const string COLOR_SEASHELL = "seashell";

        ///<summary />
        public static readonly Color Seashell = Create(COLOR_SEASHELL);


        ///<summary />
        public const string COLOR_SLATEGRAY = "slategray";

        ///<summary />
        public static readonly Color Slategray = Create(COLOR_SLATEGRAY);


        ///<summary />
        public const string COLOR_TAN = "tan";

        ///<summary />
        public static readonly Color Tan = Create(COLOR_TAN);


        ///<summary />
        public const string COLOR_VIOLET = "violet";

        ///<summary />
        public static readonly Color Violet = Create(COLOR_VIOLET);


        ///<summary />
        public const string COLOR_YELLOWGREEN = "yellowgreen";

        ///<summary />
        public static readonly Color Yellowgreen = Create(COLOR_YELLOWGREEN);


        ///<summary />
        public const string COLOR_AQUA = "aqua";

        ///<summary />
        public static readonly Color Aqua = Create(COLOR_AQUA);


        ///<summary />
        public const string COLOR_BLACK = "black";

        ///<summary />
        public static readonly Color Black = Create(COLOR_BLACK);


        ///<summary />
        public const string COLOR_BURLYWOOD = "burlywood";

        ///<summary />
        public static readonly Color Burlywood = Create(COLOR_BURLYWOOD);


        ///<summary />
        public const string COLOR_CORNFLOWERBLUE = "cornflowerblue";

        ///<summary />
        public static readonly Color Cornflowerblue = Create(COLOR_CORNFLOWERBLUE);


        ///<summary />
        public const string COLOR_DARKCYAN = "darkcyan";

        ///<summary />
        public static readonly Color Darkcyan = Create(COLOR_DARKCYAN);


        ///<summary />
        public const string COLOR_DARKKHAKI = "darkkhaki";

        ///<summary />
        public static readonly Color Darkkhaki = Create(COLOR_DARKKHAKI);


        ///<summary />
        public const string COLOR_DARKRED = "darkred";

        ///<summary />
        public static readonly Color Darkred = Create(COLOR_DARKRED);


        ///<summary />
        public const string COLOR_DARKSLATEGREY = "darkslategrey";

        ///<summary />
        public static readonly Color Darkslategrey = Create(COLOR_DARKSLATEGREY);


        ///<summary />
        public const string COLOR_DIMGRAY = "dimgray";

        ///<summary />
        public static readonly Color Dimgray = Create(COLOR_DIMGRAY);


        ///<summary />
        public const string COLOR_FORESTGREEN = "forestgreen";

        ///<summary />
        public static readonly Color Forestgreen = Create(COLOR_FORESTGREEN);


        ///<summary />
        public const string COLOR_GOLDENROD = "goldenrod";

        ///<summary />
        public static readonly Color Goldenrod = Create(COLOR_GOLDENROD);


        ///<summary />
        public const string COLOR_HONEYDEW = "honeydew";

        ///<summary />
        public static readonly Color Honeydew = Create(COLOR_HONEYDEW);


        ///<summary />
        public const string COLOR_KHAKI = "khaki";

        ///<summary />
        public static readonly Color Khaki = Create(COLOR_KHAKI);


        ///<summary />
        public const string COLOR_LIGHTBLUE = "lightblue";

        ///<summary />
        public static readonly Color Lightblue = Create(COLOR_LIGHTBLUE);


        ///<summary />
        public const string COLOR_LIGHTGREEN = "lightgreen";

        ///<summary />
        public static readonly Color Lightgreen = Create(COLOR_LIGHTGREEN);


        ///<summary />
        public const string COLOR_LIGHTSKYBLUE = "lightskyblue";

        ///<summary />
        public static readonly Color Lightskyblue = Create(COLOR_LIGHTSKYBLUE);


        ///<summary />
        public const string COLOR_LIME = "lime";

        ///<summary />
        public static readonly Color Lime = Create(COLOR_LIME);


        ///<summary />
        public const string COLOR_MEDIUMAQUAMARINE = "mediumaquamarine";

        ///<summary />
        public static readonly Color Mediumaquamarine = Create(COLOR_MEDIUMAQUAMARINE);


        ///<summary />
        public const string COLOR_MEDIUMSLATEBLUE = "mediumslateblue";

        ///<summary />
        public static readonly Color Mediumslateblue = Create(COLOR_MEDIUMSLATEBLUE);


        ///<summary />
        public const string COLOR_MINTCREAM = "mintcream";

        ///<summary />
        public static readonly Color Mintcream = Create(COLOR_MINTCREAM);


        ///<summary />
        public const string COLOR_OLDLACE = "oldlace";

        ///<summary />
        public static readonly Color Oldlace = Create(COLOR_OLDLACE);


        ///<summary />
        public const string COLOR_ORCHID = "orchid";

        ///<summary />
        public static readonly Color Orchid = Create(COLOR_ORCHID);


        ///<summary />
        public const string COLOR_PAPAYAWHIP = "papayawhip";

        ///<summary />
        public static readonly Color Papayawhip = Create(COLOR_PAPAYAWHIP);


        ///<summary />
        public const string COLOR_POWDERBLUE = "powderblue";

        ///<summary />
        public static readonly Color Powderblue = Create(COLOR_POWDERBLUE);


        ///<summary />
        public const string COLOR_SADDLEBROWN = "saddlebrown";

        ///<summary />
        public static readonly Color Saddlebrown = Create(COLOR_SADDLEBROWN);


        ///<summary />
        public const string COLOR_SIENNA = "sienna";

        ///<summary />
        public static readonly Color Sienna = Create(COLOR_SIENNA);


        ///<summary />
        public const string COLOR_SLATEGREY = "slategrey";

        ///<summary />
        public static readonly Color Slategrey = Create(COLOR_SLATEGREY);


        ///<summary />
        public const string COLOR_TEAL = "teal";

        ///<summary />
        public static readonly Color Teal = Create(COLOR_TEAL);


        ///<summary />
        public const string COLOR_WHEAT = "wheat";

        ///<summary />
        public static readonly Color Wheat = Create(COLOR_WHEAT);


        ///<summary />
        public const string COLOR_AQUAMARINE = "aquamarine";

        ///<summary />
        public static readonly Color Aquamarine = Create(COLOR_AQUAMARINE);


        ///<summary />
        public const string COLOR_BLANCHEDALMOND = "blanchedalmond";

        ///<summary />
        public static readonly Color Blanchedalmond = Create(COLOR_BLANCHEDALMOND);


        ///<summary />
        public const string COLOR_CADETBLUE = "cadetblue";

        ///<summary />
        public static readonly Color Cadetblue = Create(COLOR_CADETBLUE);


        ///<summary />
        public const string COLOR_CORNSILK = "cornsilk";

        ///<summary />
        public static readonly Color Cornsilk = Create(COLOR_CORNSILK);


        ///<summary />
        public const string COLOR_DARKGOLDENROD = "darkgoldenrod";

        ///<summary />
        public static readonly Color Darkgoldenrod = Create(COLOR_DARKGOLDENROD);


        ///<summary />
        public const string COLOR_DARKMAGENTA = "darkmagenta";

        ///<summary />
        public static readonly Color Darkmagenta = Create(COLOR_DARKMAGENTA);


        ///<summary />
        public const string COLOR_DARKSALMON = "darksalmon";

        ///<summary />
        public static readonly Color Darksalmon = Create(COLOR_DARKSALMON);


        ///<summary />
        public const string COLOR_DARKTURQUOISE = "darkturquoise";

        ///<summary />
        public static readonly Color Darkturquoise = Create(COLOR_DARKTURQUOISE);


        ///<summary />
        public const string COLOR_DIMGREY = "dimgrey";

        ///<summary />
        public static readonly Color Dimgrey = Create(COLOR_DIMGREY);


        ///<summary />
        public const string COLOR_FUCHSIA = "fuchsia";

        ///<summary />
        public static readonly Color Fuchsia = Create(COLOR_FUCHSIA);


        ///<summary />
        public const string COLOR_GRAY = "gray";

        ///<summary />
        public static readonly Color Gray = Create(COLOR_GRAY);


        ///<summary />
        public const string COLOR_HOTPINK = "hotpink";

        ///<summary />
        public static readonly Color Hotpink = Create(COLOR_HOTPINK);


        ///<summary />
        public const string COLOR_LAVENDER = "lavender";

        ///<summary />
        public static readonly Color Lavender = Create(COLOR_LAVENDER);


        ///<summary />
        public const string COLOR_LIGHTCORAL = "lightcoral";

        ///<summary />
        public static readonly Color Lightcoral = Create(COLOR_LIGHTCORAL);


        ///<summary />
        public const string COLOR_LIGHTGREY = "lightgrey";

        ///<summary />
        public static readonly Color Lightgrey = Create(COLOR_LIGHTGREY);


        ///<summary />
        public const string COLOR_LIGHTSLATEGRAY = "lightslategray";

        ///<summary />
        public static readonly Color Lightslategray = Create(COLOR_LIGHTSLATEGRAY);


        ///<summary />
        public const string COLOR_LIMEGREEN = "limegreen";

        ///<summary />
        public static readonly Color Limegreen = Create(COLOR_LIMEGREEN);


        ///<summary />
        public const string COLOR_MEDIUMBLUE = "mediumblue";

        ///<summary />
        public static readonly Color Mediumblue = Create(COLOR_MEDIUMBLUE);


        ///<summary />
        public const string COLOR_MEDIUMSPRINGGREEN = "mediumspringgreen";

        ///<summary />
        public static readonly Color Mediumspringgreen = Create(COLOR_MEDIUMSPRINGGREEN);


        ///<summary />
        public const string COLOR_MISTYROSE = "mistyrose";

        ///<summary />
        public static readonly Color Mistyrose = Create(COLOR_MISTYROSE);


        ///<summary />
        public const string COLOR_OLIVE = "olive";

        ///<summary />
        public static readonly Color Olive = Create(COLOR_OLIVE);


        ///<summary />
        public const string COLOR_PALEGOLDENROD = "palegoldenrod";

        ///<summary />
        public static readonly Color Palegoldenrod = Create(COLOR_PALEGOLDENROD);


        ///<summary />
        public const string COLOR_PEACHPUFF = "peachpuff";

        ///<summary />
        public static readonly Color Peachpuff = Create(COLOR_PEACHPUFF);


        ///<summary />
        public const string COLOR_PURPLE = "purple";

        ///<summary />
        public static readonly Color Purple = Create(COLOR_PURPLE);


        ///<summary />
        public const string COLOR_SALMON = "salmon";

        ///<summary />
        public static readonly Color Salmon = Create(COLOR_SALMON);


        ///<summary />
        public const string COLOR_SILVER = "silver";

        ///<summary />
        public static readonly Color Silver = Create(COLOR_SILVER);


        ///<summary />
        public const string COLOR_SNOW = "snow";

        ///<summary />
        public static readonly Color Snow = Create(COLOR_SNOW);


        ///<summary />
        public const string COLOR_THISTLE = "thistle";

        ///<summary />
        public static readonly Color Thistle = Create(COLOR_THISTLE);


        ///<summary />
        public const string COLOR_WHITE = "white";

        ///<summary />
        public static readonly Color White = Create(COLOR_WHITE);


        ///<summary />
        public const string COLOR_AZURE = "azure";

        ///<summary />
        public static readonly Color Azure = Create(COLOR_AZURE);


        ///<summary />
        public const string COLOR_BLUE = "blue";

        ///<summary />
        public static readonly Color Blue = Create(COLOR_BLUE);


        ///<summary />
        public const string COLOR_CHARTREUSE = "chartreuse";

        ///<summary />
        public static readonly Color Chartreuse = Create(COLOR_CHARTREUSE);


        ///<summary />
        public const string COLOR_CRIMSON = "crimson";

        ///<summary />
        public static readonly Color Crimson = Create(COLOR_CRIMSON);


        ///<summary />
        public const string COLOR_DARKGRAY = "darkgray";

        ///<summary />
        public static readonly Color Darkgray = Create(COLOR_DARKGRAY);


        ///<summary />
        public const string COLOR_DARKOLIVEGREEN = "darkolivegreen";

        ///<summary />
        public static readonly Color Darkolivegreen = Create(COLOR_DARKOLIVEGREEN);


        ///<summary />
        public const string COLOR_DARKSEAGREEN = "darkseagreen";

        ///<summary />
        public static readonly Color Darkseagreen = Create(COLOR_DARKSEAGREEN);


        ///<summary />
        public const string COLOR_DARKVIOLET = "darkviolet";

        ///<summary />
        public static readonly Color Darkviolet = Create(COLOR_DARKVIOLET);


        ///<summary />
        public const string COLOR_DODGERBLUE = "dodgerblue";

        ///<summary />
        public static readonly Color Dodgerblue = Create(COLOR_DODGERBLUE);


        ///<summary />
        public const string COLOR_GAINSBORO = "gainsboro";

        ///<summary />
        public static readonly Color Gainsboro = Create(COLOR_GAINSBORO);


        ///<summary />
        public const string COLOR_GREY = "grey";

        ///<summary />
        public static readonly Color Grey = Create(COLOR_GREY);


        ///<summary />
        public const string COLOR_INDIANRED = "indianred";

        ///<summary />
        public static readonly Color Indianred = Create(COLOR_INDIANRED);


        ///<summary />
        public const string COLOR_LAVENDERBLUSH = "lavenderblush";

        ///<summary />
        public static readonly Color Lavenderblush = Create(COLOR_LAVENDERBLUSH);


        ///<summary />
        public const string COLOR_LIGHTCYAN = "lightcyan";

        ///<summary />
        public static readonly Color Lightcyan = Create(COLOR_LIGHTCYAN);


        ///<summary />
        public const string COLOR_LIGHTPINK = "lightpink";

        ///<summary />
        public static readonly Color Lightpink = Create(COLOR_LIGHTPINK);


        ///<summary />
        public const string COLOR_LIGHTSLATEGREY = "lightslategrey";

        ///<summary />
        public static readonly Color Lightslategrey = Create(COLOR_LIGHTSLATEGREY);


        ///<summary />
        public const string COLOR_LINEN = "linen";

        ///<summary />
        public static readonly Color Linen = Create(COLOR_LINEN);


        ///<summary />
        public const string COLOR_MEDIUMORCHID = "mediumorchid";

        ///<summary />
        public static readonly Color Mediumorchid = Create(COLOR_MEDIUMORCHID);


        ///<summary />
        public const string COLOR_MEDIUMTURQUOISE = "mediumturquoise";

        ///<summary />
        public static readonly Color Mediumturquoise = Create(COLOR_MEDIUMTURQUOISE);


        ///<summary />
        public const string COLOR_MOCCASIN = "moccasin";

        ///<summary />
        public static readonly Color Moccasin = Create(COLOR_MOCCASIN);


        ///<summary />
        public const string COLOR_OLIVEDRAB = "olivedrab";

        ///<summary />
        public static readonly Color Olivedrab = Create(COLOR_OLIVEDRAB);


        ///<summary />
        public const string COLOR_PALEGREEN = "palegreen";

        ///<summary />
        public static readonly Color Palegreen = Create(COLOR_PALEGREEN);


        ///<summary />
        public const string COLOR_PERU = "peru";

        ///<summary />
        public static readonly Color Peru = Create(COLOR_PERU);


        ///<summary />
        public const string COLOR_RED = "red";

        ///<summary />
        public static readonly Color Red = Create(COLOR_RED);


        ///<summary />
        public const string COLOR_SANDYBROWN = "sandybrown";

        ///<summary />
        public static readonly Color Sandybrown = Create(COLOR_SANDYBROWN);


        ///<summary />
        public const string COLOR_SKYBLUE = "skyblue";

        ///<summary />
        public static readonly Color Skyblue = Create(COLOR_SKYBLUE);


        ///<summary />
        public const string COLOR_SPRINGGREEN = "springgreen";

        ///<summary />
        public static readonly Color Springgreen = Create(COLOR_SPRINGGREEN);


        ///<summary />
        public const string COLOR_TOMATO = "tomato";

        ///<summary />
        public static readonly Color Tomato = Create(COLOR_TOMATO);


        ///<summary />
        public const string COLOR_WHITESMOKE = "whitesmoke";

        ///<summary />
        public static readonly Color Whitesmoke = Create(COLOR_WHITESMOKE);





    }
}