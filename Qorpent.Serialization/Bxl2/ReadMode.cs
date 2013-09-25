using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qorpent.Bxl2
{
    enum ReadMode
    {
        //Start = 0,
        ElementName = 1,
        AttributeName = 2,
        AttributeValue = 4,
        Literal = ElementName | AttributeName,
        SingleLineString = 8,
        //MultiLineString = 16,
        //String = SingleLineString | MultiLineString,
        //Text = Literal | String,
        Indent = 32,
        //Quoting = 64,
        //Colon = 128,
        NewLine = 256,
		//Commentary = 512,
		EscapingBackSlash = 1024,
    }
}
