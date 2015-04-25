namespace Qorpent.Bxl
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
