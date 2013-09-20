using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qorpent.Serialization.Escaping
{
    /// <summary>
    /// 
    /// </summary>
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
        DotLiteral = 16
    }
}
