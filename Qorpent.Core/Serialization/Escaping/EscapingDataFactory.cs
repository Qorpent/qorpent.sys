﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qorpent.Serialization.Escaping
{
    class EscapingDataFactory
    {
        private static readonly Dictionary<EscapingType, IData> _data = new Dictionary<EscapingType, IData>()
        {
            {EscapingType.XmlName, new XmlName()},
            {EscapingType.XmlAttribute, new XmlAttribute()},
            {EscapingType.BxlLiteral, new BxlLiteral()},
            {EscapingType.JsonLiteral, new JsonLiteral()},
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
