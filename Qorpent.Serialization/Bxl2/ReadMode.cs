using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qorpent.Bxl2
{
    enum ReadMode
    {
        Start = 0,
        ElementName,
        AttributeName,
        AttributeValue,
        SingleLineString,
        MultiLineString,
        Indent,
		NewLine,
        Quoting1,
		Quoting2,
		Unquoting,
		EscapingBackSlash,
		Expression,
		TextContent,
		WaitingForNL,
		NamespaceName,
		NamespaceValue,
		Colon,
		Commentary
    }
}
