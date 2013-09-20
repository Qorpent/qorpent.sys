using System;
using System.Collections.Generic;

namespace Qorpent.Serialization
{
    interface IData
    {
        /// <summary>
        /// special escape dictionary for first character
        /// </summary>
        /// <returns></returns>
        Dictionary<char, string> GetFirst();
        /// <summary>
        /// escape dictionary for all characters
        /// </summary>
        /// <returns></returns>
        Dictionary<char, string> GetCommon();
        /// <summary>
        /// unescape dictionry for all characters
        /// </summary>
        /// <returns></returns>
        OptimizedEscapeDictionary GetUnescape();
        /// <summary>
        /// pattern for escaping other symbols with their codes (hex)
        /// " " (space) - for value
        /// </summary>
        /// <returns>pattern or null if unicode escaping not defined</returns>
        String GetUnicodePattern();
    }
}
