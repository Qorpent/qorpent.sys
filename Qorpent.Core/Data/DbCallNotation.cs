using System;

namespace Qorpent.Data {
    /// <summary>
    /// Вариант вызова обертки
    /// </summary>
    [Flags]
    public enum DbCallNotation {
        /// <summary>
        /// void, noquery
        /// </summary>
        None = 0,
        /// <summary>
        /// single reader
        /// </summary>
        Reader = 1,
        /// <summary>
        /// Множественный результат
        /// </summary>
        Multiple = 2,
        /// <summary>
        /// Строго единичный запрос
        /// </summary>
        Single = 4,
        /// <summary>
        /// Объектный результат
        /// </summary>
        Object = 8,

        /// <summary>
        /// scalar
        /// </summary>
        Scalar = 16,
        /// <summary>
        /// Simple object reader
        /// </summary>
        ObjectReader = Reader | Object,
        /// <summary>
        /// multiple reader
        /// </summary>
        MultipleReader = Reader | Multiple,
        
        /// <summary>
        /// multiple Orm set
        /// </summary>
        MultipleObject=Object | Multiple | Reader,
        /// <summary>
        /// single row as dictionary
        /// </summary>
        SingleRow = Reader | Single,
        /// <summary>
        /// single object with Orm
        /// </summary>
        SingleObject = Reader | Single |Object,
       
    }
}