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
        //Literal = ElementName | AttributeName,
        SingleLineString = 8,
        MultiLineString = 16,
        Indent = 32,
		NewLine = 64,
        Quoting1 = 128,
		Quoting2 = 256,
		Unquoting = 512,
		//Commentary,
		EscapingBackSlash = 1024,
		Expression = 2048,
		TextContent = 4096,
		WaitingForNL = 8192,
    }
}
