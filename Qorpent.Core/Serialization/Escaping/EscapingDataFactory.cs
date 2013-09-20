﻿using System.Collections.Generic;
using Qorpent.Serialization.Escaping;

namespace Qorpent.Serialization
{
    class EscapingDataFactory
    {
        private static readonly Dictionary<EscapingType, IData> _data = new Dictionary<EscapingType, IData>()
        {
            {EscapingType.XmlName, new XmlName()},
            {EscapingType.XmlAttribute, new XmlAttribute()},
            {EscapingType.BxlLiteral, new BxlLiteral()},
            {EscapingType.JsonLiteral, new JsonLiteral()},
            {EscapingType.DotLiteral, new DotLiteral()},
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IData Get(EscapingType type)
        {
            return _data[type];
        }
    }
}
