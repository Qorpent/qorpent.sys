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
        /*
        /// <summary>
        /// 
        /// </summary>
        Json = 8,
        /// <summary>
        /// 
        /// </summary>
        Dot = 16
        */
        /// <summary>
        /// Строка BXL с escape для спец-символов
        /// </summary>
        BxlSinglelineString = 32,
        /// <summary>
        /// Строка BXL c поддержкой многострочности
        /// </summary>
        BxlMultilineString = 64,

    }
}
