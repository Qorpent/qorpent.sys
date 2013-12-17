using System;

namespace Qorpent.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum EscapingType
    {
        /// <summary>
        /// 
        /// </summary>
        XmlName = 1,
        /// <summary>
        /// value
        /// </summary>
        XmlAttribute = 2,        
        /// <summary>
        /// 
        /// </summary>
        BxlLiteral = 4,
        
        /// <summary>
        /// 
        /// </summary>
        JsonLiteral = 8,
        /// <summary>
        /// 
        /// </summary>
        DotLiteral = 16,
        
        /// <summary>
        /// Строка BXL с escape для спец-символов
        /// </summary>
        BxlSinglelineString = 32,
        /// <summary>
        /// Строка BXL c поддержкой многострочности
        /// </summary>
        BxlMultilineString = 64,
		/// <summary>
		/// Однострочка строка BXL с поддержкой режима "литерал"
		/// </summary>
	    BxlStringOrLiteral =128,
		/// <summary>
		/// Экранированная JSON-строка
		/// </summary>
	    JsonValue = 256,
    }
}
