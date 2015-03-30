using System;

namespace Qorpent.Data {
    /// <summary>
    /// Вариант вызова обертки
    /// </summary>
    [Flags]
    public enum SqlCallNotation {
        /// <summary>
        /// void, noquery
        /// </summary>
        None = 0,
        /// <summary>
        /// single reader
        /// </summary>
        Reader = 1,
        /// <summary>
        /// multiple reader
        /// </summary>
        MultipleReader = 2,
        /// <summary>
        /// scalar
        /// </summary>
        Scalar = 4,
        /// <summary>
        /// automatic Orm
        /// </summary>
        ObjectReader = 8,
        /// <summary>
        /// multiple Orm set
        /// </summary>
        MultipleObject=16,
        /// <summary>
        /// single row as dictionary
        /// </summary>
        SingleRow = 32,
        /// <summary>
        /// single object with Orm
        /// </summary>
        SingleObject = 64,
        /// <summary>
        /// признак того, что предполагается чтение в табличном виде
        /// </summary>
        ReaderBased = Reader | MultipleReader | ObjectReader | SingleRow | MultipleObject | SingleObject
    }
}